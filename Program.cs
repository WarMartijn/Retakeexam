

using (var db = new FlightContext())
{
    //Data.ClearDB(db);
    //Data.SeedAll(db);

    var rand = new Random();

    //TestQ1
    
    System.Console.WriteLine("\n---Q1----");
    var airport = Data.Airports[rand.Next(0, Data.Airports.Length)];
    System.Console.WriteLine($"Flights from or to {airport}:");
    if(airport != null) {
        var res = Solution.Q1(db, airport);
        res?.ToList().ForEach(_ => Console.WriteLine(_)); 
    }
   
    //TestQ2
    
    Console.WriteLine("\n---Q2----");
    var numOfTickets = db.Tickets.Count();
    var name = db.Tickets.Skip(numOfTickets / 2 + rand.Next(-2, 3)).Take(1).Select(_ => _.Name).ToList().FirstOrDefault();
    Console.WriteLine($"Boardingpasses for person {name}");
    if (name != null) {
      var res = Solution.Q2(db, name);
      res?.ToList().ForEach(_ => Console.WriteLine(_)); 
    }
    
    //TestQ3
    
    Console.WriteLine("\n---Q3----");
    var bookingNumberCount = db.Bookings.Count();
    var bookingId = bookingNumberCount / 2 + rand.Next(-3, 3);
    
    Console.WriteLine($"\nBookingID: {bookingId}");
    var resQ3 = Solution.Q3(db, bookingId);
    var i = 1;
    if (resQ3 != null) {
      resQ3?.FlightDetails.ForEach(_ => Console.WriteLine($"Flight{i++} Departure:{_.Item1} -> Arrival:{_.Item2}"));
      Console.WriteLine($"TotalFare: {resQ3.TotalFare}");
    } 
    
    //TestQ4
    
    Console.WriteLine("\n---Q4----");
    var q4 = Solution.Q4(db)?.ToList();
    
    if(q4 != null) {
        foreach (var g in q4)
        {   
        if(g.TotalSeats == 0) {
            Console.WriteLine($"Flight ID: {g.FlightID}");
            Console.WriteLine("<---No Seats booked---->");
        }
        else{
            Console.WriteLine($"Flight ID: {g.FlightID}");
            Console.WriteLine($"Seat booked total: {g.TotalSeats}");
        }
        }
    }

    //TestQ5

    Console.WriteLine("\n----Q5----");
    Solution.Q5(db)?.ToList().ForEach(_ => Console.WriteLine(_));

    //TestQ6
    
    Console.WriteLine("\n----Q6----");
    bookingNumberCount = db.Bookings.Count();
    bookingId = bookingNumberCount / 2 + rand.Next(-3, 3);
    List<int> TicketIds = db.Tickets.Where(_ => _.BookingRef == bookingId).Select(_ => _.Id).Take(2).ToList();
    
    if (TicketIds.Count == 2)  {
        Console.WriteLine($"Ticket IDs: {TicketIds[0]}, {TicketIds[1]}\n");
        var flightMerge = Solution.Q6(db, TicketIds[0], TicketIds[1])?.ToList();
        flightMerge?.ForEach(_ => Console.WriteLine(_));
    }

    //TestQ7
    
    Console.WriteLine("\n----Q7----");
    var totalBookings = db.Bookings.Count();
    var totalTickets = db.Tickets.Count();
    var totalFlights = db.Flights.Count();
    var totalBPs = db.BoardingPasses.Count();

    Solution.Q7(db);

    var totalBookings_after = db.Bookings.Count();
    var totalTickets_after = db.Tickets.Count();
    var totalFlights_after = db.Flights.Count();
    var totalBPs_after = db.BoardingPasses.Count();
    Console.WriteLine($"  {totalFlights_after - totalFlights} flights");
    Console.WriteLine($"  {totalBookings_after - totalBookings} bookings");
    Console.WriteLine($"  {totalTickets_after - totalTickets} tickets");
    Console.WriteLine($"  {totalBPs_after - totalBPs} boardingpasses");
    
}


