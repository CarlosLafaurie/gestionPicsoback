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
    public class ObrasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ObrasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetObras()
        {
            if (_context.Obra == null)
            {
                return NotFound("No hay obras registradas.");
            }

            var obras = await _context.Obra
                .Where(o => o.Estado == "Activo")
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return obras.Any() ? Ok(obras) : NotFound("No hay obras activas.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetObra(int id)
        {
            var obra = await _context.Obra
                .FirstOrDefaultAsync(m => m.Id == id && m.Estado == "Activo");

            if (obra == null)
            {
                return NotFound();
            }

            return Ok(obra);
        }

        [HttpPost]
        public async Task<IActionResult> CreateObra(Obra obra)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            obra.Estado = "Activo";
            _context.Add(obra);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetObra), new { id = obra.Id }, obra);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditObra(int id, Obra obra)
        {
            if (id != obra.Id)
            {
                return BadRequest("El ID no coincide.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(obra);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObraExists(obra.Id))
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
        public async Task<IActionResult> DeleteObra(int id)
        {
            var obra = await _context.Obra.FindAsync(id);
            if (obra == null)
            {
                return NotFound();
            }

            obra.Estado = "Inactivo";
            _context.Obra.Update(obra);
            await _context.SaveChangesAsync();

            return Ok(obra);
        }

        private bool ObraExists(int id)
        {
            return (_context.Obra?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
