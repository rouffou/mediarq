using FluentAssertions;
using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Mediarq.AspNetCore.Tests;

public class ResultHttpExtensionsTests
{
    [Fact]
    public void Result_Success_Maps_To_NoContent()
        => Result.Success().ToHttpResult().Should().BeOfType<NoContent>();

    [Fact]
    public void ResultT_Success_Maps_To_Ok_With_Value()
    {
        var http = Result.Success(42).ToHttpResult();

        http.Should().BeOfType<Ok<int>>();
        ((Ok<int>)http).Value.Should().Be(42);
    }

    [Theory]
    [InlineData(ErrorType.NotFound, 404)]
    [InlineData(ErrorType.Conflict, 409)]
    [InlineData(ErrorType.Unauthorized, 401)]
    [InlineData(ErrorType.Failure, 500)]
    [InlineData(ErrorType.Problem, 500)]
    [InlineData(ErrorType.Validation, 400)]
    public void ErrorType_Maps_To_Status(ErrorType type, int expected)
        => type.ToStatusCode().Should().Be(expected);

    [Fact]
    public void Failure_Maps_To_Problem_With_Status()
    {
        var http = Result.Failure(ResultError.NotFound("User.NotFound", "missing")).ToHttpResult();

        http.Should().BeOfType<ProblemHttpResult>();
        ((ProblemHttpResult)http).StatusCode.Should().Be(404);
    }

    [Fact]
    public void Validation_Failure_Maps_To_ValidationProblem()
    {
        var error = new ValidationError([new ResultError("Validation.X.Name", "required", ErrorType.Validation)]);

        Result.Failure(error).ToHttpResult().Should().BeOfType<ValidationProblem>();
    }

    [Fact]
    public async Task Async_Maps_Result()
    {
        var http = await Task.FromResult(Result.Success("ok")).ToHttpResultAsync();

        http.Should().BeOfType<Ok<string>>();
    }
}
