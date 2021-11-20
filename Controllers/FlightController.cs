using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        // IAuthorizationService authorizationService;

        public FlightController() {
            // this.authorizationService = authorizationService;
        }

        [HttpGet("secure")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Flight>>> GetSecureFlights(int? id = null, string from = "", string to = "", bool expand = false) {
            return await this.GetFlights(id, from, to, expand);
        }
        // GET: api/Flight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights(int? id = null, string from = "", string to = "", bool expand = false)
        {
            if (!id.HasValue && expand) {
                return BadRequest("expand can only be used if one and only one record is requested via providing an id!");
            }

            using (var context = new FlightContext()) {

                var flights = context.Flights.AsQueryable();

                // flights = flights.Where(f => (f.From == from || from == "") && (f.To == to || to == ""));
                // flights = flights.Where(f => f.From.StartsWith(from) && f.To.StartsWith(to));
                if (expand) {
                    flights = flights.Include(f => f.FlightBookings).ThenInclude(fb => fb.Passenger);
                }

                if (id.HasValue) {
                    flights = flights.Where(p => p.Id == id.Value);
                }

                if (!string.IsNullOrEmpty(from)) {
                    flights = flights.Where(f => f.From.StartsWith(from));
                }

                if (!string.IsNullOrEmpty(to)) {
                    flights = flights.Where(f => f.To.StartsWith(to));
                }

                if (id.HasValue) {
                    return new JsonResult(await flights.FirstOrDefaultAsync());
                }
                else {
                    return await flights.ToListAsync();
                }
                
            }

        }        

        // GET: api/Flight/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(long id, bool expand = false)
        {
            using var context = new FlightContext();

            var query = context.Flights.AsQueryable();

            if (expand) {
                query = query.Include(f => f.FlightBookings).ThenInclude(fb => fb.Passenger);
            }

            var flight = await query.Where(f => f.Id == id).FirstAsync();

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

            if (id < 10 && id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

            if (flight.From.ToLower()  == "Tripsdrill" || flight.To.ToLower() == "Tripsdrill" ) {
                return BadRequest("The Airport 'Tripsdrill' is not supported!");
            }

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
            if (flight.Id < 10 && flight.Id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }
            
            if (flight.From.ToLower() == "Tripsdrill" || flight.To.ToLower() == "Tripsdrill" ) {
                return BadRequest("The Airport 'Tripsdrill' is not supported!");
            }

            if (flight.Id != 0) {
                await PutFlight(flight.Id, flight);
                return AcceptedAtAction("GetFlight", new { id = flight.Id }, flight);
            }

            using var context = new FlightContext();
            context.Flights.Add(flight);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetFlight", new { id = flight.Id }, flight);
        }

        // DELETE: api/Flight/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Flight>> DeleteFlight(long id)
        {
            if (id < 10 && id != 0) {
                return BadRequest("Records with Ids < 10 are reserved for demos and cannot be changed!");
            }

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
