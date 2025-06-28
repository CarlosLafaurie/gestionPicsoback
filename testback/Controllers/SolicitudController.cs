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
       
            var zonaColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            solicitud.FechaSolicitud = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaColombia);

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
            try
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
                        if (inv == null) continue;

                        if (item.Cantidad > inv.Cantidad)
                        {
                            return BadRequest($"No hay suficiente stock de {inv.Herramienta}. Disponibles: {inv.Cantidad}, Solicitados: {item.Cantidad}");
                        }

                        // 1. Restar cantidad del origen
                        inv.Cantidad -= item.Cantidad;
                        if (inv.Cantidad <= 0)
                            _context.Inventario.Remove(inv);
                        else
                            _context.Inventario.Update(inv);

                        // 2. Buscar herramienta igual en destino (misma herramienta, código, serie, marca, estado)
                        var existente = await _context.Inventario.FirstOrDefaultAsync(x =>
                            x.Herramienta == inv.Herramienta &&
                            x.Codigo == inv.Codigo &&
                            x.NumeroSerie == inv.NumeroSerie &&
                            x.Marca == inv.Marca &&
                            x.Estado == inv.Estado &&
                            x.Ubicacion == sol.Obra);

                        if (existente != null)
                        {
                            existente.Cantidad += item.Cantidad;
                            _context.Inventario.Update(existente);
                        }
                        else
                        {
                            var nuevoInventario = new Inventario
                            {
                                Codigo = inv.Codigo,
                                Herramienta = inv.Herramienta,
                                NumeroSerie = inv.NumeroSerie,
                                FechaUltimoMantenimiento = inv.FechaUltimoMantenimiento,
                                FechaProximoMantenimiento = inv.FechaProximoMantenimiento,
                                EmpresaMantenimiento = inv.EmpresaMantenimiento,
                                Observaciones = inv.Observaciones,
                                Ubicacion = sol.Obra,
                                Responsable = sol.Solicitante,
                                Estado = inv.Estado,
                                Marca = inv.Marca,
                                Proveedor = inv.Proveedor,
                                Garantia = inv.Garantia,
                                FechaCompra = inv.FechaCompra,
                                Cantidad = item.Cantidad
                            };
                            _context.Inventario.Add(nuevoInventario);
                        }

                        // 3. Registrar movimiento
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
                            Comentario = $"Solicitud aprobada #{sol.Id}",
                            Cantidad = item.Cantidad
                        };

                        _context.Movimiento.Add(mov);
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message} {(ex.InnerException?.Message ?? "")}");
            }
        }

    }
}
