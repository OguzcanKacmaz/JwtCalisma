using SharedLibrary.Configurations;
using UdemyAuthServer.Core.Configuration;
using SharedLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOptions"));//option pattern ile appserrings.json dosyas� i�erisinde bulunan TokenOptions'� al�p customtokenoption class�na mapleme i�lemi yapt�k
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));//option pattern ile appserrings.json dosyas� i�erisinde bulunan Clients'� al�p List<Client> nesnesine mapleme i�lemi yapt�k
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOption>();

builder.Services.AddCustomTokenAuth(tokenOptions);





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
