using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hospital.API.DTOs
{
    public class HistorialClinicoResponse
    {
        public int IdHistorial { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }

        [JsonConverter(typeof(LocalTimeConverter))]
        public DateTime FechaRegistro { get; set; }

        public string Diagnostico { get; set; }
        public string Observaciones { get; set; }
        public string Tratamiento { get; set; }
        public bool SeguimientoRequerido { get; set; }
        
        // Patient information
        public string NombrePaciente { get; set; }
        public string ApellidoPaciente { get; set; }
        public string Documento { get; set; }
    }

    public class LocalTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetDateTime();
            // If the date has Z (UTC), convert to local time
            if (date.Kind == DateTimeKind.Utc)
            {
                return date.ToLocalTime();
            }
            return date;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
        }
    }
} 