#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbUsuarios
    {
        public tbUsuarios()
        {
            tbAulasAul_UsuarioModificaNavigation = new HashSet<tbAulas>();
            tbAulasAul_UsuarioRegistraNavigation = new HashSet<tbAulas>();
            tbCargosCar_UsuarioModificaNavigation = new HashSet<tbCargos>();
            tbCargosCar_UsuarioRegistraNavigation = new HashSet<tbCargos>();
            tbCursosCur_UsuarioModificaNavigation = new HashSet<tbCursos>();
            tbCursosCur_UsuarioRegistraNavigation = new HashSet<tbCursos>();
            tbDiasDia_UsuarioModificaNavigation = new HashSet<tbDias>();
            tbDiasDia_UsuarioRegistraNavigation = new HashSet<tbDias>();
            tbDuracionesDur_UsuarioModificaNavigation = new HashSet<tbDuraciones>();
            tbDuracionesDur_UsuarioRegistraNavigation = new HashSet<tbDuraciones>();
            tbEstadosEst_UsuarioModificaNavigation = new HashSet<tbEstados>();
            tbEstadosEst_UsuarioRegistraNavigation = new HashSet<tbEstados>();
            tbHorarioAlumnosHoAl_UsuarioModificaNavigation = new HashSet<tbHorarioAlumnos>();
            tbHorarioAlumnosHoAl_UsuarioRegistraNavigation = new HashSet<tbHorarioAlumnos>();
            tbMateriasMat_UsuarioModificaNavigation = new HashSet<tbMaterias>();
            tbMateriasMat_UsuarioRegistraNavigation = new HashSet<tbMaterias>();
            tbModalidadesMda_UsuarioModificaNavigation = new HashSet<tbModalidades>();
            tbModalidadesMda_UsuarioRegistraNavigation = new HashSet<tbModalidades>();
            tbNivelesEducativosNiv_UsuarioModificaNavigation = new HashSet<tbNivelesEducativos>();
            tbNivelesEducativosNiv_UsuarioRegistraNavigation = new HashSet<tbNivelesEducativos>();
            tbNotasNot_UsuarioModificaNavigation = new HashSet<tbNotas>();
            tbNotasNot_UsuarioRegistraNavigation = new HashSet<tbNotas>();
            tbParcialesPac_UsuarioModificaNavigation = new HashSet<tbParciales>();
            tbParcialesPac_UsuarioRegistraNavigation = new HashSet<tbParciales>();
            tbParentescosPar_UsuarioModificaNavigation = new HashSet<tbParentescos>();
            tbParentescosPar_UsuarioRegistraNavigation = new HashSet<tbParentescos>();
            tbPersonasPer_UsuarioModificaNavigation = new HashSet<tbPersonas>();
            tbPersonasPer_UsuarioRegistraNavigation = new HashSet<tbPersonas>();
            tbSeccionesSec_UsuarioModificaNavigation = new HashSet<tbSecciones>();
            tbSeccionesSec_UsuarioRegistraNavigation = new HashSet<tbSecciones>();
            tbSemestresSem_UsuarioModificaNavigation = new HashSet<tbSemestres>();
            tbSemestresSem_UsuarioRegistraNavigation = new HashSet<tbSemestres>();
            tbTitulosTit_UsuarioModificaNavigation = new HashSet<tbTitulos>();
            tbTitulosTit_UsuarioRegistraNavigation = new HashSet<tbTitulos>();
        }

        public int Usu_Id { get; set; }
        public int Emp_Id { get; set; }
        public string Usu_Name { get; set; }
        public string Usu_Contraseña { get; set; }
        public int Rol_Id { get; set; }
        public string Usu_Ip { get; set; }
        public bool Usu_EsActivo { get; set; }
        public bool? Usu_Suspendido { get; set; }
        public bool? Usu_EsEliminado { get; set; }
        public DateTime? Usu_FechaCreacion { get; set; }
        public DateTime? Usu_fechaModificacion { get; set; }

        public virtual ICollection<tbAulas> tbAulasAul_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbAulas> tbAulasAul_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbCargos> tbCargosCar_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbCargos> tbCargosCar_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbCursos> tbCursosCur_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbCursos> tbCursosCur_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbDias> tbDiasDia_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbDias> tbDiasDia_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbDuraciones> tbDuracionesDur_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbDuraciones> tbDuracionesDur_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbEstados> tbEstadosEst_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbEstados> tbEstadosEst_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbHorarioAlumnos> tbHorarioAlumnosHoAl_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbHorarioAlumnos> tbHorarioAlumnosHoAl_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbMaterias> tbMateriasMat_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbMaterias> tbMateriasMat_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbModalidades> tbModalidadesMda_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbModalidades> tbModalidadesMda_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbNivelesEducativos> tbNivelesEducativosNiv_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbNivelesEducativos> tbNivelesEducativosNiv_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbNotas> tbNotasNot_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbNotas> tbNotasNot_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbParciales> tbParcialesPac_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbParciales> tbParcialesPac_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbParentescos> tbParentescosPar_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbParentescos> tbParentescosPar_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbPersonas> tbPersonasPer_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbPersonas> tbPersonasPer_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbSecciones> tbSeccionesSec_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbSecciones> tbSeccionesSec_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbSemestres> tbSemestresSem_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbSemestres> tbSemestresSem_UsuarioRegistraNavigation { get; set; }
        public virtual ICollection<tbTitulos> tbTitulosTit_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbTitulos> tbTitulosTit_UsuarioRegistraNavigation { get; set; }
    }
}