namespace Api;

public class UnauthorizedResult : ContentResult
{
    public UnauthorizedResult(string message = null)
    {
        dynamic content = new ExpandoObject();
        content.Type = MessageType.Error.ToString();
        content.Message = message ?? "UnauthorizedAccess";
        content.Code = "UnauthorizedAccess";
        this.StatusCode = (int)HttpStatusCode.OK;
        this.ContentType = "application/json; charset=utf-8";
        this.Content = ((object)content).Serialize();
    }
}
