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
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers(int? id = null, string name = "", string firstName = "", bool expand = false)
        {
            if (!id.HasValue && expand) {
                return BadRequest("expand can only be used if one and only one record is requested via providing an id!");
            }            

            using var _context = new FlightContext();

            var passengers =  _context.Passengers.AsQueryable();
                
            if (expand) {
                passengers = passengers.Include(p => p.FlightBookings).ThenInclude(fb => fb.Flight);
            }

            if (id.HasValue) {
                passengers = passengers.Where(p => p.Id == id.Value);
            }

            if (!string.IsNullOrEmpty(name)) {
                passengers = passengers.Where(p => p.Name.StartsWith(name));
            }

            if (!string.IsNullOrEmpty(firstName)) {
                passengers = passengers.Where(p => p.FirstName.StartsWith(firstName));
            }

            return await passengers.ToListAsync();
        }

        // GET: api/Passenger/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetPassenger(long id, bool expand = false)
        {
            using var _context = new FlightContext();

            var query =  _context.Passengers.AsQueryable();

            if (expand) {
                query = query.Include(p => p.FlightBookings).ThenInclude(fb => fb.Flight);
            }
            
            var passenger = await query.Where(p => p.Id == id).FirstAsync();

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

            if (id < 10 && id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

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
            if (passenger.Id < 10 && passenger.Id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

            using var _context = new FlightContext();

            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassenger", new { id = passenger.Id }, passenger);
        }

        // DELETE: api/Passenger/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Passenger>> DeletePassenger(long id)
        {
            if (id < 10 && id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

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
