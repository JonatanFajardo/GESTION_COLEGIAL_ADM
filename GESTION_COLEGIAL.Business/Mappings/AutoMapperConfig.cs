using AutoMapper;

namespace GESTION_COLEGIAL.Business.Mappings
{
    /// <summary>
    /// Configura los mapeos de AutoMapper para los servicios de negocio.
    /// Esta clase utiliza la API estática de AutoMapper 3.3.1.
    /// </summary>
    internal static class AutoMapperConfig
    {
        private static bool _isConfigured = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Configura todos los mapeos de AutoMapper.
        /// Solo se ejecuta una vez durante la vida de la aplicación.
        /// </summary>
        public static void Configure()
        {
            if (_isConfigured)
                return;

            lock (_lock)
            {
                if (_isConfigured)
                    return;

                ConfigureAlumnos();

                _isConfigured = true;
            }
        }

        private static void ConfigureAlumnos()
        {
            // Mapeos principales de Alumnos
            Mapper.CreateMap<DTOs.AlumnoListDto, Models.AlumnoViewModel>();
            Mapper.CreateMap<DTOs.AlumnoFindDto, Models.AlumnoViewModel>()
                .ForMember(dest => dest.EsActivoPersona, opt => opt.MapFrom(src => !src.EsEliminadoPersona));

            // Mapeos de Dropdowns necesarios para Alumnos
            Mapper.CreateMap<DTOs.NivelEducativoDropdownDto, Models.NivelEducativoViewModel>();
            Mapper.CreateMap<DTOs.EstadoDropdownDto, Models.EstadoViewModel>();
            Mapper.CreateMap<DTOs.CursoNivelDropdownDto, Models.CursoNivelDropViewModel>();
            Mapper.CreateMap<DTOs.ModalidadDropdownDto, Models.ModalidadViewModel>();
            Mapper.CreateMap<DTOs.CursoDropdownDto, Models.CursoViewModel>();
            Mapper.CreateMap<DTOs.SeccionDropdownDto, Models.SeccionViewModel>();
        }
    }
}