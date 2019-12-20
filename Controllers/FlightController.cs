using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace flight_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {

        // GET: api/Flight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights(string from = "", string to = "")
        {
            List<Flight> result;

            using (var context = new FlightContext()) {

                var flights = context.Flights.AsQueryable();

                flights = flights.Where(f => (f.From == from || from == "") && (f.To == to || to == ""));

                result = await flights.ToListAsync();
            }

            return result;

        }        

        // GET: api/Flight/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(long id)
        {
            using var context = new FlightContext();

            var flight = await context
                                .Flights
                                .Include(f => f.FlightBookings)
                                .ThenInclude(fb => fb.Passenger)
                                .Where(f => f.Id == id)
                                .FirstAsync();

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // PUT: api/Flight/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlight(long id, Flight flight)
        {
            using var context = new FlightContext();

            if (id != flight.Id)
            {
                return BadRequest();
            }

            context.Entry(flight).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Flight
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            using var context = new FlightContext();

            context.Flights.Add(flight);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetFlight", new { id = flight.Id }, flight);
        }

        // DELETE: api/Flight/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(long id)
        {
            using var context = new FlightContext();

            var flight = await context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }

            context.Flights.Remove(flight);
            await context.SaveChangesAsync();

            return flight;
        }

        private bool FlightExists(long id)
        {
            using var context = new FlightContext();

            return context.Flights.Any(e => e.Id == id);
        }
    }
}
