using autenticacion.Dtos;
using autenticacion.Models;

namespace autenticacion.Interface
{
    public interface IJwtHandle
    {
        TokenSettings GeneratedToken(ResponseAuthenticationDTO user);
    }

}
