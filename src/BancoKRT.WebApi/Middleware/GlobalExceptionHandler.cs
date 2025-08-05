using BancoKRT.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BancoKRT.WebApi.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Ocorreu uma exceção: {Message}", exception.Message);

            var (statusCode, title, detail) = exception switch
            {
                InvalidOperationException ex => (HttpStatusCode.Conflict, "Operação Inválida", ex.Message),
                AccessDeniedException accessDeniedException => (HttpStatusCode.Forbidden, "Acesso Negado", accessDeniedException.Message),
                _ => (HttpStatusCode.InternalServerError, "Erro Inesperado", "Ocorreu um erro inesperado no servidor.")
            };

            httpContext.Response.StatusCode = (int)statusCode;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail
            }, cancellationToken);

            return true;
        }
    }
}
