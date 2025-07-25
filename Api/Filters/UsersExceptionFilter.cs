using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Filters;

public class UsersExceptionFilter : IExceptionFilter
{
    private readonly ILogger<UsersExceptionFilter> _logger;
    private readonly IWebHostEnvironment _env;

    public UsersExceptionFilter(ILogger<UsersExceptionFilter> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    
    public void OnException(ExceptionContext context)
    {
        /*if (context.Exception is ExpiredToken expiredTokenException)
        {
            _logger.LogWarning($"Token expired: Token = {expiredTokenException.Token}");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Token expired"
            };

            context.Result = new UnauthorizedObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }*/
        if (context.Exception is InvalidTokenException invalidTokenException)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Provided token is invalid",
                Detail = "Try to re-login"
            };

            context.Result = new UnauthorizedObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }
        else if (context.Exception is UserAlreadyExistsException userExistsException)
        {
            //_logger.LogWarning($"User exists = {userExistsException.Email}");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "User with this email already existst",
                Detail = "Try to use another email, or restore this one"
            };

            context.Result = new ConflictObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }
        else if (context.Exception is UserNotFoundException or InvalidCredentialsException)
        {
            var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "User with this login and password not found",
                    Detail = "Try to check is you're email is correct, after this check password"
                };

            context.Result = new NotFoundObjectResult(problemDetails);
            context.ExceptionHandled = true;
        } /*else if (context.Exception is UserAlreadyVerified)
        {
            _logger.LogWarning("UserAlreadyVerified");
      
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "User Already Verified",
                Detail = "Try to login"
            };

            context.Result = new ConflictObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }*/
        /*else if (context.Exception is InvalidCode)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status406NotAcceptable,
                Title = "User Already Verified",
                Detail = "Try to login"
            };

            context.Result = new ObjectResult(problemDetails);
            context.ExceptionHandled = true;
        } else if (context.Exception is UserWasNotVerifyed)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status204NoContent,
                Title = "User was not verified",
                Detail = "Try to register again"
            };
            

            context.Result = new ObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }*/
        // Handle other exceptions (built-in or custom) here
        else if (!_env.IsDevelopment()) // Only show generic error in production
        {
            _logger.LogError($"An unhandled exception occurred: {context.Exception.Message}");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "Please contact support if the issue persists."
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}