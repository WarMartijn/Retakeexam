
public class FlightContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseNpgsql(@"Host=localhost:5432;Username=postgres;Password=;Database=FlightBooking;Maximum Pool Size=200");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>().HasKey(_ => _.Ref);
        modelBuilder.Entity<BoardingPass>().HasKey(_ => new { _.FlightID, _.TicketID });
        
        modelBuilder.Entity<Ticket>()
        .HasOne(_ => _.Booking)
        .WithMany()
        .HasForeignKey(_ => _.BookingRef)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

        modelBuilder.Entity<BoardingPass>()
        .HasOne<Ticket>()
        .WithMany()
        .HasForeignKey(_ => _.TicketID)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

        modelBuilder.Entity<BoardingPass>()
        .HasOne<Flight>()
        .WithMany()
        .HasForeignKey(_ => _.FlightID)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

    }
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; }=null!;
    public DbSet<Flight> Flights { get; set; } = null!;
    public DbSet<BoardingPass> BoardingPasses { get; set; } = null!;
}

public record Booking(int Ref, DateOnly Date);

public record Ticket {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Booking Booking { get; set; } = null!;
    public int BookingRef { get; set; }
};

public record Flight (int Id , string DepartureAirport, string ArrivalAirport, DateTime DepartureTime, DateTime ArrivalTime );

public record BoardingPass (
    int FlightID,
    int TicketID,
    decimal Fare,
    string SeatNumber,
		DateTime IssueTime
);

public class Data {

    static public string[] Airports = new string[] { "AMS", "JFK", "LHR", "FCO", "SIN", "DXB", "CDG", "INN"};

    public static void ClearDB(FlightContext db) {
        string truncateTables = @"
                    TRUNCATE TABLE ""Bookings"" CASCADE;
                    TRUNCATE TABLE ""BoardingPasses"" CASCADE;
                    TRUNCATE TABLE ""Tickets"" CASCADE;
                    TRUNCATE TABLE ""Flights"" CASCADE;
                ";
        db.Database.ExecuteSqlRaw(truncateTables);         
    }

    public static void SeedAll(FlightContext db) {
        //Add some flights 
        int numOfFlights = 30; //for some flights there is a return flight.
        SeedFlights(db, numOfFlights);
				numOfFlights = db.Flights.Count();//due the randomness in the seeding the actual number of inserted flights might be different than 30
        var rand = new Random();

        var ID = 1;
        for (int bookingID = 1; bookingID <= 10; bookingID++) {
            //1. Add Booking
            db.Bookings.Add(new Booking (bookingID, DateTimeUtils.RandomDateOnly()));
            //2. Add Random Tickets per booking
            var Px = rand.Next(2, 3);

            for (int idx = 1; idx <= Px; idx++, ID++) {
                db.Tickets.Add(new Ticket() {Id = ID, BookingRef = bookingID, Name=$"Person {bookingID} : {idx}"}); 
                //3. book some flight, return or one way and one stop or multiple stops.
                var NrOfConnections = rand.Next(4, 7);
                    
                //4. random flights
                var FlightIds = Enumerable.Range(1, numOfFlights).OrderBy(x => rand.Next()).ToArray();
                var fIdx = 1;
								var listOfAirportIDs = new List<int>();
                for (int fc = 1; fc <= NrOfConnections; fc++) {
                    if(fc == 1) {
											fIdx = FlightIds[fc];
											listOfAirportIDs.Add(fIdx);
											db.BoardingPasses.Add(new BoardingPass(fIdx, ID, new decimal(rand.Next(10, 2000)), rand.Next(30).ToString(), DateTime.UtcNow)); 
										}
										else{
											var depAirport = db.Flights.Where(_ => _.Id == fIdx).Select(_ => _.DepartureAirport).FirstOrDefault();
											var arrAirport = db.Flights.Where(_ => _.Id == fIdx).Select(_ => _.ArrivalAirport).FirstOrDefault();

											var nextFlight = db.Flights.Where(_ => _.Id != fIdx && _.DepartureAirport == arrAirport && _.ArrivalAirport != depAirport).FirstOrDefault();
											if(nextFlight != null && !listOfAirportIDs.Contains(nextFlight.Id)) {
												fIdx = nextFlight.Id;
												listOfAirportIDs.Add(fIdx);
												db.BoardingPasses.Add(new BoardingPass(fIdx, ID, new decimal(rand.Next(10, 2000)), rand.Next(30).ToString(), DateTime.UtcNow)); 
											}
											else {
												var totalFlights = db.Flights.Count();
												var randDate = DateTimeUtils.RandomDateTime();
												var nextAirport = Airports.Where(_ => _ != arrAirport && _ != depAirport).FirstOrDefault();
												fIdx = totalFlights + 1;
												listOfAirportIDs.Add(fIdx);
												var flightToAdd = new Flight(fIdx, arrAirport, nextAirport, randDate, randDate.AddHours(rand.NextDouble()*24.0));
							          db.Flights.Add(flightToAdd);
                        db.SaveChanges();
												db.BoardingPasses.Add(new BoardingPass(fIdx, ID, new decimal(rand.Next(10, 2000)), rand.Next(30).ToString(), DateTime.UtcNow)); 
											}
										}
								}
            }
        }
        db.SaveChanges();
        Console.WriteLine($"Data seeding:");
        Console.WriteLine($"{db.Flights.Count()} flights in total added to database");
        Console.WriteLine($"{db.Bookings.Count()} bookings in total added to database");
        Console.WriteLine($"{db.Tickets.Count()} tickets in total added to database");
        Console.WriteLine($"{db.BoardingPasses.Count()} boarding passes in total added to database");
    }

    public static void SeedFlights(FlightContext db, int numOfFlights) {
        var rand  = new Random();
				var AirportIds = Enumerable.Range(0, Airports.Length).OrderBy(x => rand.Next()).ToArray();
        for (int i = 1; i < numOfFlights - 1; i++) {
            var AirportIndex = AirportIds[(i - 1) % AirportIds.Length];					
            var randDate = DateTimeUtils.RandomDateTime();
            db.Flights.Add(new Flight(i, Airports[AirportIndex], Airports[(AirportIndex+3)%Airports.Length], randDate, randDate.AddHours(rand.NextDouble()*24.0)));
            
						randDate = DateTimeUtils.RandomDateTime();
						db.Flights.Add(new Flight(++i, Airports[(AirportIndex+3)%Airports.Length], Airports[(AirportIndex+rand.Next(1,2))%Airports.Length], randDate, randDate.AddHours(rand.NextDouble()*24.0)));

					  if(rand.NextDouble() > 0.5) {
							randDate = DateTimeUtils.RandomDateTime();
							db.Flights.Add(new Flight(++i, Airports[(AirportIndex+3)%Airports.Length], Airports[AirportIndex], randDate, randDate.AddHours(rand.NextDouble()*24.0)));
						}
				}
        db.SaveChanges();
    }  
}