namespace SmartAccountant.Core.Helpers;

public static class ExceptionExtensions
{
    /// <exception cref="ArgumentNullException" />
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
            // AggregateException includes top-level inner exception messages in its message already.
            // Therefore, we skip inner exceptions.
            var innerInnerExceptions = aggrEx.InnerExceptions
                .Where(x => x.InnerException != null)
                .SelectMany(x => x.InnerException!.GetAllExceptions());

            foreach (Exception innerEx in innerInnerExceptions)
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
