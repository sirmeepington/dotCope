using DotCope.Coping;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton(svc => new CopeService(svc.GetRequiredService<IWebHostEnvironment>().WebRootPath));
builder.Services.AddControllers();

builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration.GetValue<string>("SentryDsn");
    o.TracesSampleRate = 1.0;
    o.Debug = builder.Environment.IsDevelopment();
});

var app = builder.Build();

app.UseRouting();

app.UseSentryTracing();

app.UseEndpoints(opt =>
{
    opt.MapDefaultControllerRoute();
});

app.Run();
