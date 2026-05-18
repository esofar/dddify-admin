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

builder.Services.AddSms(builder.Configuration, builder.Environment);

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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    if (builder.Environment.IsDevelopment())
    {
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    }
    else
    {
        options.KnownIPNetworks.Add(new System.Net.IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
        options.KnownIPNetworks.Add(new System.Net.IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
        options.KnownIPNetworks.Add(new System.Net.IPNetwork(IPAddress.Parse("192.168.0.0"), 16));
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference("/docs", options =>
    {
        options.Title = "Dddify Admin API";
    });
}

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
