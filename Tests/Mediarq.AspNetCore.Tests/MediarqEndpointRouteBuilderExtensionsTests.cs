using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Mediarq.AspNetCore.Tests.Fixtures;
using Mediarq.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;

namespace Mediarq.AspNetCore.Tests;

public class MediarqEndpointRouteBuilderExtensionsTests
{
    private static async Task<WebApplication> CreateAppAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddMediarq(isHttp: false, typeof(MediarqEndpointRouteBuilderExtensionsTests).Assembly);

        var app = builder.Build();
        app.MapMediarq(typeof(MediarqEndpointRouteBuilderExtensionsTests).Assembly);
        await app.StartAsync();
        return app;
    }

    [Fact]
    public async Task Get_Binds_The_Route_Parameter_And_Returns_200_With_The_Value()
    {
        await using var app = await CreateAppAsync();
        var client = app.GetTestClient();
        var id = Guid.NewGuid();
        Store.Orders[id] = new OrderDto(id, "Alice");

        var response = await client.GetAsync($"/orders/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<OrderDto>();
        dto!.Customer.Should().Be("Alice");
    }

    [Fact]
    public async Task Get_For_An_Unknown_Id_Returns_404()
    {
        await using var app = await CreateAppAsync();
        var client = app.GetTestClient();

        var response = await client.GetAsync($"/orders/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Binds_The_Body_And_Returns_200_With_The_Created_Id()
    {
        await using var app = await CreateAppAsync();
        var client = app.GetTestClient();

        var response = await client.PostAsJsonAsync("/orders", new { Customer = "Bob" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        Store.Orders.Should().ContainKey(id).WhoseValue.Customer.Should().Be("Bob");
    }

    [Fact]
    public async Task Delete_Binds_The_Route_Parameter_And_Returns_204_For_A_Void_Command()
    {
        await using var app = await CreateAppAsync();
        var client = app.GetTestClient();
        var id = Guid.NewGuid();
        Store.Orders[id] = new OrderDto(id, "Carol");

        var response = await client.DeleteAsync($"/orders/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        Store.Orders.Should().NotContainKey(id);
    }

    [Fact]
    public async Task An_Unattributed_Command_Is_Not_Mapped()
    {
        await using var app = await CreateAppAsync();
        var client = app.GetTestClient();

        var response = await client.PostAsync("/unmapped-command", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void MapMediarq_Returns_A_Chainable_RouteGroupBuilder()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMediarq(isHttp: false, typeof(MediarqEndpointRouteBuilderExtensionsTests).Assembly);
        using var app = builder.Build();

        var group = app.MapMediarq(typeof(MediarqEndpointRouteBuilderExtensionsTests).Assembly);
        var act = () => group.WithTags("orders");

        act.Should().NotThrow();
    }

    [Fact]
    public void MapMediarq_Throws_On_Null_Endpoints()
    {
        IEndpointRouteBuilder endpoints = null!;

        var act = () => endpoints.MapMediarq();

        act.Should().Throw<ArgumentNullException>();
    }
}
