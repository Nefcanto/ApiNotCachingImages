namespace Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddResponseCompression();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        var mvcBuilder = services.AddControllers(options =>
        {
            options.EnableEndpointRouting = false;
        });
        mvcBuilder.AddApplicationPart(typeof(Startup).Assembly);

        services.AddResponseCaching();

        IdentityModelEventSource.ShowPII = true;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseActionTime();

        app.UseExceptionHandler();

        app.UseRouting();

        app.Use(next => context =>
        {
            context.Request.EnableBuffering();
            return next(context);
        });

        app.UseResponseCompression();

        DisableCacheForData(app);

        HttpContextHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMvc(options =>
        {
            options.MapRoute("Default", "{controller=Default}/{action=Index}/{id?}");
        });
    }

    public void DisableCacheForData(IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.ToString().Contains("image/resize"))
            {
                context.Response.Headers["Cache-Control"] = "max-age=31536000";
            }
            else
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
            }
            await next.Invoke();
        });
    }
}
