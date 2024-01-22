

namespace DataFormats {


    //----Question 2-----

    public record BoardingPassWithName : BoardingPass
    {
        public string Name{get; set;}
        public BoardingPassWithName(int FlightID, int TicketID, decimal Fare, string SeatNumber, string name, DateTime issueDate) : base(FlightID, TicketID, Fare, SeatNumber, issueDate)
        {
            Name = name;
        }

        public BoardingPassWithName(BoardingPass bp, string name) : base(bp)
        {
            Name = name;
        }
    }


    //----Question 3-----

    public record BookingOverview(List<Tuple<string, string>> FlightDetails, decimal TotalFare);


    //----Question 4-----
    
    public record SeatsInFlight(int FlightID, int TotalSeats);


    //----Question 6-----

    public record FlightInfo(int FlightID, string Dept, string Arr);


    //----Question 7-----
    

    //Utilities in order to generate random DateTime and DateOnly for Flights and BoardingPasses
    public static class DateTimeUtils{
        //FlightDates:
        public static DateTime RandomDateTime() {
            var rand = new Random();
            return new DateTime(2024, rand.Next(1, 12), rand.Next(1, 28), rand.Next(24), rand.Next(60), 0, DateTimeKind.Utc);
        }
        //Booking dates:
        public static DateOnly RandomDateOnly(){
            var rand = new Random();
            return new DateOnly(2022, rand.Next(1, 12), rand.Next(1, 28));
        }
    }

}