using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;
using System.Linq;
using System.Threading.Tasks;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventario()
        {
            if (_context.Inventario == null)
            {
                return NotFound("No hay elementos en el inventario.");
            }

            var inventario = await _context.Inventario
                .Where(i => i.Estado == "Activo")
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return inventario.Any() ? Ok(inventario) : NotFound("No hay elementos activos en el inventario.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _context.Inventario
                .FirstOrDefaultAsync(i => i.Id == id && i.Estado == "Activo");

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(Inventario item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            item.Estado = "Activo";
            _context.Inventario.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditItem(int id, Inventario item)
        {
            if (id != item.Id)
            {
                return BadRequest("El ID no coincide.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var itemExistente = await _context.Inventario.FindAsync(id);
            if (itemExistente == null)
            {
                return NotFound();
            }

            itemExistente.Codigo = item.Codigo;
            itemExistente.Herramienta = item.Herramienta;
            itemExistente.NumeroSerie = item.NumeroSerie;
            itemExistente.Marca = item.Marca;
            itemExistente.FechaUltimoMantenimiento = item.FechaUltimoMantenimiento;
            itemExistente.FechaProximoMantenimiento = item.FechaProximoMantenimiento;
            itemExistente.EmpresaMantenimiento = item.EmpresaMantenimiento;
            itemExistente.Observaciones = item.Observaciones;
            itemExistente.FechaCompra = item.FechaCompra;
            itemExistente.Proveedor = item.Proveedor;
            itemExistente.Garantia = item.Garantia;
            itemExistente.Ubicacion = item.Ubicacion;
            itemExistente.Responsable = item.Responsable;
            itemExistente.Estado = item.Estado;
            itemExistente.Cantidad = item.Cantidad;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Inventario.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Estado = "Inactivo";
            _context.Inventario.Update(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }


        [HttpGet("por-obra/{nombreObra}")]
        public IActionResult ObtenerPorObra(string nombreObra)
        {
            var materiales = _context.Inventario
                .Where(m => m.Ubicacion.Trim().ToLower() == nombreObra.Trim().ToLower())
                .ToList();
            return Ok(materiales);
        }   
    }
}
