using FluentAssertions;
using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace Mediarq.AspNetCore.Tests;

public class ResultActionResultExtensionsTests
{
    [Fact]
    public void Result_Success_Maps_To_NoContent()
        => Result.Success().ToActionResult().Should().BeOfType<NoContentResult>();

    [Fact]
    public void ResultT_Success_Maps_To_Ok_With_Value()
    {
        var action = Result.Success(42).ToActionResult();

        action.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)action).Value.Should().Be(42);
    }

    [Fact]
    public void Failure_Maps_To_Problem_With_Status()
    {
        var action = Result.Failure(ResultError.NotFound("User.NotFound", "missing")).ToActionResult();

        action.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)action;
        objectResult.StatusCode.Should().Be(404);
        objectResult.Value.Should().BeOfType<ProblemDetails>();
    }

    [Fact]
    public void Validation_Failure_Maps_To_ValidationProblemDetails()
    {
        var error = new ValidationError([new ResultError("Name", "required", ErrorType.Validation)]);

        var action = Result.Failure(error).ToActionResult();

        var objectResult = action.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(400);
        objectResult.Value.Should().BeOfType<ValidationProblemDetails>();
    }

    [Fact]
    public void Failure_Honors_ConfigureProblem_Hook()
    {
        var action = Result.Failure(ResultError.Failure("X", "boom"))
            .ToActionResult(problem => problem.Extensions["traceId"] = "abc");

        var problem = (ProblemDetails)((ObjectResult)action).Value!;
        problem.Extensions.Should().ContainKey("traceId");
    }

    [Fact]
    public async Task Async_Maps_Result()
    {
        var action = await Task.FromResult(Result.Success("ok")).ToActionResultAsync();

        action.Should().BeOfType<OkObjectResult>();
    }
}
