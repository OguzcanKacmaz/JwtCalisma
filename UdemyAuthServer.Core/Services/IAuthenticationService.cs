using SharedLibrary.Dtos;
using UdemyAuthServer.Core.DTOs;

namespace UdemyAuthServer.Core.Services;

public interface IAuthenticationService
{
    Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);
    Task<Response<TokenDto>> CreateTokenByRefreshAsync(string refreshToken);
    Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken);
    Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto loginDto);
}
