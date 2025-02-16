namespace SmartAccountant.Core.Helpers;

public static class ExceptionExtensions
{
    public static string GetAllMessages(this Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        IEnumerable<string> messages = exception
            .GetAllExceptions()
            .Where(e => !string.IsNullOrWhiteSpace(e.Message))
            .Select(e => e.Message);

        string flattened = string.Join(Environment.NewLine, messages);
        return flattened;
    }

    private static IEnumerable<Exception> GetAllExceptions(this Exception exception)
    {
        yield return exception;

        if (exception is AggregateException aggrEx)
        {
            foreach (Exception innerEx in aggrEx.InnerExceptions.SelectMany(e => e.GetAllExceptions()))
            {
                yield return innerEx;
            }
        }
        else if (exception.InnerException != null)
        {
            foreach (Exception innerEx in exception.InnerException.GetAllExceptions())
            {
                yield return innerEx;
            }
        }
    }
}
