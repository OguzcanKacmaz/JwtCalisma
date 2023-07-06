using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Entities;
using UdemyAuthServer.Core.Repositories;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;

namespace UdemyAuthServer.Service.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<UserApp> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;
    private readonly List<Client> _optionsClient;

    public AuthenticationService(ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService, IOptions<List<Client>> optionsClient)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _userRefreshTokenService = userRefreshTokenService;
        _optionsClient = optionsClient.Value;
    }
    public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
            return Response<TokenDto>.Fail("Email veya Şifre Yanlış", StatusCodes.Status400BadRequest, true);
        var checkUserPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!checkUserPassword)
            return Response<TokenDto>.Fail("Email veya Şifre Yanlış", StatusCodes.Status400BadRequest, true);
        var token = await _tokenService.CreateToken(user);
        var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
        if (userRefreshToken is null)
            await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
        else
        {
            userRefreshToken.Code = token.RefreshToken;
            userRefreshToken.Expiration = token.RefreshTokenExpiration;
        }
        await _unitOfWork.SaveChancesAsync();
        return Response<TokenDto>.Success(token, StatusCodes.Status200OK);
    }

    public async Task<Response<TokenDto>> CreateTokenByRefreshAsync(string refreshToken)
    {
        var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).FirstOrDefaultAsync();
        if (existRefreshToken is null)
            return Response<TokenDto>.Fail("RefreshToken Bulunamadı", StatusCodes.Status404NotFound, true);
        var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
        if (user is null)
            return Response<TokenDto>.Fail("UserId Bulunamadı", StatusCodes.Status404NotFound, true);
        var token =await _tokenService.CreateToken(user);
        existRefreshToken.Code = token.RefreshToken;
        existRefreshToken.Expiration = token.RefreshTokenExpiration;
        await _unitOfWork.SaveChancesAsync();
        return Response<TokenDto>.Success(token, StatusCodes.Status200OK);
    }

    public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto loginDto)
    {
        var client = _optionsClient.SingleOrDefault(x => x.Id == loginDto.ClientId && x.Secret == loginDto.ClientSecret);
        if (client is null)
            return Response<ClientTokenDto>.Fail("CliendId veya Secret bulunamadı", StatusCodes.Status404NotFound, true);
        var token = _tokenService.CreateTokenByClient(client);
        return Response<ClientTokenDto>.Success(token, StatusCodes.Status200OK);
    }

    public async Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken)
    {
        var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).FirstOrDefaultAsync();
        if (existRefreshToken is null)
            return Response<NoDataDto>.Fail("RefreshToken Bulunamadı", StatusCodes.Status404NotFound, true);
        _userRefreshTokenService.Remove(existRefreshToken);
        await _unitOfWork.SaveChancesAsync();
        return Response<NoDataDto>.Success(StatusCodes.Status200OK);
    }
}
