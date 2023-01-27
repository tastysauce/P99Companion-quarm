using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services
{
    public class EventService : IEventService
    {
        private object _subscriberLock = new object();
        private Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();

        public void AddSubscriber<T>(IEventSubscriber<T> subscriber)
        {
            lock(_subscriberLock)
            {
                var type = typeof(T);
                if(!_subscribers.ContainsKey(type))
                {
                    _subscribers.Add(type, new List<object>());
                }

                _subscribers[type].Add(subscriber);
            }
        }

        public void RemoveSubscriber<T>(IEventSubscriber<T> subscriber)
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                return;
            }

            _subscribers[type].Remove(subscriber);
        }

        public Task Raise<T>(T value)
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                return Task.CompletedTask;
            }

            var items = _subscribers[type].Select(x => x as IEventSubscriber<T>).ToList();
            var tasks = items.Select(x => x.Handle(value));
            return Task.WhenAll(tasks);
        }

        public Task Raise<T>()
        {
            return Raise<T>(default(T));
        }
    }
}
