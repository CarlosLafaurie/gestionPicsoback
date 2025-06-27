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
            var list = await _context.Solicitud
                .Include(s => s.Items).ThenInclude(i => i.Inventario).ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
            public async Task<IActionResult> GetSolicitudById(int id)
            {
                var solicitud = await _context.Solicitud
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (solicitud == null)
                    return NotFound();

                return Ok(solicitud);
            }
        [HttpPost]
        public async Task<IActionResult> CreateSolicitud([FromBody] Solicitud solicitud)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            solicitud.FechaSolicitud = DateTime.UtcNow;
            solicitud.Estado = EstadoSolicitud.Pendiente;

            foreach (var item in solicitud.Items)
            {
                _context.Entry(item).State = EntityState.Added;
            }

            _context.Solicitud.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudes), new { id = solicitud.Id }, solicitud);
        }




        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] EstadoSolicitud nuevoEstado)
        {
            var sol = await _context.Solicitud
                .Include(s => s.Items)
                .ThenInclude(i => i.Inventario)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sol == null)
                return NotFound();

            if (sol.Estado == nuevoEstado)
                return BadRequest("Ya está en ese estado.");

            sol.Estado = nuevoEstado;
            _context.Solicitud.Update(sol);

            if (nuevoEstado == EstadoSolicitud.Aprobada)
            {
                foreach (var item in sol.Items)
                {
                    var inv = item.Inventario;

                    if (inv == null)
                    {
                        // No se puede hacer movimiento si no hay inventario
                        // Puedes registrar un log si lo deseas
                        continue;
                    }

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
