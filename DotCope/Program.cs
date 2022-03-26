using DotCope.Coping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CopeService>(svc => new CopeService(svc.GetRequiredService<IWebHostEnvironment>().WebRootPath));
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(opt =>
{
    opt.MapDefaultControllerRoute();
});

app.Run();
