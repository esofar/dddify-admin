var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming();

    cfg.AddLocalization();

    cfg.AddCurrentUser(options =>
    {
        options.UseIdClaim("sub");
    });

    cfg.AddApiResultWrapping();

    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
        options.UseSnakeCaseNamingConvention();

        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, LogLevel.Information);
        }
    });
});

builder.Services.AddDistributedCache(builder.Configuration);
builder.Services.AddDistributedLock();

builder.Services.AddEmailVerificationCode(builder.Configuration, builder.Environment);

builder.Services.AddJwtAuthentication();
builder.Services.AddSessionCleanup();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<CheckUserPermissionFilter>();
});

builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer<PermissionOperationTransformer>();
});

builder.Services.AddOptions<ForwardedHeadersOptions>()
    .Configure(options =>
    {
        var section = builder.Configuration.GetSection("ForwardedHeaders");
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.ForwardLimit = section.GetValue("ForwardLimit", 1);

        // 保留框架默认的回环代理信任，仅追加实际部署使用的代理。
        foreach (var proxy in section.GetSection("KnownProxies").Get<string[]>() ?? [])
        {
            options.KnownProxies.Add(IPAddress.Parse(proxy));
        }

        foreach (var network in section.GetSection("KnownIPNetworks").Get<string[]>() ?? [])
        {
            options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
        }
    })
    .Validate(options => options.ForwardLimit is > 0, "ForwardedHeaders:ForwardLimit must be positive.")
    .ValidateOnStart();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

// 在 HTTPS 重定向和认证前解析代理转发信息。
app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
