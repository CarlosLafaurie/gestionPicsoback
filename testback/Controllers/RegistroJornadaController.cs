// RegistroJornadaController.cs
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
        public async Task<ActionResult<IEnumerable<RegistroJornada>>> ObtenerResumenHoras(
            [FromQuery] bool usarFestivos = false,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var empleados = await _context.Empleado.ToListAsync();
            var ingresosRaw = await _context.IngresosPersonal.ToListAsync();
            var salidasRaw = await _context.SalidasPersonal.ToListAsync();

            // Filtrar por rango de fechas
            if (fechaInicio.HasValue)
            {
                ingresosRaw = ingresosRaw
                    .Where(i => i.FechaHoraEntrada.Date >= fechaInicio.Value.Date)
                    .ToList();
                salidasRaw = salidasRaw
                    .Where(s => s.FechaHoraSalida.Date >= fechaInicio.Value.Date)
                    .ToList();
            }
            if (fechaFin.HasValue)
            {
                ingresosRaw = ingresosRaw
                    .Where(i => i.FechaHoraEntrada.Date <= fechaFin.Value.Date)
                    .ToList();
                salidasRaw = salidasRaw
                    .Where(s => s.FechaHoraSalida.Date <= fechaFin.Value.Date)
                    .ToList();
            }

            // Festivos opcionales
            var festivos = usarFestivos
                ? await _festivoService.ObtenerFestivosColombia(DateTime.Now.Year)
                : new List<DateTime>();

            var resultados = new List<RegistroJornada>();

            // Agrupo ingresos por empleado + fecha
            var grupos = ingresosRaw
                .GroupBy(i => new { i.EmpleadoId, Fecha = i.FechaHoraEntrada.Date });

            foreach (var g in grupos)
            {
                int empleadoId = g.Key.EmpleadoId;
                DateTime fecha = g.Key.Fecha;

                // ** Listas de ingresos y salidas para este empleado+fecha **
                var ingresosDelDia = g
                    .OrderBy(i => i.FechaHoraEntrada)
                    .ToList();

                var salidasDelDia = salidasRaw
                    .Where(s => s.EmpleadoId == empleadoId
                             && s.FechaHoraSalida.Date == fecha)
                    .OrderBy(s => s.FechaHoraSalida)
                    .ToList();

                if (!salidasDelDia.Any())
                    continue;   // sin salidas, no calculamos

                var emp = empleados.First(e => e.Id == empleadoId);

                // Llamada a la calculadora usando listas
                var reg = _calculadora.CalcularRegistro(
                    emp,
                    ingresosDelDia,
                    salidasDelDia,
                    festivos
                );

                resultados.Add(reg);
            }

            return Ok(resultados);
        }
    }
}
