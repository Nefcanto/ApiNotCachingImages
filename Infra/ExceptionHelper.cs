namespace Infra;

public class ExceptionHelper
{
    public static string GetSpecificInfo(Exception ex)
    {
        if (ex is ReflectionTypeLoadException)
        {
            var loaderExceptions = ((ReflectionTypeLoadException)ex).LoaderExceptions;
            var result = "";
            foreach (var loaderException in loaderExceptions)
            {
                result += $"\r\n{loaderException.Message}";
            }
            return result;
        }
        else if (ex is TypeLoadException)
        {
            var typeLoadException = (TypeLoadException)ex;
            return $"\r\n{typeLoadException.Message} => {typeLoadException.TypeName}";
        }
        //else if (ex is HttpResponseException)
        //{
        //    var exception = ((HttpResponseException)ex);
        //    var result = "{0} - {1}".Fill(exception.Response.StatusCode, exception.Response.ReasonPhrase);
        //}
        else if (ex is FileNotFoundException)
        {
            return ((FileNotFoundException)ex).FileName;
        }
        return "";
    }

    public static string TranslateToFriendlyMessage(Exception ex)
    {
        string result = "";
        while (ex != null)
        {
            if (ex is ServerException || ex is ClientException)
            {
                return ex.Message;
            }
            else if (ex is FileNotFoundException)
            {
                return $"File {Path.GetFileNameWithoutExtension(((FileNotFoundException)ex).FileName)} is not found";
            }
            else if (ex is AmbiguousActionException)
            {
                var matches = Regex.Matches(ex.Message, @$"\n[^\)]*\)").OfType<Match>().Select(i => i.Value.Trim()).ToList().Merge();
                return $"Multiple actions matched: {matches}";
            }
            if (result.IsSomething())
            {
                break;
            }
            ex = ex.InnerException;
        }
        return "An error occured. Please notify the adminstrator.";
    }

    public static List<string> BuildExceptionStack(Exception ex)
    {
        var errors = new List<string>();
        BuildExceptionStack(ex, ref errors);
        errors = errors.Where(i => i.IsSomething()).ToList();
        errors = errors.Where(i => i != ex.Message).ToList();
        errors.Insert(0, ex.Message);
        return errors;
    }

    private static void BuildExceptionStack(Exception ex, ref List<string> errors)
    {
        errors.Add(ex.Message);
        errors.Add(ExceptionHelper.GetSpecificInfo(ex));
        var stackTrace = FilterStackTrace(ex.StackTrace);
        errors.AddRange(Regex.Replace(stackTrace, @"at ", "\n").Split('\n'));
        if (ex.InnerException != null)
        {
            BuildExceptionStack(ex.InnerException, ref errors);
        }
        else if (ex is AggregateException)
        {
            var innerExceptions = ((AggregateException)ex).InnerExceptions;
            foreach (var innerException in innerExceptions)
            {
                BuildExceptionStack(innerException, ref errors);
            }
        }
        else if (ex is FileLoadException)
        {
            errors.Add(((FileLoadException)ex).FusionLog);
            errors.Add(((FileLoadException)ex).FileName);
        }
    }

    private static string FilterStackTrace(string stackTrace)
    {
        if (stackTrace.IsNothing())
        {
            return "";
        }
        var newStackTrace = Regex.Replace(stackTrace, @"^(\s*at\s*(System|Microsoft|MySqlConnector)+|-*\s*End|\s*at\s*lambda).*$", "", RegexOptions.Multiline);
        newStackTrace = Regex.Replace(newStackTrace, @"\n\n", "", RegexOptions.Multiline);
        return newStackTrace;
    }
}
