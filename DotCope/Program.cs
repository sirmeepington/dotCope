using DotCope;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CopeService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(opt =>
{
    opt.MapDefaultControllerRoute();
});

app.Run();
