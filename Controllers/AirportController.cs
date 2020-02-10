using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class AirportController : ControllerBase
{
    // GET: api/Booking
    [HttpGet]
    public string[] Get() {
        return DatabaseSeeder.airports.OrderBy(a => a).ToArray();
    }
}
