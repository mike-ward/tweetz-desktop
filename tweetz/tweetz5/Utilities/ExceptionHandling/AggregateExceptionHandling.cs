using System.Diagnostics;
using System.Threading.Tasks;

namespace tweetz5.Utilities.ExceptionHandling
{
    internal static class AggregateExceptionHandling
    {
        public static void LogAggregateExceptions(this Task task)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    var aggregateException = t.Exception.Flatten();
                    foreach (var ex in aggregateException.InnerExceptions) Trace.TraceError(ex.ToString());
                }
            },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}