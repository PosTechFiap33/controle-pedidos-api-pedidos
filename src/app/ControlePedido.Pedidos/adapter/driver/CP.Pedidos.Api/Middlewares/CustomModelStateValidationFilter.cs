using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CP.Pedidos.Api.Middlewares;

[ExcludeFromCodeCoverage]
public class CustomModelStateValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = new List<string>();

            foreach (var modelStateEntry in context.ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            var errorDetails = new ValidationProblemDetails(new Dictionary<string, string[]> {
                    {
                     "Mensagens", errors.ToArray()
                    }
                });


            context.Result = new BadRequestObjectResult(errorDetails);
        }
    }
}
