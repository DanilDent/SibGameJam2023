using UnityEngine;
using JHelpers;
using System;

namespace Enemy 
{
    public class EventBusSingleton : MonoBehaviour
    {
        public static EventBusSingleton Instance { get; private set; }

        private EventBus _eventBus;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.parent = null;
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            _eventBus = new EventBus();
        }

        public void Subscribe<T>(Action<T> callback, int priority = 0) where T : ISignal
        {
            _eventBus.Subscribe(callback, priority);
        }

        public void Invoke<T>(T signal) where T : ISignal
        {
            _eventBus.Invoke(signal);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : ISignal
        {
            _eventBus.Unsubscribe(callback);
        }
    }
}