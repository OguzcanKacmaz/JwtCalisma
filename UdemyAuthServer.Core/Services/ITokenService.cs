using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Entities;

namespace UdemyAuthServer.Core.Services;

public interface ITokenService
{
    Task<TokenDto> CreateToken(UserApp user);
    ClientTokenDto CreateTokenByClient(Client client);
}
