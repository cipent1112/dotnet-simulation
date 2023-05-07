using Microsoft.AspNetCore.Mvc.Filters;

namespace Simulation.Performance.Attributes;

public class ResponseAttribute : Attribute, IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        throw new NotImplementedException();
    }
}