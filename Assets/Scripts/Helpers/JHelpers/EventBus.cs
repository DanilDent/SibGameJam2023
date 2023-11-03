using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JHelpers
{
    public class EventBus
    {
        private Dictionary<string, List<CallbackWithPriority>> _signalCallbacks = new Dictionary<string, List<CallbackWithPriority>>();

        public void Subscribe<T>(Action<T> callback, int priority = 0) where T : ISignal
        {
            //mb use hashcode or smth else
            string key = typeof(T).Name;

            if (_signalCallbacks.ContainsKey(key))
            {
                _signalCallbacks[key].Add(new CallbackWithPriority(priority, callback));
            }
            else
            {
                _signalCallbacks.Add(key, new List<CallbackWithPriority>() { new(priority, callback) });
            }

            _signalCallbacks[key] = _signalCallbacks[key].OrderByDescending(x => x.Priority).ToList();
        }

        public void Invoke<T>(T signal) where T : ISignal
        {
            string key = typeof(T).Name;

            if (_signalCallbacks.ContainsKey(key))
            {
                foreach (var obj in _signalCallbacks[key])
                {
                    var callback = obj.Callback as Action<T>;
                    callback?.Invoke(signal);
                }
            }
        }

        public void Unsubscribe<T>(Action<T> callback) where T : ISignal
        {
            string key = typeof(T).Name;

            if (_signalCallbacks.ContainsKey(key))
            {
                var callbackToDelete = _signalCallbacks[key].FirstOrDefault(x => x.Callback.Equals(callback));
                
                if (callbackToDelete != null)
                {
                    _signalCallbacks[key].Remove(callbackToDelete);
                }
            }
            else
            {
                Debug.LogErrorFormat("Trying to unsubscribe for not existing key! {0} ", key);
            }
        }
    }
}