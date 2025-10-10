using SharedResources.DTOs.ErrorResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.API.IdentityAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                ErrorResponse<ErrorDTO>.
                    CreateErrorResponse(
                        new ErrorDTO()
                        {
                            Exception = exception
                        },
                        ErrorResponseMessage.InternalServerError,
                        System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
