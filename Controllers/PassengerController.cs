using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace flight_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {

        // GET: api/Passenger
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers(string name = "", string firstName = "")
        {

            using var _context = new FlightContext();

            return await _context
                            .Passengers
                            .Where(p => 
                                (p.Name == name || name == "") 
                                && (p.FirstName == firstName || firstName == "" ))
                            .ToListAsync();
        }

        // GET: api/Passenger/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetPassenger(long id)
        {
            using var _context = new FlightContext();

            var passenger = await _context
                                    .Passengers
                                    .Include(p => p.FlightBookings)
                                    .ThenInclude(fb => fb.Flight)
                                    .Where(p => p.Id == id)
                                    .FirstAsync();

            if (passenger == null)
            {
                return NotFound();
            }

            return passenger;
        }

        // PUT: api/Passenger/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPassenger(long id, Passenger passenger)
        {
            using var _context = new FlightContext();

            if (id != passenger.Id)
            {
                return BadRequest();
            }

            _context.Entry(passenger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PassengerExists(id))
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

        // POST: api/Passenger
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Passenger>> PostPassenger(Passenger passenger)
        {
            using var _context = new FlightContext();

            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassenger", new { id = passenger.Id }, passenger);
        }

        // DELETE: api/Passenger/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Passenger>> DeletePassenger(long id)
        {
            using var _context = new FlightContext();

            var passenger = await _context.Passengers.FindAsync(id);
            if (passenger == null)
            {
                return NotFound();
            }

            _context.Passengers.Remove(passenger);
            await _context.SaveChangesAsync();

            return passenger;
        }

        private bool PassengerExists(long id)
        {
            using var _context = new FlightContext();
            
            return _context.Passengers.Any(e => e.Id == id);
        }
    }
}
