using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

public class GenericValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public GenericValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ValidationResult> ValidateAsync<T>(T dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var validator = _serviceProvider.GetService<IValidator<T>>();

        if (validator == null)
            throw new InvalidOperationException($"No validator registered for {typeof(T).Name}");

        return await validator.ValidateAsync(dto);
    }

    public async Task ValidateAndThrowAsync<T>(T dto)
    {
        var result = await ValidateAsync(dto);

        if (!result.IsValid)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"Validation failed for {typeof(T).Name}: {errors}");
        }
    }
}