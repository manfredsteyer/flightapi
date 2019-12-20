using Microsoft.EntityFrameworkCore;

public class FlightContext : DbContext
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<FlightBooking> FlightBookings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseInMemoryDatabase("flight-db");
        //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=flightdb;Trusted_Connection=True;MultipleActiveResultSets=true");
    }
}
