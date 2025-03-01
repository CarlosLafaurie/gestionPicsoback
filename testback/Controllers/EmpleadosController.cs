using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmpleadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Empleados
        [HttpGet]
        public async Task<IActionResult> GetEmpleados()
        {
            var empleados = await _context.Empleado.OrderByDescending(x => x.Id).ToListAsync();
            return _context.Empleado != null ? Ok(empleados) : Problem("Entity set 'ApplicationDbContext.Empleado' is null.");
        }

        // GET: api/Empleados/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmpleado(int? id)
        {
            if (id == null || _context.Empleado == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleado.FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return Ok(empleado);
        }

        // POST: api/Empleados
        [HttpPost]
        public async Task<IActionResult> CreateEmpleado([Bind("Id,Cedula,NombreCompleto,Cargo,Obra,Responsable,FechaHoraEntrada,FechaHoraSalida,Comentarios,PermisosEspeciales")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/Empleados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditEmpleado(int id, [Bind("Id,Cedula,NombreCompleto,Cargo,Obra,Responsable,FechaHoraEntrada,FechaHoraSalida,Comentarios,PermisosEspeciales")] Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
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

        // DELETE: api/Empleados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            try
            {
                var empleado = await _context.Empleado.FindAsync(id);

                if (empleado != null)
                {
                    _context.Empleado.Remove(empleado);
                }
                await _context.SaveChangesAsync();
                return Ok(empleado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool EmpleadoExists(int id)
        {
            return (_context.Empleado?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
