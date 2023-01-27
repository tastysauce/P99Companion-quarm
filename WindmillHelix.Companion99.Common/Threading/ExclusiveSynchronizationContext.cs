using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common.Threading
{
    internal class ExclusiveSynchronizationContext : SynchronizationContext, IDisposable
    {
        private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);

        private readonly Queue<Tuple<SendOrPostCallback, object>> _items =
            new Queue<Tuple<SendOrPostCallback, object>>();

        private bool _isDone;

        public Exception InnerException { get; set; }

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("We cannot send to our same thread");
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            lock (_items)
            {
                _items.Enqueue(Tuple.Create(d, state));
            }

            _workItemsWaiting.Set();
        }

        public void EndMessageLoop()
        {
            Post(_ => _isDone = true, null);
        }

        public void BeginMessageLoop()
        {
            while (!_isDone)
            {
                Tuple<SendOrPostCallback, object> task = null;
                lock (_items)
                {
                    if (_items.Count > 0)
                    {
                        task = _items.Dequeue();
                    }
                }

                if (task != null)
                {
                    task.Item1(task.Item2);
                    if (InnerException != null)
                    {
                        // the method threw an exeption
                        throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                    }
                }
                else
                {
                    _workItemsWaiting.WaitOne();
                }
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        public void Dispose()
        {
            _workItemsWaiting.Dispose();
        }
    }
}
