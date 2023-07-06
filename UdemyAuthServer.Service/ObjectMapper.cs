using AutoMapper;

namespace UdemyAuthServer.Service;

public static class ObjectMapper
{
    private static readonly Lazy<IMapper> _lazy = new(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DtoMapper>();
        });
        return config.CreateMapper();
    });

    public static IMapper Mapper => _lazy.Value;
}
