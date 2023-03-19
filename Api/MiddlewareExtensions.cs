namespace Api;

public static class MiddlewareExtensions
{
    public static void UseExceptionHandler(this IApplicationBuilder application)
    {
        application.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                await LogAndCreateErrorJson(context);
            });
        });
    }

    private static async System.Threading.Tasks.Task LogAndCreateErrorJson(HttpContext context)
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>().Error;
        if (exception.GetType().FullName != typeof(ClientException).FullName)
        {
            Logger.LogException(exception);
        }
        dynamic response = new ExpandoObject();
        response.Type = MessageType.Error.ToString();
        if (exception is MySqlException)
        {
            response.Text = "DB exception";
        }
        else
        {
            response.Text = ExceptionHelper.TranslateToFriendlyMessage(exception);
        }
        if (exception is ClientException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            if (((ClientException)exception).Code.IsSomething())
            {
                response.Code = ((ClientException)exception).Code;
            }
            if (((ClientException)exception).Data != null)
            {
                response.Data = ((ClientException)exception).Data;
            }
        }
        if (exception is ServerException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        if (InfraConfig.IsDeveloping)
        {
            response.Stack = ExceptionHelper.BuildExceptionStack(exception);
        }
        context.Response.ContentType = "application/json; charset=utf-8";
        string result = ((object)response).Serialize();
        await context.Response.WriteAsync(result);
    }

    public static void UseApiDelayedResponse(this IApplicationBuilder application)
    {
        application.Use(async (context, next) =>
        {
            Thread.Sleep(5000);
            await next.Invoke();
        });
    }

    public static void UseActionTime(this IApplicationBuilder application)
    {
        application.Use(async (context, next) =>
        {
            var watch = new Stopwatch();
            watch.Start();
            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                watch.Stop();
                context.Response.Headers.Add("Milliseconds", watch.ElapsedMilliseconds.ToString());
                return System.Threading.Tasks.Task.CompletedTask;
            }, context);
            await next.Invoke(); ;
        });
    }
}
