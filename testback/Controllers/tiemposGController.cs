using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;
using System.Threading.Tasks;
using System.Linq;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class tiemposgController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public tiemposgController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Gettiemposg()
        {
            var tiemposg = await _context.tiemposg.OrderByDescending(x => x.Id).ToListAsync();
            return _context.tiemposg != null ? Ok(tiemposg) : Problem("Entity set 'ApplicationDbcontext.tiemposg' is null.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTiempoG(int id)
        {
            var tiempoG = await _context.tiemposg.FindAsync(id);
            if (tiempoG == null)
            {
                return NotFound();
            }
            return Ok(tiempoG);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTiempoG(tiemposg tiempoG)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Add(tiempoG);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTiempoG), new { id = tiempoG.Id }, tiempoG);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTiempoG(int id, tiemposg tiempoG)
        {
            if (id != tiempoG.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(tiempoG);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TiempoGExists(tiempoG.Id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTiempoG(int id)
        {
            var tiempoG = await _context.tiemposg.FindAsync(id);
            if (tiempoG == null)
            {
                return NotFound();
            }

            _context.tiemposg.Remove(tiempoG);
            await _context.SaveChangesAsync();
            return Ok(tiempoG);
        }

        private bool TiempoGExists(int id)
        {
            return _context.tiemposg.Any(e => e.Id == id);
        }
    }
}
