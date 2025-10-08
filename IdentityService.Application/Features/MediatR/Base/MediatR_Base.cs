using FluentValidation;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Features.MediatR.Base
{
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
