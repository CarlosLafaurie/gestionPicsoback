using System;
using System.Collections.Generic;
using System.Linq;
using testback.Models;

namespace testback.Services
{
    public class CalculadoraJornada
    {
        public RegistroJornada CalcularRegistro(
            Empleado empleado,
            IngresosPersonal ingreso,
            SalidasPersonal salida,
            List<DateTime> diasFestivos)
        {
            var reg = new RegistroJornada
            {
                NombreCompleto = empleado.NombreCompleto,
                Fecha = ingreso.FechaHoraEntrada.Date,
                HoraEntrada = ingreso.FechaHoraEntrada,
                HoraSalida = salida.FechaHoraSalida,
                TrabajoDomingo = ingreso.FechaHoraEntrada.DayOfWeek == DayOfWeek.Sunday,
                TrabajoFestivo = diasFestivos.Contains(ingreso.FechaHoraEntrada.Date)
            };

            if (salida.FechaHoraSalida <= ingreso.FechaHoraEntrada)
            {
                salida.FechaHoraSalida = salida.FechaHoraSalida.AddDays(1);
            }

            var descansos = new List<(TimeSpan inicio, TimeSpan fin)>
            {
                (new TimeSpan(9, 0, 0), new TimeSpan(9, 30, 0)),
                (new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0))
            };

            var InicioDiurno = new TimeSpan(6, 0, 0);
            var FinDiurno = new TimeSpan(21, 0, 0);

            double diurnas = 0, nocturnas = 0, extrasDiurnas = 0, extrasNocturnas = 0;
            double horasAcumuladas = 0;

            DateTime cursor = ingreso.FechaHoraEntrada;

            while (cursor < salida.FechaHoraSalida)
            {
                DateTime next = cursor.AddMinutes(10);
                if (next > salida.FechaHoraSalida)
                    next = salida.FechaHoraSalida;

                var horaActual = cursor.TimeOfDay;
                var horaSiguiente = next.TimeOfDay;

                bool enDescanso = descansos.Any(d =>
                    horaActual < d.fin && horaSiguiente > d.inicio);

                if (enDescanso)
                {
                    cursor = next;
                    continue;
                }

                var tramoHoras = (next - cursor).TotalHours;

                bool esDiurno = horaActual >= InicioDiurno && horaActual < FinDiurno;

                if (esDiurno)
                {
                    if (horasAcumuladas < 8)
                    {
                        double faltanPara8 = 8 - horasAcumuladas;
                        double normales = Math.Min(tramoHoras, faltanPara8);
                        double extras = tramoHoras - normales;

                        diurnas += normales;
                        extrasDiurnas += extras;
                    }
                    else
                    {
                        extrasDiurnas += tramoHoras;
                    }
                }
                else // nocturno
                {
                    if (horasAcumuladas < 8)
                    {
                        double faltanPara8 = 8 - horasAcumuladas;
                        double normales = Math.Min(tramoHoras, faltanPara8);
                        double extras = tramoHoras - normales;

                        nocturnas += normales;
                        extrasNocturnas += extras;
                    }
                    else
                    {
                        extrasNocturnas += tramoHoras;
                    }
                }

                horasAcumuladas += tramoHoras;
                cursor = next;
            }

            reg.HorasTrabajadas = Math.Round(diurnas + nocturnas + extrasDiurnas + extrasNocturnas, 2);
            reg.HorasDiurnas = Math.Round(diurnas, 2);
            reg.HorasNocturnas = Math.Round(nocturnas, 2);
            reg.HorasExtrasDiurnas = Math.Round(extrasDiurnas, 2);
            reg.HorasExtrasNocturnas = Math.Round(extrasNocturnas, 2);

            return reg;
        }
    }
}
