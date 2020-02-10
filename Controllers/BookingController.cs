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
    public class BookingController : ControllerBase
    {
      
        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightBooking>>> GetFlightBookings(long? id = null, long? flightId = null, long? passengerId = null, bool expand = false)
        {
            if (!id.HasValue && expand) {
                return BadRequest("expand can only be used if one and only one record is requested via providing an id!");
            }  

            using var _context = new FlightContext();

            var query = _context.FlightBookings.AsQueryable();

            if (expand) {
                query = query.Include(fb => fb.Flight).Include(fb => fb.Passenger);
            }

            if(id.HasValue) {
                query = query.Where(b => b.Id == id.Value);
            }

            if(flightId.HasValue) {
                query = query.Where(b => b.FlightId == flightId.Value);
            }

            if(passengerId.HasValue) {
                query = query.Where(b => b.PassengerId == passengerId.Value);
            }

            if (id.HasValue) {
                return new JsonResult(await query.FirstOrDefaultAsync());
            }
            else {
                return await query.ToListAsync();
            }
                      
        }

        // GET: api/Booking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightBooking>> GetFlightBooking(long id, bool expand = true)
        {
            using var _context = new FlightContext();

            var query = _context.FlightBookings.AsQueryable();
            
            if (expand) {
                query = query.Include(fb => fb.Flight).Include(fb => fb.Passenger);
            }

            var flightBooking = await query.FirstAsync();

            if (flightBooking == null)
            {
                return NotFound();
            }

            return flightBooking;
        }

        // PUT: api/Booking/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightBooking(long id, FlightBooking flightBooking)
        {
            using var _context = new FlightContext();

            if (id < 10 && id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

            if (id != flightBooking.Id)
            {
                return BadRequest();
            }

            _context.Entry(flightBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightBookingExists(id))
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

        // POST: api/Booking
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FlightBooking>> PostFlightBooking(FlightBooking flightBooking)
        {
            if (flightBooking.Id < 10 && flightBooking.Id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

            if (flightBooking.Id != 0) {
                await PutFlightBooking(flightBooking.Id, flightBooking);
                return AcceptedAtAction("GetFlight", new { id = flightBooking.Id }, flightBooking);
            }

            using var _context = new FlightContext();

            _context.FlightBookings.Add(flightBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightBooking", new { id = flightBooking.Id }, flightBooking);
        }

        // DELETE: api/Booking/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightBooking>> DeleteFlightBooking(long id)
        {

            if (id < 10 && id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

            using var _context = new FlightContext();

            var flightBooking = await _context.FlightBookings.FindAsync(id);
            if (flightBooking == null)
            {
                return NotFound();
            }

            _context.FlightBookings.Remove(flightBooking);
            await _context.SaveChangesAsync();

            return flightBooking;
        }

        private bool FlightBookingExists(long id)
        {
            using var _context = new FlightContext();

            return _context.FlightBookings.Any(e => e.Id == id);
        }
    }
}
