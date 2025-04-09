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
        private readonly string _rutaBase = @"C:\Users\CAMILO TAMAYO\Desktop\Desarrollo\gestion de tiempos\back\Docspermisos";

        public DocumentoPermisoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("SubirDocumento")]
        public async Task<IActionResult> SubirDocumento([FromForm] SubirDocumentoDto dto)
        {
            if (dto.Archivo == null || dto.Archivo.Length == 0)
                return BadRequest("No se recibió ningún archivo");

            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(dto.Archivo.FileName);
            var rutaCompleta = Path.Combine(_rutaBase, nombreArchivo);

            if (!Directory.Exists(_rutaBase))
            {
                Directory.CreateDirectory(_rutaBase);
            }

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await dto.Archivo.CopyToAsync(stream);
            }

            var documento = new DocumentoPermiso
            {
                NombreEmpleado = dto.NombreEmpleado,
                Comentarios = dto.Comentarios,
                FechaHoraEntrada = dto.FechaHoraEntrada,
                RutaDocumento = rutaCompleta
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

            if (System.IO.File.Exists(documento.RutaDocumento))
                System.IO.File.Delete(documento.RutaDocumento);

            _context.DocumentoPermisos.Remove(documento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly string _pdfFolderPath;

        public PdfController(IWebHostEnvironment env)
        {
            _pdfFolderPath = @"C:\Users\CAMILO TAMAYO\Desktop\Desarrollo\gestion de tiempos\back\Docspermisos";
        }

        [HttpGet("ver/{fileName}")]
        public IActionResult VerPdf(string fileName)
        {
            var filePath = Path.Combine(_pdfFolderPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"Archivo no encontrado en: {filePath}");
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/pdf");
        }

        [HttpGet("descargar/{fileName}")]
        public IActionResult DescargarPdf(string fileName)
        {
            var filePath = Path.Combine(_pdfFolderPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("El archivo PDF no existe.");

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/pdf", fileName);
        }
    }
}
