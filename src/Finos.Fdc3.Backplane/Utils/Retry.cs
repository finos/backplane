using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Utils
{
    public class Retry
    {
        /// <summary>
        /// Retry an action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="retryInterval"></param>
        /// <param name="maxAttemptCount">If set to zero, retries forever.</param>
        /// <returns></returns>
        public static async Task Do(
        Func<Task<bool>> action,
        TimeSpan retryInterval,
        int maxAttemptCount = 3, CancellationToken ct = default)
        {
            // if retry count is zero, means retry forever.
            maxAttemptCount = maxAttemptCount == 0 ? int.MaxValue : maxAttemptCount;
            List<Exception> exceptions = new List<Exception>();
            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (attempted > 0)
                    {
                        await Task.Delay(retryInterval);
                    }
                    if (await action())
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}
