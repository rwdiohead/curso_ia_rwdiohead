using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DevSecOpsDemo.Api.Tests;

/// <summary>
/// Pruebas de integración para la API DevSecOpsDemo usando WebApplicationFactory
/// </summary>
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    #region Health Endpoint Tests

    [Fact]
    public async Task GetHealth_ReturnsOk_WithCorrectStatus()
    {
        // Arrange
        var expectedStatus = "ok";

        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(content);
        Assert.Equal(expectedStatus, content.Status);
        Assert.True(content.Timestamp <= DateTime.UtcNow);
        Assert.True(content.Timestamp > DateTime.UtcNow.AddMinutes(-1));
    }

    #endregion

    #region Suma Endpoint Tests - Casos Exitosos

    [Fact]
    public async Task PostSuma_WithValidNumbers_ReturnsOk_WithCorrectSum()
    {
        // Arrange
        var request = new { A = 10, B = 15 };
        var expectedSum = 25;

        // Act
        var response = await _client.PostAsJsonAsync("/api/suma", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<SumaResponse>();
        Assert.NotNull(content);
        Assert.Equal(request.A, content.A);
        Assert.Equal(request.B, content.B);
        Assert.Equal(expectedSum, content.Resultado);
    }

    [Fact]
    public async Task PostSuma_WithNegativeNumbers_ReturnsOk_WithCorrectSum()
    {
        // Arrange
        var request = new { A = -10, B = -5 };
        var expectedSum = -15;

        // Act
        var response = await _client.PostAsJsonAsync("/api/suma", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<SumaResponse>();
        Assert.NotNull(content);
        Assert.Equal(expectedSum, content.Resultado);
    }

    [Fact]
    public async Task PostSuma_WithZero_ReturnsOk_WithCorrectSum()
    {
        // Arrange
        var request = new { A = 10, B = 0 };
        var expectedSum = 10;

        // Act
        var response = await _client.PostAsJsonAsync("/api/suma", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<SumaResponse>();
        Assert.NotNull(content);
        Assert.Equal(expectedSum, content.Resultado);
    }

    #endregion

    #region Suma Endpoint Tests - Casos de Error

    [Fact]
    public async Task PostSuma_WithNullBody_ReturnsBadRequest_WithErrorMessage()
    {
        // Arrange
        var expectedError = "Error, body inválido";

        // Act
        var response = await _client.PostAsJsonAsync("/api/suma", (object?)null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(content);
        Assert.Equal(expectedError, content.Error);
    }

    [Fact]
    public async Task PostSuma_WithEmptyJson_ReturnsOk_WithDefaultValues()
    {
        // Arrange
        var requestContent = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/suma", requestContent);

        // Assert
        // JSON vacío {} deserializa a valores por defecto (A=0, B=0)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadFromJsonAsync<SumaResponse>();
        Assert.NotNull(content);
        Assert.Equal(0, content.A);
        Assert.Equal(0, content.B);
        Assert.Equal(0, content.Resultado);
    }

    [Fact]
    public async Task PostSuma_WithInvalidJson_ReturnsBadRequest()
    {
        // Arrange
        var requestContent = new StringContent("invalid-json", System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/suma", requestContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Response Models

    /// <summary>
    /// Modelo de respuesta para el endpoint /api/health
    /// </summary>
    private record HealthResponse(string Status, DateTime Timestamp);

    /// <summary>
    /// Modelo de respuesta para el endpoint /api/suma (caso exitoso)
    /// </summary>
    private record SumaResponse(int A, int B, int Resultado);

    /// <summary>
    /// Modelo de respuesta para errores
    /// </summary>
    private record ErrorResponse(string Error);

    #endregion
}
