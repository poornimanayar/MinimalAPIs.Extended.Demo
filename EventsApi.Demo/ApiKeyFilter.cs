namespace EventsApi.Structuring;

public class ApiKeyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
            //access to HttpContext
        var apiKey = context.HttpContext.Request.Headers["api-key"];

        Console.WriteLine("CHECKING FOR API KEY BEFORE ROUTE HANDLER EXECUTION");

        if (string.IsNullOrEmpty(apiKey))
        {
            return TypedResults.BadRequest("Missing API Key");
        }

        ////invokes the next filter in the pipeline or the route handler if no other filters present
        //runs before route handler execution
        var result = await next.Invoke(context);

        Console.WriteLine("ADDING CUSTOM HEADERS AFTER ROUTE HANDLER EXECUTION");

        //runs after route handler execution
        context.HttpContext.Response.Headers.Add("custom-header", "custom-header-value");

        return result;
    }
}