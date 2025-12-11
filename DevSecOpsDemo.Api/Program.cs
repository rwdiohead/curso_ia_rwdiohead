var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar middleware
// Swagger habilitado en todos los entornos para facilitar pruebas
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "DevSecOpsDemo API v1");
    options.RoutePrefix = string.Empty; // Swagger UI en la raíz (http://localhost:5000)
});

// Comentado para facilitar pruebas locales
// app.UseHttpsRedirection();

// Endpoint GET /api/health
app.MapGet("/api/health", () => 
{
    return Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow });
})
.WithName("GetHealth")
.Produces(200);

// Endpoint POST /api/suma
app.MapPost("/api/suma", (SumaRequest? request) =>
{
    // Si el body es inválido o nulo, retornar error 400
    if (request is null)
    {
        return Results.BadRequest(new { error = "Error, body inválido" });
    }

    // Si el body es válido, retornar código de éxito con el resultado
    return Results.Ok(new { 
        a = request.A, 
        b = request.B, 
        resultado = request.A + request.B 
    });
})
.WithName("PostSuma")
.Produces(200)
.Produces(400);

app.Run();

// Record para el request de suma
public record SumaRequest(int A, int B);

// Clase Program parcial y pública para permitir acceso desde pruebas de integración
public partial class Program { }
