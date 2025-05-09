using System;                                 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SolicitudController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSolicitudes()
        {
            var list = await _context.Solicitud.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSolicitud([FromBody] Solicitud solicitud)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            solicitud.FechaSolicitud = DateTime.UtcNow;
            solicitud.Estado = EstadoSolicitud.Pendiente;

            _context.Solicitud.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudes), new { id = solicitud.Id }, solicitud);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] EstadoSolicitud nuevoEstado)
        {
            var sol = await _context.Solicitud.FindAsync(id);
            if (sol == null) return NotFound();

            if (sol.Estado == nuevoEstado)
                return BadRequest("Ya está en ese estado.");

            sol.Estado = nuevoEstado;
            _context.Solicitud.Update(sol);

            if (nuevoEstado == EstadoSolicitud.Aprobada)
            {
                var inv = await _context.Inventario.FindAsync(sol.InventarioId);
                if (inv != null)
                {
                    var mov = new Movimiento
                    {
                        InventarioId = inv.Id,
                        CodigoHerramienta = inv.Codigo,
                        NombreHerramienta = inv.Herramienta,
                        Responsable = sol.Solicitante,
                        Obra = sol.Obra,
                        FechaMovimiento = DateTime.UtcNow,
                        TipoMovimiento = "Salida",
                        Estado = inv.Estado,
                        Comentario = $"Solicitud aprobada #{sol.Id}"
                    };
                    _context.Movimiento.Add(mov);

                    inv.Responsable = sol.Solicitante;
                    inv.Ubicacion = sol.Obra;
                    _context.Inventario.Update(inv);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
