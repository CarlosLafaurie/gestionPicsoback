using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;

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
            var obras = await _context.Obra.OrderByDescending(x => x.Id).ToListAsync();
            return _context.Obra != null ? Ok(obras) : Problem("Entity set 'ApplicationDbContext.Obra' is null.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetObra(int? id)
        {
            if (id == null || _context.Obra == null)
            {
                return NotFound();
            }

            var obra = await _context.Obra.FirstOrDefaultAsync(m => m.Id == id);
            if (obra == null)
            {
                return NotFound();
            }

            return Ok(obra);
        }

        [HttpPost]
        public async Task<IActionResult> CreateObra([Bind("Id,NombreObra,Responsable,ClienteObra,CostoObra")] Obra obra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obra);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetObra), new { id = obra.Id }, obra);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditObra(int id, [Bind("Id,NombreObra,Responsable,ClienteObra,CostoObra")] Obra obra)
        {
            if (id != obra.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
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
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObra(int id)
        {
            try
            {
                var obra = await _context.Obra.FindAsync(id);

                if (obra != null)
                {
                    _context.Obra.Remove(obra);
                }
                await _context.SaveChangesAsync();
                return Ok(obra);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool ObraExists(int id)
        {
            return (_context.Obra?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

