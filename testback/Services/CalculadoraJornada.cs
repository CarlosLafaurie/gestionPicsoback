// CalculadoraJornada.cs
using System;
using System.Collections.Generic;
using System.Linq;
using testback.Models;

namespace testback.Services
{
    public class CalculadoraJornada
    {
        /// <summary>
        /// Calcula la jornada de un empleado para una fecha determinada,
        /// procesando todos los ingresos y salidas (primario y adicionales).
        /// </summary>
        public RegistroJornada CalcularRegistro(
            Empleado empleado,
            List<IngresosPersonal> ingresos,
            List<SalidasPersonal> salidas,
            List<DateTime> diasFestivos)
        {
            // Fecha del día (todas las entradas ya comparten la misma Date)
            var fecha = ingresos
                .Select(i => i.FechaHoraEntrada.Date)
                .FirstOrDefault();

            var reg = new RegistroJornada
            {
                NombreCompleto = empleado.NombreCompleto,
                Fecha = fecha,
                TrabajoDomingo = fecha.DayOfWeek == DayOfWeek.Sunday,
                TrabajoFestivo = diasFestivos.Contains(fecha)
            };

            // Ordeno y extraigo los DateTime
            var entradas = ingresos
                .Select(i => i.FechaHoraEntrada)
                .OrderBy(d => d)
                .ToList();
            var salidasOrdered = salidas
                .Select(s => s.FechaHoraSalida)
                .OrderBy(d => d)
                .ToList();

            // Emparejo cada entrada con su salida correspondiente
            var pares = entradas.Zip(salidasOrdered,
                (ent, sal) => (Entrada: ent, Salida: sal))
                .ToList();

            // Definición de descansos y franjas
            var descansos = new List<(TimeSpan inicio, TimeSpan fin)>
            {
                (new TimeSpan(9, 0, 0), new TimeSpan(9, 30, 0)),
                (new TimeSpan(13,0,0), new TimeSpan(14,0,0))
            };
            var inicioDiurno = new TimeSpan(6, 0, 0);
            var finDiurno = new TimeSpan(21, 0, 0);

            double diurnas = 0, nocturnas = 0, extrasDiurnas = 0, extrasNocturnas = 0;

            // Recorro cada tramo de 10 minutos
            foreach (var (entrada, salida) in pares)
            {
                var inicio = entrada;
                var fin = salida <= entrada ? salida.AddDays(1) : salida;
                double acumuladoNormal = 0;
                var cursor = inicio;

                while (cursor < fin)
                {
                    var next = cursor.AddMinutes(10);
                    if (next > fin) next = fin;

                    var t0 = cursor.TimeOfDay;
                    var t1 = next.TimeOfDay;

                    // Si está en descanso, avanzo
                    if (descansos.Any(d => t0 < d.fin && t1 > d.inicio))
                    {
                        cursor = next;
                        continue;
                    }

                    var horas = (next - cursor).TotalHours;
                    bool esDiurno = t0 >= inicioDiurno && t0 < finDiurno;

                    if (acumuladoNormal < 8)
                    {
                        var faltan = 8 - acumuladoNormal;
                        var normales = Math.Min(horas, faltan);
                        var extras = horas - normales;

                        if (esDiurno)
                        {
                            diurnas += normales;
                            extrasDiurnas += extras;
                        }
                        else
                        {
                            nocturnas += normales;
                            extrasNocturnas += extras;
                        }

                        acumuladoNormal += normales;
                    }
                    else
                    {
                        // Todo extra
                        if (esDiurno)
                            extrasDiurnas += horas;
                        else
                            extrasNocturnas += horas;
                    }

                    cursor = next;
                }
            }

            // Asigno totales
            reg.HoraEntrada = entradas.FirstOrDefault();
            reg.HoraSalida = salidasOrdered.LastOrDefault();
            reg.HorasDiurnas = Math.Round(diurnas, 2);
            reg.HorasNocturnas = Math.Round(nocturnas, 2);
            reg.HorasExtrasDiurnas = Math.Round(extrasDiurnas, 2);
            reg.HorasExtrasNocturnas = Math.Round(extrasNocturnas, 2);
            reg.HorasTrabajadas = Math.Round(diurnas + nocturnas + extrasDiurnas + extrasNocturnas, 2);

            return reg;
        }
    }
}
