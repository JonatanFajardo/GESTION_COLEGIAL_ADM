using Newtonsoft.Json;
using System;

namespace GESTION_COLEGIAL.Business.Models.Graduacion
{
    /// <summary>
    /// DTO compacto usado únicamente para leer el archivo App_Data/graduados-historicos.json.
    /// Las claves se acortan al máximo para reducir peso del archivo; el service lo mapea
    /// a AlumnoGraduadoViewModel para no cambiar la API ni el JS del cliente.
    /// </summary>
    public class AlumnoGraduadoCompactDto
    {
        [JsonProperty("id")]  public int Id { get; set; }
        [JsonProperty("dni")] public string Dni { get; set; }
        [JsonProperty("n")]   public string Nombre { get; set; }
        [JsonProperty("fn")]  public DateTime? FechaNac { get; set; }
        [JsonProperty("s")]   public string Sexo { get; set; }
        [JsonProperty("e")]   public string Correo { get; set; }
        [JsonProperty("t")]   public string Telefono { get; set; }
        [JsonProperty("nv")]  public string Nivel { get; set; }
        [JsonProperty("tt")]  public string Titulo { get; set; }
        [JsonProperty("ai")]  public int? AnioIngreso { get; set; }
        [JsonProperty("ag")]  public int AnioGrad { get; set; }
        [JsonProperty("tp")]  public decimal TotalPagado { get; set; }
        [JsonProperty("en")]  public string EncargadosJson { get; set; }
        [JsonProperty("oid")] public int? AluIdOriginal { get; set; }
        [JsonProperty("fa")]  public DateTime FechaArchivo { get; set; }
        [JsonProperty("ua")]  public int UsuarioArchivo { get; set; }

        public AlumnoGraduadoViewModel ToViewModel()
        {
            return new AlumnoGraduadoViewModel
            {
                AluGrad_Id = Id,
                AluGrad_Identidad = Dni,
                AluGrad_NombreCompleto = Nombre,
                AluGrad_FechaNacimiento = FechaNac,
                AluGrad_Sexo = Sexo,
                AluGrad_Correo = Correo,
                AluGrad_Telefono = Telefono,
                AluGrad_NivelEducativo = Nivel,
                AluGrad_TituloObtenido = Titulo,
                AluGrad_AnioIngreso = AnioIngreso,
                AluGrad_AnioGraduacion = AnioGrad,
                AluGrad_TotalPagado = TotalPagado,
                AluGrad_EncargadosJson = EncargadosJson,
                AluGrad_AluIdOriginal = AluIdOriginal,
                AluGrad_FechaArchivo = FechaArchivo,
                AluGrad_UsuarioArchivo = UsuarioArchivo,
            };
        }
    }
}
