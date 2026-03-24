using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using DA;
using DA.Repositorios;
using Flujo;
using Reglas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Seguridad.API", Version = "v1" });
});

builder.Services.AddScoped<IRepositorioDapper, RepositorioDapper>();
builder.Services.AddScoped<IUsuarioDA, UsuarioDA>();
builder.Services.AddScoped<IUsuarioFlujo, UsuarioFlujo>();
builder.Services.AddScoped<IAutenticacionBC, AutenticacionBC>();
builder.Services.AddScoped<IAutenticacionFlujo, AutenticacionFlujo>();

var politicaAcceso = "PoliticaDeAcceso";

builder.Services.AddCors(options =>
{
    options.AddPolicy(politicaAcceso, policy =>
    {
        policy.WithOrigins(
                "https://localhost",
                "https://localhost:50427",
                "https://localhost:50428",
                "https://localhost:7136",
                "https://localhost:7039"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(politicaAcceso);

app.MapControllers();

app.Run();