using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace autenticacion.Dtos
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AuthenticationDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class ResponseAuthenticationDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }

    }

}
