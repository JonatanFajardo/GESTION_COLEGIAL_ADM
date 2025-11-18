namespace Gestion.Colegial.Entities.DTOs
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UserInfoDTO User { get; set; }
    }

    public class UserInfoDTO
    {
        public int Usu_Id { get; set; }
        public string Usu_Name { get; set; }
        public int Emp_Id { get; set; }
        public int Rol_Id { get; set; }
        public string Rol_Nombre { get; set; }
    }
}