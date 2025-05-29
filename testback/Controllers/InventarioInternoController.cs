using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioInternoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventarioInternoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los registros
        [HttpGet]
        public async Task<IActionResult> GetInventarioInterno()
        {
            var inventarioInterno = await _context.InventarioInterno
                .Include(i => i.Inventario)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(inventarioInterno);
        }

        // Obtener por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventarioInterno(int id)
        {
            var item = await _context.InventarioInterno
                .Include(i => i.Inventario)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // Obtener por obra
        [HttpGet("obra/{obra}")]
        public async Task<IActionResult> GetPorObra(string obra)
        {
            var items = await _context.InventarioInterno
                .Include(i => i.Inventario)
                .Where(x => x.Obra.ToLower() == obra.ToLower())
                .ToListAsync();

            return Ok(items);
        }

        // Crear nuevo registro
        [HttpPost]
        public async Task<IActionResult> CreateInventarioInterno(InventarioInterno data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.InventarioInterno.Add(data);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventarioInterno), new { id = data.Id }, data);
        }

        // Actualizar registro existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventarioInterno(int id, InventarioInterno data)
        {
            if (id != data.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(data).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.InventarioInterno.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // Eliminar registro
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventarioInterno(int id)
        {
            var item = await _context.InventarioInterno.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.InventarioInterno.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }
    }
}