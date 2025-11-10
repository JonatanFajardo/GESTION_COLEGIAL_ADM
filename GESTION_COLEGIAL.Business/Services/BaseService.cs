using AutoMapper;
using GESTION_COLEGIAL.Business.Mappings;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase base para los servicios de negocio.
    /// Proporciona acceso a AutoMapper configurado.
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// Constructor de BaseService.
        /// Configura AutoMapper si no ha sido configurado previamente.
        /// </summary>
        protected BaseService()
        {
            // Asegura que AutoMapper esté configurado
            AutoMapperConfig.Configure();
        }

        /// <summary>
        /// Mapea un objeto de tipo TSource a TDestination usando AutoMapper.
        /// </summary>
        protected TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
