
class Solution{

    //Q1: Select all the outbound and inbound flights at given airport, the arrival time should be used to order the result.
    public static IQueryable<Flight> Q1(FlightContext db, string airport) {
        var query = (from f in db.Flights where f.DepartureAirport == airport || f.ArrivalAirport == airport select f)
        .OrderBy(f=>f.ArrivalTime).AsQueryable();
        return query;  //this line of code should be changed 

    }

    //Q2: For given person name 
    //    find the boarding passes 
    // (flight id, Ticket id, fare, seat number and issue date) 
    //with passenger name [BoardingPassWithName].
    public static IQueryable<BoardingPassWithName> Q2(FlightContext db, string person) {
        var query = (from t in db.Tickets 
                    join b in db.BoardingPasses on t.Id equals b.TicketID
                    where t.Name == person 
                    select new BoardingPassWithName(b,t.Name));
        return query;  //this line of code should be changed 
        
    }

    //Q3: Returns an instance of BookingOverview for a given booking: 
    //    List of Tuples containing Departure and Arrival airports (FlightDetails); 
    //    Calculate the total fare of given booking (TotalFare).
    public static BookingOverview Q3(FlightContext db, int booking) {   
           return default;
    }

    //Q4: List down number of seats booked (TotalSeats) per flight (FlightID)  [SeatsInFlight]
    //    do not forget to include flights with no Boarding passes issued (no seats booked), 
    //    as well if any -> LEFT JOIN
    //    Using the Sum method might be useful to compute TotalSeats.
    
    public static IQueryable<SeatsInFlight> Q4(FlightContext db) {
        var query = (from bo in db.BoardingPasses 
                    join f in db.Flights on bo.FlightID equals f.Id into grp
                    from _ in grp.DefaultIfEmpty()
                    select new SeatsInFlight(_.Id, grp.Count()));
        return query;  //this line of code should be changed   
              
    }
 
    //Q5: List down the flights [if any] that were never booked
    public static IQueryable<Flight> Q5(FlightContext db) {
        
        var flights =(from f in db.Flights 
                        where !db.BoardingPasses.Any(b=>b.FlightID == f.Id)
                        select f);
        return flights;  //this line of code should be changed 
        
    }
    
    //Q6: Given two ticket IDs, 
    //    merge the FlightInfo elements (projection of Flight entity) belonging to both given tickets 
    //    WITHOUT repetitions.
    public static List<FlightInfo> Q6(FlightContext db, int TicketID1, int TicketID2) {
          
          var query1 = (from bo in db.BoardingPasses
                        join t in db.Tickets on bo.TicketID equals t.Id 
                        join f in db.Flights on bo.FlightID equals f.Id 
                        where t.Id == TicketID1
                        select new FlightInfo(bo.FlightID,f.DepartureAirport,f.ArrivalAirport));
          var query2 = (from bo in db.BoardingPasses
                        join t in db.Tickets on bo.TicketID equals t.Id 
                        join f in db.Flights on bo.FlightID equals f.Id 
                        where t.Id == TicketID2
                        select new FlightInfo(bo.FlightID,f.DepartureAirport,f.ArrivalAirport));
        var final = query1.Except(query2).ToList();
        return final;  //this line of code should be changed 
        
    }

    //Q7: Create a new flight, new booking, new ticket and a new boarding pass
    //    and make the changes persistent.
    //    HINT: having a look at the implementation of the seed methods in Data class (FlightModel.cs) can be useful.
    //          as well as DateTimeUtils methods in DataFormats.cs
    public static void Q7(FlightContext db) {
        var newFlight = new Flight(10000000,"A","B",DateTimeUtils.RandomDateTime(),DateTimeUtils.RandomDateTime());
        var newBooking = new Booking(200,DateTimeUtils.RandomDateOnly());
        var newTicket = new Ticket{
            Id = 1000,
            Name = "Wow",
            Booking=newBooking,
            BookingRef = newBooking.Ref
        };
        var newPass = new BoardingPass(newFlight.Id, newTicket.Id,1000000,"1203",DateTimeUtils.RandomDateTime());
        db.AddRange(newFlight,newBooking,newTicket,newPass);
        db.SaveChanges();
    }
}


