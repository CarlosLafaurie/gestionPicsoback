using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmpleadosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmpleados(int page = 1, int pageSize = 10)
    {
        var empleados = await _context.Empleado
            .Where(e => e.Estado == "Activo")
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(empleados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmpleado(int id)
    {
        var empleado = await _context.Empleado
            .FirstOrDefaultAsync(m => m.Id == id && m.Estado == "Activo");

        if (empleado == null)
        {
            return NotFound();
        }

        return Ok(empleado);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmpleado([Bind("Id,Cedula,NombreCompleto,Cargo,Obra,Responsable,ResponsableSecundario,Salario")] Empleado empleado)
    {
        if (ModelState.IsValid)
        {
            empleado.Estado = "Activo";
            _context.Add(empleado);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
        }
        return BadRequest(ModelState);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditEmpleado(int id, [Bind("Id,Cedula,NombreCompleto,Cargo,Obra,Responsable,ResponsableSecundario,Salario")] Empleado empleado)
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
            return Ok(empleado);
        }
        return BadRequest(ModelState);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmpleado(int id)
    {
        var empleado = await _context.Empleado.FindAsync(id);

        if (empleado == null)
        {
            return NotFound();
        }

        empleado.Estado = "Inactivo";
        _context.Empleado.Update(empleado);
        await _context.SaveChangesAsync();

        return Ok(empleado);
    }

    private bool EmpleadoExists(int id)
    {
        return (_context.Empleado?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
