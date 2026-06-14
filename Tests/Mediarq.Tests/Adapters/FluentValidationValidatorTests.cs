using FluentAssertions;
using FluentValidation;
using Mediarq.FluentValidation;

namespace Mediarq.Tests.Adapters;

public class FluentValidationValidatorTests
{
    public record Person(string Name);

    private sealed class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator() => RuleFor(p => p.Name).NotEmpty().WithMessage("Name required");
    }

    [Fact]
    public async Task ValidateAsync_Returns_Errors_From_FluentValidation()
    {
        var adapter = new FluentValidationValidator<Person>(new IValidator<Person>[] { new PersonValidator() });

        var results = (await adapter.ValidateAsync(new Person(""))).ToList();

        results.Should().ContainSingle();
        results[0].IsValid.Should().BeFalse();
        results[0].Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be("Name required");
    }

    [Fact]
    public async Task ValidateAsync_Empty_When_Valid()
    {
        var adapter = new FluentValidationValidator<Person>(new IValidator<Person>[] { new PersonValidator() });

        var results = (await adapter.ValidateAsync(new Person("Alice"))).ToList();

        results.Should().BeEmpty();
    }
}
