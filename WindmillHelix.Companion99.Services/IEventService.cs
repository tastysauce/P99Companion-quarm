using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services
{
    public interface IEventService
    {
        void AddSubscriber<T>(IEventSubscriber<T> subscriber);

        void RemoveSubscriber<T>(IEventSubscriber<T> subscriber);

        Task Raise<T>(T value);

        Task Raise<T>();
    }
}
