using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalidasPersonalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalidasPersonalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalidas()
        {
            var salidas = await _context.SalidasPersonal.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(salidas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalida(int id)
        {
            var salida = await _context.SalidasPersonal.FindAsync(id);
            if (salida == null)
            {
                return NotFound();
            }
            return Ok(salida);
        }

        [HttpGet("ultimo/{empleadoId}")]
        public async Task<IActionResult> GetUltimaSalidaPorEmpleado(int empleadoId)
        {
            var ultimo = await _context.SalidasPersonal
                .Where(s => s.EmpleadoId == empleadoId)
                .OrderByDescending(s => s.FechaHoraSalida)
                .FirstOrDefaultAsync();

            if (ultimo == null)
                return NotFound($"No se encontró salida para el empleado con ID {empleadoId}.");

            return Ok(ultimo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalida(SalidasPersonal salida)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool fechaDuplicada = await _context.SalidasPersonal
                .AnyAsync(s => s.EmpleadoId == salida.EmpleadoId &&
                               s.FechaHoraSalida.Date == salida.FechaHoraSalida.Date);

            if (fechaDuplicada)
            {
                return Conflict("Ya existe una salida registrada para este empleado en esa fecha.");
            }

            _context.SalidasPersonal.Add(salida);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSalida), new { id = salida.Id }, salida);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalida(int id, SalidasPersonal salida)
        {
            if (id != salida.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(salida).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.SalidasPersonal.Any(x => x.Id == id))
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
        public async Task<IActionResult> DeleteSalida(int id)
        {
            var salida = await _context.SalidasPersonal.FindAsync(id);
            if (salida == null)
            {
                return NotFound();
            }
            _context.SalidasPersonal.Remove(salida);
            await _context.SaveChangesAsync();
            return Ok(salida);
        }
    }
}
