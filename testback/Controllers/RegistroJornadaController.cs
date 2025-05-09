using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testback.Data;
using testback.Models;
using testback.Services;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroJornadaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CalculadoraJornada _calculadora;
        private readonly FestivoApiService _festivoService;

        public RegistroJornadaController(
            ApplicationDbContext context,
            CalculadoraJornada calculadora,
            FestivoApiService festivoService)
        {
            _context = context;
            _calculadora = calculadora;
            _festivoService = festivoService;
        }

        [HttpGet("resumenhoras")]
        public async Task<ActionResult<IEnumerable<RegistroJornada>>> ObtenerResumenHoras([FromQuery] bool usarFestivos = false)
        {
            var empleados = await _context.Empleado.ToListAsync();
            var ingresos = await _context.IngresosPersonal.ToListAsync();
            var salidas = await _context.SalidasPersonal.ToListAsync();

            var festivos = usarFestivos
                ? await _festivoService.ObtenerFestivosColombia(DateTime.Now.Year)
                : new List<DateTime>();

            var resultados = new List<RegistroJornada>();
            var grupos = ingresos.GroupBy(i => new { i.EmpleadoId, Fecha = i.FechaHoraEntrada.Date });

            foreach (var g in grupos)
            {
                var ingreso = g.OrderBy(i => i.FechaHoraEntrada).First();
                var fecha = ingreso.FechaHoraEntrada.Date;
                var salida = salidas
                              .Where(s => s.EmpleadoId == ingreso.EmpleadoId
                                        && s.FechaHoraSalida.Date == fecha)
                              .OrderByDescending(s => s.FechaHoraSalida)
                              .FirstOrDefault();
                if (salida == null) continue;

                var emp = empleados.First(e => e.Id == ingreso.EmpleadoId);
                var reg = _calculadora.CalcularRegistro(emp, ingreso, salida, festivos);
                resultados.Add(reg);
            }

            return Ok(resultados);
        }
    }
}
