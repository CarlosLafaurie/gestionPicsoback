using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngresosPersonalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IngresosPersonalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetIngresos()
        {
            var ingresos = await _context.IngresosPersonal.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(ingresos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngreso(int id)
        {
            var ingreso = await _context.IngresosPersonal.FindAsync(id);
            if (ingreso == null)
            {
                return NotFound();
            }
            return Ok(ingreso);
        }

        [HttpPost]
        public async Task<IActionResult> CreateIngreso(IngresosPersonal ingreso)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool fechaDuplicada = await _context.IngresosPersonal
                .AnyAsync(i => i.EmpleadoId == ingreso.EmpleadoId &&
                               i.FechaHoraEntrada.Date == ingreso.FechaHoraEntrada.Date);

            if (fechaDuplicada)
            {
                return Conflict("Ya existe un ingreso registrado para este empleado en esa fecha.");
            }

            _context.IngresosPersonal.Add(ingreso);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIngreso), new { id = ingreso.Id }, ingreso);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngreso(int id, IngresosPersonal ingreso)
        {
            if (id != ingreso.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(ingreso).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.IngresosPersonal.Any(x => x.Id == id))
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
        public async Task<IActionResult> DeleteIngreso(int id)
        {
            var ingreso = await _context.IngresosPersonal.FindAsync(id);
            if (ingreso == null)
            {
                return NotFound();
            }
            _context.IngresosPersonal.Remove(ingreso);
            await _context.SaveChangesAsync();
            return Ok(ingreso);
        }
    }
}
