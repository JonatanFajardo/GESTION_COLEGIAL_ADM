using System;

namespace GESTION_COLEGIAL.Business.Helpers
{
    /// <summary>
    /// Fecha y hora de Honduras (UTC-6, sin horario de verano). Los contenedores corren en UTC:
    /// usar esto en vez de DateTime.Now cuando importa qué día es "hoy" en el colegio.
    /// </summary>
    public static class HoraHonduras
    {
        private static readonly TimeZoneInfo Zona = BuscarZona();

        private static TimeZoneInfo BuscarZona()
        {
            foreach (string id in new[] { "America/Tegucigalpa", "Central America Standard Time" })
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch (Exception) { /* sigue con la siguiente */ }
            }
            return TimeZoneInfo.CreateCustomTimeZone("Honduras", TimeSpan.FromHours(-6), "Honduras", "Honduras");
        }

        public static DateTime Ahora => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zona);

        public static DateTime Hoy => Ahora.Date;
    }
}
