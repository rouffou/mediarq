using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.DataAnnotations.Tests;

public class DataAnnotationsValidatorTests
{
    public sealed class Model
    {
        [Required]
        public string? Name { get; set; }

        [Range(1, 10)]
        public int Quantity { get; set; }
    }

    [Fact]
    public void Valid_Model_Passes()
    {
        var validator = new DataAnnotationsValidator<Model>();

        var results = validator.Validate(new Model { Name = "ok", Quantity = 5 });

        results.Should().OnlyContain(r => r.IsValid);
    }

    [Fact]
    public void Invalid_Model_Fails_With_Property_Errors()
    {
        var validator = new DataAnnotationsValidator<Model>();

        var results = validator.Validate(new Model { Name = null, Quantity = 99 });

        var result = results.Should().ContainSingle().Subject;
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void AddMediarqDataAnnotations_Registers_OpenGeneric_Validator()
    {
        var services = new ServiceCollection();

        services.AddMediarqDataAnnotations();

        services.Should().Contain(d => d.ServiceType == typeof(IValidator<>)
            && d.ImplementationType == typeof(DataAnnotationsValidator<>));
    }
}
