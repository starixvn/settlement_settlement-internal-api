using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.UnitTests.Services;

public class InvokeExternalAPIServicesTests
{
    private readonly IHttpClientFactory _mockHttpClientFactory;
    private readonly InvokeExternalAPIServices _service;

    public InvokeExternalAPIServicesTests()
    {
        _mockHttpClientFactory = Substitute.For<IHttpClientFactory>();
        _service = new InvokeExternalAPIServicesImp(_mockHttpClientFactory);
    }

    [Fact]
    public async Task PostAsync_WithValidRequest_ShouldReturnResponseContent()
    {
        // Arrange
        var requestString = "{\"test\": \"data\"}";
        var url = "https://api.example.com/test";
        var expectedResponse = "{\"success\": true}";

        var mockHttpMessageHandler = new MockHttpMessageHandler(expectedResponse, HttpStatusCode.OK);
        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await _service.PostAsync(requestString, url);

        // Assert
        Assert.Equal(expectedResponse, result);
        Assert.Equal(1, mockHttpMessageHandler.CallCount);
    }

    [Fact]
    public async Task PostAsync_WithHttpRequestException_ShouldRetryAndEventuallyThrow()
    {
        // Arrange
        var requestString = "{\"test\": \"data\"}";
        var url = "https://api.example.com/test";

        var mockHttpMessageHandler = new MockHttpMessageHandler("", HttpStatusCode.InternalServerError, shouldThrow: true);
        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.CreateClient().Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.PostAsync(requestString, url));
        Assert.Equal(4, mockHttpMessageHandler.CallCount); // Initial call + 3 retries = 4 total calls
    }

    [Fact]
    public async Task PostAsync_WithNonSuccessStatusCode_ShouldThrowHttpRequestException()
    {
        // Arrange
        var requestString = "{\"test\": \"data\"}";
        var url = "https://api.example.com/test";

        var mockHttpMessageHandler = new MockHttpMessageHandler("Error", HttpStatusCode.BadRequest);
        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.CreateClient().Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.PostAsync(requestString, url));
    }

    [Fact]
    public async Task PostAsync_WithValidRequest_ShouldSetCorrectContentType()
    {
        // Arrange
        var requestString = "{\"test\": \"data\"}";
        var url = "https://api.example.com/test";
        var expectedResponse = "{\"success\": true}";

        var mockHttpMessageHandler = new MockHttpMessageHandler(expectedResponse, HttpStatusCode.OK);
        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.CreateClient().Returns(httpClient);

        // Act
        await _service.PostAsync(requestString, url);

        // Assert
        var request = mockHttpMessageHandler.LastRequest;
        Assert.NotNull(request);
        Assert.Equal("application/json", request.Content?.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", request.Content?.Headers.ContentType?.CharSet);
    }

    [Fact]
    public async Task PostAsync_WithValidRequest_ShouldUseCorrectUrl()
    {
        // Arrange
        var requestString = "{\"test\": \"data\"}";
        var url = "https://api.example.com/test";
        var expectedResponse = "{\"success\": true}";

        var mockHttpMessageHandler = new MockHttpMessageHandler(expectedResponse, HttpStatusCode.OK);
        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.CreateClient().Returns(httpClient);

        // Act
        await _service.PostAsync(requestString, url);

        // Assert
        var request = mockHttpMessageHandler.LastRequest;
        Assert.NotNull(request);
        Assert.Equal(url, request.RequestUri?.ToString());
        Assert.Equal(HttpMethod.Post, request.Method);
    }

    [Fact]
    public async Task PostAsync_WithValidRequest_ShouldSendCorrectContent()
    {
        // Arrange
        var requestString = "{\"test\": \"data\"}";
        var url = "https://api.example.com/test";
        var expectedResponse = "{\"success\": true}";

        var mockHttpMessageHandler = new MockHttpMessageHandler(expectedResponse, HttpStatusCode.OK);
        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.CreateClient().Returns(httpClient);

        // Act
        await _service.PostAsync(requestString, url);

        // Assert
        var request = mockHttpMessageHandler.LastRequest;
        Assert.NotNull(request);
        var content = await request.Content!.ReadAsStringAsync();
        Assert.Equal(requestString, content);
    }
}

// Mock HttpMessageHandler for testing
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _statusCode;
    private readonly bool _shouldThrow;
    public int CallCount { get; private set; }
    public HttpRequestMessage? LastRequest { get; private set; }

    public MockHttpMessageHandler(string response, HttpStatusCode statusCode, bool shouldThrow = false)
    {
        _response = response;
        _statusCode = statusCode;
        _shouldThrow = shouldThrow;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount++;
        LastRequest = request;

        if (_shouldThrow)
        {
            throw new HttpRequestException("Simulated network error");
        }

        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_response, Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
