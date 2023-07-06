using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Entities;
using UdemyAuthServer.Core.Services;

namespace UdemyAuthServer.Service.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<UserApp> _userManager;
    private readonly CustomTokenOption _customTokenOption;

    public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> customTokenOption)
    {
        _userManager = userManager;
        _customTokenOption = customTokenOption.Value;
    }
    private string CreateRefreshToken()
    {
        var numberByte = new Byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(numberByte);
        return Convert.ToBase64String(numberByte);
    }
    private async Task<IEnumerable<Claim>> GetClaims(UserApp userApp, List<string> audiences)
    {
        var userRoles = await _userManager.GetRolesAsync(userApp);
        var userList = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,userApp.Id),
            new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
            new Claim(ClaimTypes.Name,userApp.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };
        userList.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));
        userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
        return userList;
    }
    private IEnumerable<Claim> GetClaimsByClient(Client client)
    {
        var claims = new List<Claim>()
        {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, client.Id)
        };

        claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
        return claims;
    }
    public async Task<TokenDto> CreateToken(UserApp user)
    {
        var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
        var refreshTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.RefreshTokenExpiration);
        var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);
        JwtSecurityToken jwtSecurityToken = new(issuer: _customTokenOption.Issuer, expires: accessTokenExpiration, notBefore: DateTime.Now, claims:await GetClaims(user, _customTokenOption.Audience), signingCredentials: signingCredentials);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwtSecurityToken);
        var tokenDto = new TokenDto()
        {
            AccessToken = token,
            RefreshToken = CreateRefreshToken(),
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        };
        return tokenDto;
    }

    public ClientTokenDto CreateTokenByClient(Client client)
    {
        var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
        var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);
        JwtSecurityToken jwtSecurityToken = new(issuer: _customTokenOption.Issuer, expires: accessTokenExpiration, notBefore: DateTime.Now, claims: GetClaimsByClient(client), signingCredentials: signingCredentials);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(jwtSecurityToken);

        var tokenDto = new ClientTokenDto()
        {
            AccessToken = token,
            AccessTokenExpiration = accessTokenExpiration
        };
        return tokenDto;
    }
}
