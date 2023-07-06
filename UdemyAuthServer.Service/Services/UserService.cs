using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using SharedLibrary.Exeptions;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Entities;
using UdemyAuthServer.Core.Services;

namespace UdemyAuthServer.Service.Services;

public class UserService : IUserService
{
    private readonly UserManager<UserApp> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<UserApp> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = new UserApp() { Email = createUserDto.Email, UserName = createUserDto.UserName };
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description).ToList();
            return Response<UserAppDto>.Fail(new ErrorDto(errors, true), StatusCodes.Status400BadRequest);
        }
        return Response<UserAppDto>.Success(_mapper.Map<UserAppDto>(user), StatusCodes.Status201Created);
    }

    public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
            return Response<UserAppDto>.Fail("Kullanıcı Adı Bulunamadı", StatusCodes.Status404NotFound, true);
        return Response<UserAppDto>.Success(_mapper.Map<UserAppDto>(user), StatusCodes.Status200OK);
    }
}
