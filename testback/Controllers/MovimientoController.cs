using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovimientoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovimientos()
        {
            var movimientos = await _context.Movimiento
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            return Ok(movimientos);
        }

        [HttpGet("por-herramienta/{codigo}")]
        public async Task<IActionResult> GetMovimientosPorHerramienta(string codigo)
        {
            var movimientos = await _context.Movimiento
                .Where(m => m.CodigoHerramienta == codigo)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            if (!movimientos.Any())
                return NotFound($"No se encontraron movimientos para la herramienta con código: {codigo}");

            return Ok(movimientos);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarMovimiento(Movimiento movimiento)
        {
            var inventario = await _context.Inventario.FindAsync(movimiento.InventarioId);
            if (inventario == null)
                return NotFound("Herramienta no encontrada.");

            movimiento.CodigoHerramienta = inventario.Codigo;
            movimiento.NombreHerramienta = inventario.Herramienta;
            movimiento.FechaMovimiento = DateTime.UtcNow;

            _context.Movimiento.Add(movimiento);

            // Actualiza inventario
            inventario.Responsable = movimiento.Responsable;
            inventario.Ubicacion = movimiento.Obra;
            inventario.Estado = movimiento.Estado;

            _context.Inventario.Update(inventario);
            await _context.SaveChangesAsync();

            return Ok(movimiento);
        }

        [HttpGet("filtrar")]
        public async Task<IActionResult> FiltrarMovimientos(
            [FromQuery] string? obra,
            [FromQuery] string? responsable,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var query = _context.Movimiento.AsQueryable();

            if (!string.IsNullOrEmpty(obra))
                query = query.Where(m => m.Obra == obra);

            if (!string.IsNullOrEmpty(responsable))
                query = query.Where(m => m.Responsable == responsable);

            if (desde.HasValue)
                query = query.Where(m => m.FechaMovimiento >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(m => m.FechaMovimiento <= hasta.Value);

            var resultados = await query
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            return Ok(resultados);
        }
    }
}
