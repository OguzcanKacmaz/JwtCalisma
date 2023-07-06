using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Configurations;
using SharedLibrary.Extensions;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.Entities;
using UdemyAuthServer.Core.Repositories;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;
using UdemyAuthServer.Data;
using UdemyAuthServer.Data.Repositories;
using UdemyAuthServer.Service;
using UdemyAuthServer.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOptions"));//option pattern ile appserrings.json dosyasý içerisinde bulunan TokenOptions'ý alýp customtokenoption classýna mapleme iþlemi yaptýk
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));//option pattern ile appserrings.json dosyasý içerisinde bulunan Clients'ý alýp List<Client> nesnesine mapleme iþlemi yaptýk
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOption>();

builder.Services
    .AddCustomTokenAuth(tokenOptions)
    .UseCustomValidationResponse();

builder.Services.AddAutoMapper(typeof(DtoMapper));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();




builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
builder.Services.AddIdentity<UserApp, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true; //email adresininin unique olmasý gerektiðini belirtiyoruz
    opt.Password.RequireNonAlphanumeric = false; //*!? gibi karakterleri zorunlu tutmadýk
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCustomException();
}
app.UseCustomException();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
