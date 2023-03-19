namespace Api;

public static class HttpContextHelper
{
    private static IHttpContextAccessor httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextHelper.httpContextAccessor = httpContextAccessor;
    }

    public static HttpContext Current => httpContextAccessor.HttpContext;
}
