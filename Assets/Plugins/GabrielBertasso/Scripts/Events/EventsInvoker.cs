using System;
using System.Collections;
using System.Collections.Generic;
using GabrielBertasso.DataStructures;
using GabrielBertasso.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.Events
{
    public class EventsInvoker : MonoBehaviour
    {
        [SerializeField] private bool _canInvoke = true;
        [SerializeField] private List<KeyValue<string, UnityEvent>> _events;
        [SerializeField] private int _frameInterval;

        public bool CanInvoke
        {
            get => _canInvoke;
            set => _canInvoke = value;
        }


        [Button]
        public void InvokeEvent(string key)
        {
            if (!_canInvoke)
            {
                return;
            }

            if (_events == null || !_events.TryGetValues(key, out List<UnityEvent> events))
            {
                Debug.LogWarning("UnityEvent with key \"" + key + "\" not found!", this);
                return;
            }

            foreach (UnityEvent @event in events)
            {
                try
                {
                    @event?.Invoke();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }
        }

        [Button]
        public void InvokeAllEvents()
        {
            InvokeAllEvents(_frameInterval);
        }

        public void InvokeAllEvents(int frameInterval)
        {
            if (!_canInvoke || _events == null)
            {
                return;
            }

            if (frameInterval > 0)
            {
                StartCoroutine(InvokeAllEventsRoutine(frameInterval));
                return;
            }

            foreach (KeyValue<string, UnityEvent> @event in _events)
            {
                try
                {
                    @event.Value?.Invoke();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }
        }

        private IEnumerator InvokeAllEventsRoutine(int frameInterval)
        {
            if (!_canInvoke || _events == null)
            {
                yield break;
            }

            foreach (KeyValue<string, UnityEvent> @event in _events)
            {
                try
                {
                    @event.Value?.Invoke();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }

                for (int i = 0; i < frameInterval; i++)
                {
                    yield return null;
                }
            }
        }
    }
}