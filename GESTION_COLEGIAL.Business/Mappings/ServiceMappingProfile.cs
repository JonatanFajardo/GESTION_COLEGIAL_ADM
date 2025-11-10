using AutoMapper;
using GESTION_COLEGIAL.Business.DTOs;
using GESTION_COLEGIAL.Business.Models;

namespace GESTION_COLEGIAL.Business.Mappings
{
    /// <summary>
    /// Maps DTO responses from the API into the view models consumed by the UI layer.
    /// </summary>
    internal class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            ConfigureCargos();
            ConfigureAlumnos();
            ConfigureDropdowns();
        }

        private void ConfigureCargos()
        {
            CreateMap<CargoListDto, CargoViewModel>();
            CreateMap<CargoFindDto, CargoViewModel>();
            CreateMap<CargoDetailDto, CargoViewModel>();
        }

        private void ConfigureAlumnos()
        {
            CreateMap<AlumnoListDto, AlumnoViewModel>();
            CreateMap<AlumnoFindDto, AlumnoViewModel>()
                .ForMember(dest => dest.EsActivoPersona, opt => opt.MapFrom(src => !src.EsEliminadoPersona));
        }

        private void ConfigureDropdowns()
        {
            CreateMap<NivelEducativoDropdownDto, NivelEducativoViewModel>();
            CreateMap<EstadoDropdownDto, EstadoViewModel>();
            CreateMap<CursoNivelDropdownDto, CursoNivelDropViewModel>();
            CreateMap<ModalidadDropdownDto, ModalidadViewModel>();
            CreateMap<CursoDropdownDto, CursoViewModel>();
            CreateMap<SeccionDropdownDto, SeccionViewModel>();
        }
    }
}
