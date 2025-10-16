using FluentValidation;
using Microsoft.Extensions.Logging;

namespace SharedResources.MediatR
{
    /// <summary>
    /// Provides a reusable base class for MediatR handlers that includes validation, logging, and service dependencies.
    /// </summary>
    /// <typeparam name="TValidationType">
    /// The type of the request object that will be validated.
    /// </typeparam>
    /// <typeparam name="TloggerType">
    /// The type used by the logger to categorize log entries.
    /// </typeparam>
    /// <typeparam name="TService">
    /// The service dependency used to handle the business logic (e.g., authentication, user management).
    /// </typeparam>
    /// <remarks>
    /// This base class helps reduce boilerplate code in MediatR handlers by injecting common dependencies.
    /// </remarks>

    public class MediatR_Base
        <TValidationType,
        TloggerType,
        TService
        >
    {
        protected readonly IValidator<TValidationType> _validator;
        protected readonly ILogger<TloggerType> _logger;
        protected readonly TService _service;
        public MediatR_Base(
            IValidator<TValidationType> validator,
            ILogger<TloggerType> logger,
            TService service
            )
        {
            _validator = validator;
            _logger = logger;
            _service = service;
        }
    }
}
