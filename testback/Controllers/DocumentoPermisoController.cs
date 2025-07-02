using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;
using testback.Models.Dtos;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoPermisoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DocumentoPermisoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("SubirDocumento")]
        public async Task<IActionResult> SubirDocumento([FromForm] SubirDocumentoDto dto)
        {
            if (dto.Archivo == null || dto.Archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo");

            if (dto.FechaInicio > dto.FechaFin)
                return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");

            using var memoryStream = new MemoryStream();
            await dto.Archivo.CopyToAsync(memoryStream);
            var contenidoArchivo = memoryStream.ToArray();

            var documento = new DocumentoPermiso
            {
                NombreEmpleado = dto.NombreEmpleado,
                Comentarios = dto.Comentarios,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Archivo = contenidoArchivo,
                NombreArchivo = dto.Archivo.FileName
            };

            _context.DocumentoPermisos.Add(documento);
            await _context.SaveChangesAsync();
            return Ok(documento);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoPermiso>>> GetTodos()
        {
            return await _context.DocumentoPermisos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoPermiso>> GetPorId(int id)
        {
            var documento = await _context.DocumentoPermisos.FindAsync(id);

            if (documento == null)
                return NotFound();

            return documento;
        }

        [HttpGet("ver/{id}")]
        public async Task<IActionResult> VerArchivo(int id)
        {
            var documento = await _context.DocumentoPermisos.FindAsync(id);

            if (documento == null || documento.Archivo == null)
                return NotFound("Documento no encontrado o sin contenido.");

            return File(documento.Archivo, "application/pdf");
        }

        [HttpGet("descargar/{id}")]
        public async Task<IActionResult> DescargarArchivo(int id)
        {
            var documento = await _context.DocumentoPermisos.FindAsync(id);

            if (documento == null || documento.Archivo == null)
                return NotFound("Documento no encontrado o sin contenido.");

            return File(documento.Archivo, "application/pdf", documento.NombreArchivo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, DocumentoPermiso documento)
        {
            if (id != documento.Id)
                return BadRequest();

            _context.Entry(documento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DocumentoPermisos.Any(d => d.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var documento = await _context.DocumentoPermisos.FindAsync(id);
            if (documento == null)
                return NotFound();

            _context.DocumentoPermisos.Remove(documento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
