using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common.Threading
{
    /// <remarks>
    /// Lifted from <see href="http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously">here</see>
    /// </remarks>>
    public static class AsyncHelper
    {
        public static void RunSynchronously(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            using (var exclusiveSynchronizationContext = new ExclusiveSynchronizationContext())
            {
                SynchronizationContext.SetSynchronizationContext(exclusiveSynchronizationContext);

                exclusiveSynchronizationContext.Post(
                    async _ =>
                    {
                        try
                        {
                            await task();
                        }
                        catch (Exception e)
                        {
                            exclusiveSynchronizationContext.InnerException = e;
                            throw;
                        }
                        finally
                        {
                            exclusiveSynchronizationContext.EndMessageLoop();
                        }
                    },
                    null);

                exclusiveSynchronizationContext.BeginMessageLoop();
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        public static T RunSynchronously<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            T result = default(T);

            using (var exclusiveSynchronizationContext = new ExclusiveSynchronizationContext())
            {
                SynchronizationContext.SetSynchronizationContext(exclusiveSynchronizationContext);
                exclusiveSynchronizationContext.Post(
                    async _ =>
                    {
                        try
                        {
                            result = await task();
                        }
                        catch (Exception e)
                        {
                            exclusiveSynchronizationContext.InnerException = e;
                            throw;
                        }
                        finally
                        {
                            exclusiveSynchronizationContext.EndMessageLoop();
                        }
                    },
                    null);

                exclusiveSynchronizationContext.BeginMessageLoop();
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }

            return result;
        }
    }
}
