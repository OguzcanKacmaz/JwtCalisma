using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;

namespace SharedLibrary.Extensions;

public static class CustomTokenAuth
{
    public static IServiceCollection AddCustomTokenAuth(this IServiceCollection services, CustomTokenOption tokenOption)
    {
        services.AddAuthentication(opt => //auth sisteme ekledik.
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//1 tane üyelik sistemi olduğu için örneğin kullanıcı ve bayi girişi gibi bir sistem olmadığı için bu şekilde kullanıyoruz.
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // jwt ile haberleşip kontrol yapılması için aynı şemayı kullandık
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt => // jwt ekleyip aynı şemayı verdikki birbiri ile haberleşip kontrolü sağlasın
        {
            opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidIssuer = tokenOption.Issuer, //burada bizim appserrings.json dosyasmızın içerisinde tanımladığımız parametrelerden Issuer olanını ayarlarda atamasını yapıyoruz
                ValidAudience = tokenOption.Audience[0],//burada bizim appserrings.json dosyasmızın içerisinde tanımladığımız parametrelerden Audience olanını ayarlarda atamasını yapıyoruz
                IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOption.SecurityKey),//burada bizim simetrikkey atamasını yapıyoruz
                ValidateIssuerSigningKey = true, //Gelen Token değeri içerisinde simetrik key doğrulamasını yap diyoruz
                ValidateAudience = true, //Gelen Token değeri içerisinde Audience doğrulamasını yap diyoruz
                ValidateIssuer = true,//Gelen Token değeri içerisinde Issuer doğrulamasını yap diyoruz
                ValidateLifetime = true,//Gelen Token değeri içerisinde token süresinin doğrulamasını yap diyoruz
                ClockSkew = TimeSpan.Zero // burada farklı noktalarda ayağa kaldırılan apiler projeler için aradaki farkı sıfıra indirmek için bu ayar yapıldı.Default olarak 5dk ekler
            };
        });
        return services;
    }
}
