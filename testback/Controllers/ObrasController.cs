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
            var obras = await _context.Obra
                .Where(o => o.Estado == "Activo")
                .OrderByDescending(o => o.Id)
                .ToListAsync();  // quitamos AsNoTracking para evitar problemas

            // Cargar todos los usuarios en memoria una sola vez
            var usuarios = await _context.Usuario
                .Select(u => new { u.Id, u.NombreCompleto })
                .ToListAsync();

            // Armar DTO con nombre del responsable desde los datos cargados
            var dto = obras.Select(o => new
            {
                o.Id,
                o.NombreObra,
                o.ClienteObra,
                o.CostoObra,
                o.Estado,
                o.Ubicacion,
                o.ResponsableId,
                ResponsableNombre = usuarios.FirstOrDefault(u => u.Id == o.ResponsableId)?.NombreCompleto,
                o.ResponsableSecundario
            });

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetObra(int id)
        {
            var o = await _context.Obra
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Estado == "Activo");

            if (o == null) return NotFound();

            var nombreResp = await _context.Usuario
                .Where(u => u.Id == o.ResponsableId)
                .Select(u => u.NombreCompleto)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                o.Id,
                o.NombreObra,
                o.ClienteObra,
                o.CostoObra,
                o.Estado,
                o.Ubicacion,
                o.ResponsableId,
                ResponsableNombre = nombreResp,
                o.ResponsableSecundario
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateObra([FromBody] Obra o)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(o.Ubicacion))
                return BadRequest("Ubicación requerida.");

            if (o.ResponsableId.HasValue)
            {
                bool existe = await _context.Usuario.AnyAsync(u => u.Id == o.ResponsableId);
                if (!existe) return BadRequest("Responsable no existe.");
            }

            o.Estado = "Activo";
            _context.Obra.Add(o);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetObra), new { id = o.Id }, new { o.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditObra(int id, [FromBody] Obra o)
        {
            if (id != o.Id) return BadRequest("ID no coincide.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (o.ResponsableId.HasValue)
            {
                bool existe = await _context.Usuario.AnyAsync(u => u.Id == o.ResponsableId);
                if (!existe) return BadRequest("Responsable no existe.");
            }

            var orig = await _context.Obra.FindAsync(id);
            if (orig == null) return NotFound();

            var nombreObraAnterior = orig.NombreObra;

            orig.NombreObra = o.NombreObra;
            orig.ClienteObra = o.ClienteObra;
            orig.CostoObra = o.CostoObra;
            orig.Estado = o.Estado;
            orig.Ubicacion = o.Ubicacion;
            orig.ResponsableId = o.ResponsableId;
            orig.ResponsableSecundario = o.ResponsableSecundario;

            await _context.SaveChangesAsync();

            string? nombreResponsable = null;
            if (o.ResponsableId.HasValue)
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Id == o.ResponsableId.Value);
                if (usuario != null)
                    nombreResponsable = usuario.NombreCompleto;
            }

            var empleadosRelacionados = await _context.Empleado
                .Where(e => e.Obra == o.NombreObra)
                .ToListAsync();

            foreach (var empleado in empleadosRelacionados)
            {
                empleado.Responsable = nombreResponsable ?? "Sin responsable";
                empleado.ResponsableSecundario = o.ResponsableSecundario ?? "Sin responsable";
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObra(int id)
        {
            var o = await _context.Obra.FindAsync(id);
            if (o == null) return NotFound();

            o.Estado = "Inactivo";
            await _context.SaveChangesAsync();
            return Ok(new { o.Id });
        }
    }
}
