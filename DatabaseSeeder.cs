using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class DatabaseSeeder {

    private string[] airports = {
        "Graz", "Wien", "Salzburg", "Linz",
        "Hamburg", "München", "Nürnberg", "Hamburg", "Berlin", 
        "Stuttgart", "Frankfurt", "Bremen",
        "Zürich",
        "Paris", "London", "Madrid", "Rom",
        "Atlanta", "Los Angeles", "Chicago", "Dallas", "Denver", "New York",
        "San Francisco", "Seattle", "Las Vegas", "Orlando", "Phoenix", "Houston",
        "Miami", "Boston", "Detroit", "Salt Lake City", "Washington"
    };

    // Source: https://www.beliebte-vornamen.de/jahrgang/j2018/top-500-2018
    private string[] firstNames = {
        "Emma", "Mia", "Hanna", "Emilia", "Sofia", "Lina", "Anna",
        "Ben", "Paul", "Leon", "Finn", "Elias", "Jonas", "Luis", "Noah"
    };

    private string[] lastNames = {
        "Muster", "Sorglos",
        "Müller", "Schmidt", "Schneider", "Fischer", "Weber", 
        "Meyer", "Wagner", "Becker", "Schulz",
        "Smith", "Jones", "Taylor", "Williams", "Brown", "Davies", "Evans",
        "Wilson", "Thomas", "Roberts", "Johnson", "Lewis", "Walker"
    };


    // Creates some static default data
    private Tuple<Flight[], Passenger[], FlightBooking[]> CreateDefaults() {

        var flight1 = new Flight { 
            // Id = 1, 
            From = "Hamburg", To="Graz", 
            Date=DateTime.Now.AddDays(20), Delayed = false };
        
        var flight2 = new Flight { 
            // Id = 2, 
            From = "Hamburg", To="Graz", 
            Date=DateTime.Now.AddDays(21), Delayed = true };

        var flight3 = new Flight { 
            // Id = 3, 
            From = "Graz", To="Hamburg", 
            Date=DateTime.Now.AddDays(20), Delayed = false };
        
        var flight4 = new Flight { 
            // Id = 4, 
            From = "Graz", To="Hamburg", 
            Date=DateTime.Now.AddDays(21), Delayed = true };

        var flight5 = new Flight { 
            // Id = 5, 
            From = "Graz", To="Hamburg", 
            Date=DateTime.Now.AddDays(22), Delayed = false };

        var passenger1 = new Passenger {
            // Id = 1, 
            Name = "Muster", FirstName = "Max", 
            BonusMiles = 20000, PassengerStatus = "A"
        };

        var passenger2 = new Passenger {
            // Id = 2, 
            Name = "Susi", FirstName = "Sorglos", 
            BonusMiles = 21000, PassengerStatus = "A"
        };

        var passenger3 = new Passenger {
            // Id = 3, 
            Name = "Anna", FirstName = "Muster", 
            BonusMiles = 7000, PassengerStatus = "B"
        };

        var booking1 = new FlightBooking { 
            // Id = 1, 
            BookingDate = DateTime.Now.AddDays(-5),
            Passenger = passenger1, Flight = flight1,
            Seat = "1D", FlightClass = 2
        };

        var booking2 = new FlightBooking { 
            // Id = 2, 
            BookingDate = DateTime.Now.AddDays(-3),
            Passenger = passenger2, Flight = flight1,
            Seat = "1F", FlightClass = 2
        };

        var booking3 = new FlightBooking { 
            // Id = 3, 
            BookingDate = DateTime.Now.AddDays(-4),
            Passenger = passenger2, Flight = flight2,
            Seat = "1F", FlightClass = 2
        };

        var booking4 = new FlightBooking { 
            // Id = 4, 
            BookingDate = DateTime.Now.AddDays(-2),
            Passenger = passenger3, Flight = flight3,
            Seat = "1F", FlightClass = 2
        };

        var flights = new Flight[] { flight1, flight2, flight3, flight4, flight5 };
        var passengers = new Passenger[] { passenger1, passenger2, passenger3 };
        var bookings = new FlightBooking[] { booking1, booking2, booking3, booking4 };

        return new Tuple<Flight[], Passenger[], FlightBooking[]>(flights, passengers, bookings);
    }

    private Tuple<Flight[], Passenger[], FlightBooking[]> createRandom() {

        var flights = new List<Flight>();
        var passengers = new List<Passenger>();
        var bookings = new List<FlightBooking>();

        // var nextId = 10;
        var rand = new Random();

        foreach (var from in airports) {
            foreach (var to in airports) {

                if (from == to) continue;

                for(var i=0; i< rand.Next(10); i++ ) {

                    flights.Add(new Flight {
                        // Id = nextId++,
                        From = from,
                        To = to,
                        Date = DateTime.Now.AddDays(20 + i),
                        Delayed = rand.NextDouble() < 0.3
                    });

                }
            }
        }

        foreach (var firstName in firstNames) {
            foreach (var lastName in lastNames) {
                passengers.Add(new Passenger {
                    // Id = nextId++,
                    FirstName = firstName,
                    Name = lastName,
                    BonusMiles = rand.Next(500000),
                    PassengerStatus = (rand.NextDouble() < 0.5) ? "A" : "B"
                });
            }
        }

        var bookingCount = this.firstNames.Length * this.lastNames.Length * 2;

        for(var i=0; i<bookingCount; i++) {
            var passenger = passengers[rand.Next(passengers.Count)];
            var flight = flights[rand.Next(Math.Min(flights.Count, 10))];

            bookings.Add(new FlightBooking {
                // Id = nextId++,
                BookingDate = DateTime.Now.AddDays(i % 5 * -1),
                Seat = "",
                FlightClass = rand.Next(2) + 1,
                Passenger = passenger,
                Flight = flight
            });

        }

        return new Tuple<Flight[], Passenger[], FlightBooking[]>(
            flights.ToArray(), passengers.ToArray(), bookings.ToArray()
        );

    }

    public void SeedDatabase()
        {
            using (var context = new FlightContext())
            {
                var defaults = CreateDefaults();
                var random = createRandom();

                context.Flights.AddRange(defaults.Item1);
                context.Flights.AddRange(random.Item1);

                context.Passengers.AddRange(defaults.Item2);
                context.Passengers.AddRange(random.Item2);

                context.FlightBookings.AddRange(defaults.Item3);
                context.FlightBookings.AddRange(random.Item3);

                context.SaveChanges();
            }
        }
}