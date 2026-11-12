namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>
    /// Qué hay en Configuración: los hubs de catálogos y parámetros, agrupados por dominio.
    /// Una sola fuente para la página-índice (/configuracion) y para el buscador "¿Qué querés hacer?".
    /// Para agregar un hub nuevo, sumar aquí su tarjeta.
    /// </summary>
    public static class CatalogoConfiguracion
    {
        /// <summary>
        /// Grupos con solo las tarjetas que el usuario puede abrir (los grupos vacíos se omiten).
        /// </summary>
        public static List<GrupoConfiguracion> Visibles(bool esAdmin, Func<string, bool> tienePantalla)
        {
            return Todos()
                .Select(g =>
                {
                    g.Tarjetas = g.Tarjetas.Where(t => esAdmin || tienePantalla(t.Pantalla)).ToList();
                    return g;
                })
                .Where(g => g.Tarjetas.Count > 0)
                .ToList();
        }

        public static List<GrupoConfiguracion> Todos() => new List<GrupoConfiguracion>
        {
            new GrupoConfiguracion
            {
                Id = "estructura",
                Titulo = "Estructura académica",
                Descripcion = "La base sobre la que se arman los cursos, las secciones y la matrícula.",
                Tarjetas =
                {
                    new TarjetaConfiguracion
                    {
                        Titulo = "Estructura académica",
                        Descripcion = "Las piezas base de la organización académica y del calendario de evaluación.",
                        Icono = "bi-buildings",
                        Controller = "CatalogosInstitucion",
                        Pantalla = "Estructura Académica",
                        Contenido = new[] { "Modalidades", "Niveles educativos", "Secciones", "Semestres", "Parciales" }
                    },
                    new TarjetaConfiguracion
                    {
                        Titulo = "Programa académico",
                        Descripcion = "Qué se enseña: carreras y ciclos, sus grados y sus materias.",
                        Icono = "bi-journal-bookmark-fill",
                        Controller = "ProgramaAcademico",
                        Pantalla = "Programa Académico",
                        Contenido = new[] { "Cursos (BTP y ciclos)", "Grados académicos", "Materias", "Duraciones" }
                    }
                }
            },
            new GrupoConfiguracion
            {
                Id = "cobranza",
                Titulo = "Cobranza",
                Descripcion = "Qué se cobra, cuánto y cómo se registra el pago.",
                Tarjetas =
                {
                    new TarjetaConfiguracion
                    {
                        Titulo = "Configuración de cobranza",
                        Descripcion = "Conceptos y tarifas que generan los cargos, y las reglas de cada pago.",
                        Icono = "bi-cash-coin",
                        Controller = "ConfiguracionCobranza",
                        Pantalla = "Configuración de Cobranza",
                        Contenido = new[] { "Conceptos de pago", "Tarifas", "Descuentos", "Formas de pago", "Estados de pago" }
                    }
                }
            },
            new GrupoConfiguracion
            {
                Id = "recursos",
                Titulo = "Recursos",
                Descripcion = "Espacios y tiempos con los que se arman los horarios.",
                Tarjetas =
                {
                    new TarjetaConfiguracion
                    {
                        Titulo = "Recursos de horario",
                        Descripcion = "Las aulas disponibles y los días y horas de clase.",
                        Icono = "bi-calendar-week",
                        Controller = "RecursosHorario",
                        Pantalla = "Recursos de Horario",
                        Contenido = new[] { "Aulas", "Días", "Horas" }
                    }
                }
            },
            new GrupoConfiguracion
            {
                Id = "sistema",
                Titulo = "Sistema",
                Descripcion = "Valores generales que usan otras pantallas del sistema.",
                Tarjetas =
                {
                    new TarjetaConfiguracion
                    {
                        Titulo = "Parámetros del sistema",
                        Descripcion = "Datos de la institución y valores académicos y financieros por defecto.",
                        Icono = "bi-sliders",
                        Controller = "ConfiguracionSistema",
                        Pantalla = "Configuración del Sistema",
                        Contenido = new[] { "Institución", "Año lectivo activo", "Nota mínima", "Cupo por aula", "Moneda y mora" }
                    }
                }
            },
            new GrupoConfiguracion
            {
                Id = "apoyo",
                Titulo = "Listas de apoyo",
                Descripcion = "Listas cortas que casi nunca cambian.",
                Plegado = true,
                Tarjetas =
                {
                    new TarjetaConfiguracion
                    {
                        Titulo = "Catálogos comunes",
                        Descripcion = "Opciones que aparecen en formularios de alumnos, encargados y empleados.",
                        Icono = "bi-collection",
                        Controller = "CatalogosComunes",
                        Pantalla = "Catálogos Comunes",
                        Contenido = new[] { "Estados", "Parentescos", "Títulos", "Cargos" }
                    }
                }
            }
        };
    }
}
