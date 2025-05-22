using System;
using System.Collections.Generic;
using System.Linq;
using testback.Models;

namespace testback.Services
{
    public class CalculadoraJornada
    {
        private static readonly TimeSpan InicioDiurno = new TimeSpan(6, 0, 0);
        private static readonly TimeSpan FinDiurno = new TimeSpan(21, 0, 0);

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

            // Definimos descansos diarios (aplican todos los días)
            var descansos = new List<(TimeSpan inicio, TimeSpan fin)>
            {
                (new TimeSpan(9, 0, 0), new TimeSpan(9, 30, 0)),   // Desayuno
                (new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0))   // Almuerzo
            };

            double diurnas = 0, nocturnas = 0, extrasDiurnas = 0, extrasNocturnas = 0;
            double acumuladoDiurno = 0;

            DateTime cursor = ingreso.FechaHoraEntrada;

            while (cursor < salida.FechaHoraSalida)
            {
                DateTime next = cursor.AddMinutes(30);
                if (next > salida.FechaHoraSalida)
                    next = salida.FechaHoraSalida;

                var horaActual = cursor.TimeOfDay;
                var horaSiguiente = next.TimeOfDay;

                // Verificamos si este bloque se solapa con algún descanso
                bool enDescanso = descansos.Any(d =>
                    horaActual < d.fin && horaSiguiente > d.inicio);

                if (enDescanso)
                {
                    cursor = next; // Saltar bloque de descanso
                    continue;
                }

                var tramoHoras = (next - cursor).TotalHours;
                bool esDiurno = horaActual >= InicioDiurno && horaActual < FinDiurno;

                if (esDiurno)
                {
                    var faltaPara8 = Math.Max(0, 8 - acumuladoDiurno);
                    var hDiurna = Math.Min(tramoHoras, faltaPara8);
                    var hExtraD = tramoHoras - hDiurna;

                    diurnas += hDiurna;
                    extrasDiurnas += hExtraD;
                    acumuladoDiurno += hDiurna;
                }
                else
                {
                    nocturnas += tramoHoras;
                    // Aquí podrías calcular extras nocturnas si se requiere
                }

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
