using autenticacion.Database;
using autenticacion.Dtos;
using autenticacion.Interface;
using autenticacion.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace autenticacion.Services
{
    public class AuthenticationService
    {
        private readonly DBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtHandle _jwtHandle;


        public AuthenticationService(DBContext context, IConfiguration configuration, IJwtHandle jwtHandle)
        {
            _context = context;
            _configuration = configuration;
            _jwtHandle = jwtHandle;
        }

        public async Task<List<UserDTO>> GetAllAsync()
        { 
            var data = new List<UserDTO>();
            var user = await _context.Users.Where(x => x.Status == true).ToListAsync();
            foreach (var item in user)
            {
                data.Add(new UserDTO
                {
                    UserCode = item.UserCode,
                    UserName = item.UserName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    Role = item.Role

                });
            }
            return data;
        }

        public async Task<UserDTO> GetByCodeAsync(string codice)
        {
            UserDTO data = new UserDTO();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserCode == codice && x.Status == true);

            if (user != null) { 
                data.UserCode = user.UserCode;
                data.UserName = user.UserName;
                data.FirstName = user.FirstName;
                data.LastName = user.LastName;
                data.Email = user.Email;
                data.Role = user.Role;
            }
            else
            {
                return null;
            }

            return data;
        }

        public async Task<UserDTO> CreateAsync(User model)
        {
            model.Status = true;
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            model.Password = HashPassword(model.Password);

            var createOrder = _context.Users.Add(model);
            await _context.SaveChangesAsync();
            var id = createOrder.Entity.UserId;

            var result = await GetByCodeAsync(model.UserCode);

            return result;
        }

        public async Task<string> GenerateNextUserCodeAsync()
        {
            var quantity = await _context.Users.CountAsync();
            var nextCode = $"USER_{quantity + 1}";
            return nextCode;
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashedInput = HashPassword(inputPassword);
            return hashedInput == storedHash;
        }

        public async Task<ResponseAuthenticationDTO> LoginAsync(string username)
        {
            ResponseAuthenticationDTO authenticationDTO = new ResponseAuthenticationDTO();

            var user = await _context.Users.Where(x => x.UserName == username && x.Status == true).FirstOrDefaultAsync();
            UserDTO userDTO = new UserDTO();

            userDTO.UserCode = user.UserCode;
            userDTO.UserName = user.UserName;
            userDTO.FirstName = user.FirstName;
            userDTO.LastName = user.LastName;
            userDTO.Email = user.Email;
            userDTO.Role = user.Role;
            userDTO.Status = user.Status;
            userDTO.CreatedAt = user.CreatedAt;
            userDTO.UpdatedAt = user.UpdatedAt;

            authenticationDTO.User = userDTO;
            //authenticationDTO.Token = GenerateJwtToken(user.UserName);
            var token = _jwtHandle.GeneratedToken(authenticationDTO);
            authenticationDTO.Token = token.AccessToken;

            return authenticationDTO;
        }

        public async Task<bool> VerifyUser(AuthenticationDTO model)
        {
            var exist = await _context.Users.Where(x => x.UserName == model.UserName && x.Status == true).FirstOrDefaultAsync();
            if (exist != null) {
                var password = VerifyPassword(model.Password, exist.Password);
                if(password == true)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
