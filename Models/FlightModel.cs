using System;
using System.Collections.Generic;

public class Flight {
    public long Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public DateTime Date { get; set; }
    public bool Delayed { get; set; }

    public List<FlightBooking> FlightBookings { get; set; } = new List<FlightBooking>();
}

public class Passenger {
    public long Id { get; set; }
    public string Name { get; set; }
    public string FirstName { get; set; }
    public int BonusMiles { get; set; }
    public string PassengerStatus { get; set; }
    public List<FlightBooking> FlightBookings { get; set; } = new List<FlightBooking>();
}

public class FlightBooking {
    public long Id { get; set; }
    public long PassengerId { get; set; }
    public long FlightId { get; set; }
    public DateTime BookingDate { get; set;}
    
    public int FlightClass { get; set; }
    public string Seat { get; set; }

    public Passenger Passenger { get; set; }
    public Flight Flight { get; set; }
}