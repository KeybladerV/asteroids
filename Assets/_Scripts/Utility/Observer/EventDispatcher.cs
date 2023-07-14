using System;
using System.Collections.Generic;
using System.Linq;

namespace Asteroid.Utility.Observer
{
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IDictionary<Event, object> _listeners;
        private readonly IDictionary<Event, object> _listenersOnce;

        public EventDispatcher()
        {
            _listeners = new Dictionary<Event, object>();
            _listenersOnce = new Dictionary<Event, object>();
        }

        public void AddListener(Event @event, Action listener)
        {
            if (!_listeners.TryGetValue(@event, out var listenersObj) || listenersObj == null)
            {
                _listeners[@event] = listener;
                return;
            }
            var listeners = (Action)listenersObj;
            if (!listeners.GetInvocationList().Contains(listener))
                _listeners[@event] = listeners + listener;
        }

        public void AddListener<T1>(Event<T1> @event, Action<T1> listener)
        {
            if (!_listeners.TryGetValue(@event, out var listenersObj) || listenersObj == null)
            {
                _listeners[@event] = listener;
                return;
            }
            var listeners = (Action<T1>)listenersObj;
            if (!listeners.GetInvocationList().Contains(listener))
                _listeners[@event] = listeners + listener;
        }

        public void AddListener<T1, T2>(Event<T1, T2> @event, Action<T1, T2> listener)
        {
            if (!_listeners.TryGetValue(@event, out var listenersObj) || listenersObj == null)
            {
                _listeners[@event] = listener;
                return;
            }
            var listeners = (Action<T1, T2>)listenersObj;
            if (!listeners.GetInvocationList().Contains(listener))
                _listeners[@event] = listeners + listener;
        }

        public void AddListenerOnce(Event @event, Action listener)
        {
            if (!_listenersOnce.TryGetValue(@event, out var listenersObj) || listenersObj == null)
            {
                _listenersOnce[@event] = listener;
                return;
            }
            var listeners = (Action)listenersObj;
            if (!listeners.GetInvocationList().Contains(listener))
                _listenersOnce[@event] = listeners + listener;
        }

        public void AddListenerOnce<T1>(Event<T1> @event, Action<T1> listener)
        {
            if (!_listenersOnce.TryGetValue(@event, out var listenersObj) || listenersObj == null)
            {
                _listenersOnce[@event] = listener;
                return;
            }
            var listeners = (Action<T1>)listenersObj;
            if (!listeners.GetInvocationList().Contains(listener))
                _listenersOnce[@event] = listeners + listener;
        }

        public void AddListenerOnce<T1, T2>(Event<T1, T2> @event, Action<T1, T2> listener)
        {
            if (!_listenersOnce.TryGetValue(@event, out var listenersObj) || listenersObj == null)
            {
                _listenersOnce[@event] = listener;
                return;
            }
            var listeners = (Action<T1, T2>)listenersObj;
            if (!listeners.GetInvocationList().Contains(listener))
                _listenersOnce[@event] = listeners + listener;
        }

        public bool ContainsListener(Event @event, Action listener)
        {
            if (_listeners.TryGetValue(@event, out var listeners) && listeners != null)
                return Array.IndexOf(((Action)listeners).GetInvocationList(), listener) != -1;
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce) && listenersOnce != null)
                return Array.IndexOf(((Action)listenersOnce).GetInvocationList(), listener) != -1;

            return false;
        }

        public bool ContainsListener<T1>(Event<T1> @event, Action<T1> listener)
        {
            if (_listeners.TryGetValue(@event, out var listeners) && listeners != null)
                return Array.IndexOf(((Action<T1>)listeners).GetInvocationList(), listener) != -1;
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce) && listenersOnce != null)
                return Array.IndexOf(((Action<T1>)listenersOnce).GetInvocationList(), listener) != -1;

            return false;
        }

        public bool ContainsListener<T1, T2>(Event<T1, T2> @event, Action<T1, T2> listener)
        {
            if (_listeners.TryGetValue(@event, out var listeners) && listeners != null)
                return Array.IndexOf(((Action<T1, T2>)listeners).GetInvocationList(), listener) != -1;
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce) && listenersOnce != null)
                return Array.IndexOf(((Action<T1, T2>)listenersOnce).GetInvocationList(), listener) != -1;

            return false;
        }

        public void RemoveListener(Event @event, Action listener)
        {
            if (_listeners.TryGetValue(@event, out var listeners))
                _listeners[@event] = (Action)listeners - listener;
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce))
                _listenersOnce[@event] = (Action)listenersOnce - listener;
        }

        public void RemoveListener<T1>(Event<T1> @event, Action<T1> listener)
        {
            if (_listeners.TryGetValue(@event, out var listeners))
                _listeners[@event] = (Action<T1>)listeners - listener;
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce))
                _listenersOnce[@event] = (Action<T1>)listenersOnce - listener;
        }

        public void RemoveListener<T1, T2>(Event<T1, T2> @event, Action<T1, T2> listener)
        {
            if (_listeners.TryGetValue(@event, out var listeners))
                _listeners[@event] = (Action<T1, T2>)listeners - listener;
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce))
                _listenersOnce[@event] = (Action<T1, T2>)listenersOnce - listener;
        }

        public void RemoveAllListeners()
        {
            _listeners.Clear();
            _listenersOnce.Clear();
        }

        public void RemoveAllListeners(Event @event)
        {
            _listeners.Remove(@event);
            _listenersOnce.Remove(@event);
        }

        public void Dispatch(Event @event)
        {
            if (_listeners.TryGetValue(@event, out var listeners))
                ((Action)listeners)?.Invoke();

            if (_listenersOnce.TryGetValue(@event, out var listenersOnce))
            {
                _listenersOnce.Remove(@event);
                ((Action)listenersOnce)?.Invoke();
            }
        }

        public void Dispatch<T1>(Event<T1> @event, T1 param01)
        {
            if (_listeners.TryGetValue(@event, out var listeners))
                ((Action<T1>)listeners)?.Invoke(param01);
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce))
            {
                _listenersOnce.Remove(@event);
                ((Action<T1>)listenersOnce)?.Invoke(param01);
            }
        }

        public void Dispatch<T1, T2>(Event<T1, T2> @event, T1 param01, T2 param02)
        {
            if (_listeners.TryGetValue(@event, out var listeners))
                ((Action<T1, T2>)listeners)?.Invoke(param01, param02);
            
            if (_listenersOnce.TryGetValue(@event, out var listenersOnce))
            {
                _listenersOnce.Remove(@event);
                ((Action<T1, T2>)listenersOnce)?.Invoke(param01, param02);
            }
        }
    }
}