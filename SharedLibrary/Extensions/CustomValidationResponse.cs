using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Dtos;

namespace SharedLibrary.Extensions;

public static class CustomValidationResponse
{
    public static IServiceCollection UseCustomValidationResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values.Where(x => x.Errors.Count > 0).SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                ErrorDto errorDto = new(errors.ToList(), true);
                var response = Response<NoContentResult>.Fail(errorDto, StatusCodes.Status400BadRequest);
                return new BadRequestObjectResult(response);
            };
        });
        return services;
    }
}
