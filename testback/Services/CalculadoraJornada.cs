using System;
using System.Collections.Generic;
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

            var totalHoras = (salida.FechaHoraSalida - ingreso.FechaHoraEntrada).TotalHours;
            reg.HorasTrabajadas = Math.Round(totalHoras, 2);

            double diurnas = 0, nocturnas = 0, extrasDiurnas = 0, extrasNocturnas = 0;
            double acumuladoDiurno = 0;

            DateTime cursor = ingreso.FechaHoraEntrada;
            while (cursor < salida.FechaHoraSalida)
            {
                DateTime next = cursor.AddHours(1);
                if (next > salida.FechaHoraSalida)
                    next = salida.FechaHoraSalida;

                var tramoHoras = (next - cursor).TotalHours;
                var horaActual = cursor.TimeOfDay;

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
                    // En este ejemplo no estamos calculando extras nocturnas por separado
                }

                cursor = next;
            }

            reg.HorasDiurnas = Math.Round(diurnas, 2);
            reg.HorasNocturnas = Math.Round(nocturnas, 2);
            reg.HorasExtrasDiurnas = Math.Round(extrasDiurnas, 2);
            reg.HorasExtrasNocturnas = Math.Round(extrasNocturnas, 2);

            return reg;
        }
    }
}
