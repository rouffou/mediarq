using FluentAssertions;
using Mediarq.AspNetCore.Tests.Fixtures;
using Mediarq.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;

namespace Mediarq.AspNetCore.Tests;

public class MediarqEndpointRouteBuilderExtensionsOpenApiTests
{
    private static WebApplication CreateMappedApp()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMediarq(isHttp: false, typeof(MediarqEndpointRouteBuilderExtensionsOpenApiTests).Assembly);

        var app = builder.Build();
        app.MapMediarq(typeof(MediarqEndpointRouteBuilderExtensionsOpenApiTests).Assembly);
        return app;
    }

    private static RouteEndpoint FindEndpoint(WebApplication app, string method, string pattern) =>
        ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(ds => ds.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(e =>
                e.RoutePattern.RawText == pattern &&
                e.Metadata.GetMetadata<IHttpMethodMetadata>()!.HttpMethods.Contains(method));

    private static IReadOnlyList<IProducesResponseTypeMetadata> ResponseMetadata(RouteEndpoint endpoint) =>
        endpoint.Metadata.GetOrderedMetadata<IProducesResponseTypeMetadata>();

    private static readonly int[] FailureStatusCodes =
    [
        StatusCodes.Status400BadRequest,
        StatusCodes.Status401Unauthorized,
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound,
        StatusCodes.Status409Conflict,
        StatusCodes.Status500InternalServerError,
    ];

    [Fact]
    public void Query_Returning_ResultOfT_Declares_200_With_The_Value_Type_And_Failure_Statuses()
    {
        using var app = CreateMappedApp();
        var metadata = ResponseMetadata(FindEndpoint(app, "GET", "/orders/{id}"));

        metadata.Should().ContainSingle(m => m.StatusCode == StatusCodes.Status200OK && m.Type == typeof(OrderDto));
        metadata.Select(m => m.StatusCode).Should().Contain(FailureStatusCodes);
    }

    [Fact]
    public void Command_Returning_ResultOfT_Declares_200_With_The_Value_Type_And_Failure_Statuses()
    {
        using var app = CreateMappedApp();
        var metadata = ResponseMetadata(FindEndpoint(app, "POST", "/orders"));

        metadata.Should().ContainSingle(m => m.StatusCode == StatusCodes.Status200OK && m.Type == typeof(Guid));
        metadata.Select(m => m.StatusCode).Should().Contain(FailureStatusCodes);
    }

    [Fact]
    public void Command_Returning_Result_Declares_204_And_Failure_Statuses()
    {
        using var app = CreateMappedApp();
        var metadata = ResponseMetadata(FindEndpoint(app, "PUT", "/orders/{id}/customer"));

        metadata.Should().ContainSingle(m => m.StatusCode == StatusCodes.Status204NoContent);
        metadata.Select(m => m.StatusCode).Should().Contain(FailureStatusCodes);
    }

    [Fact]
    public void No_Result_Command_Declares_Only_204()
    {
        using var app = CreateMappedApp();
        var metadata = ResponseMetadata(FindEndpoint(app, "DELETE", "/orders/{id}"));

        metadata.Should().ContainSingle();
        metadata.Single().StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
