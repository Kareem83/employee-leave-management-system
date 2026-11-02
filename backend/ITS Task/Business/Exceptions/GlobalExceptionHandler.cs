
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ITS_Task.Business.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode;
            string title;

            if (exception is DbUpdateConcurrencyException)
            {
                statusCode = StatusCodes.Status409Conflict;
                title = "The data was modified by another request.";
            }
            else if (exception is SqlException sqlEx &&
                     (sqlEx.Number == 1205 || sqlEx.Number == 1222))
            {
                statusCode = StatusCodes.Status409Conflict;
                title = "A concurrent operation prevented completion. Please retry.";
            }
            else
            {
                return false;
            }

            httpContext.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title
            };

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);

            return true;
        }
    }
}
