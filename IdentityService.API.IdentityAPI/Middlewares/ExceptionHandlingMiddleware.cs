using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.ErrorResponseDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using System.Text.Json;

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
                await HandleErrorAsync(context, exception, (int)HttpStatusCode.InternalServerError, "application/json");
            }
        }
        private static async Task HandleErrorAsync(HttpContext context, Exception ex, int httpStatusCode, string contentType)
        {
            context.Response.ContentType = contentType;
            context.Response.StatusCode = httpStatusCode;

            HttpStatusCode statisCodeAsHttp = (HttpStatusCode)httpStatusCode;

            var errorResponse =
                IdentityResponse<IIdentityResponseDTO>.
                    CreateErrorResponse(
                        IdentityResponseMesage.InternalServerError,
                        statisCodeAsHttp,
                        []);

            //var json = JsonConvert.SerializeObject(errorResponse);
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
