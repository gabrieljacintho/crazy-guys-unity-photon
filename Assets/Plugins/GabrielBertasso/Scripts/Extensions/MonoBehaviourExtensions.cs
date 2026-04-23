using System;
using System.Collections;
using UnityEngine;

namespace GabrielBertasso.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine DoOnNextFrame(this MonoBehaviour monoBehaviour, Action action)
        {
            IEnumerator Routine()
            {
                yield return null;
                action?.Invoke();
            }

            return monoBehaviour.StartCoroutine(Routine());
        }

        public static Coroutine DoAfterFrames(this MonoBehaviour monoBehaviour, Action action, int frames)
        {
            IEnumerator Routine()
            {
                for (int i = 0; i < frames; i++)
                {
                    yield return null;
                    action?.Invoke();
                }
            }

            return monoBehaviour.StartCoroutine(Routine());
        }

        public static Coroutine DoAfterSeconds(this MonoBehaviour monoBehaviour, Action action, float seconds, bool inRealtime = false)
        {
            IEnumerator Routine()
            {
                if (inRealtime)
                {
                    yield return new WaitForSecondsRealtime(seconds);
                }
                else
                {
                    yield return new WaitForSeconds(seconds);
                }

                action?.Invoke();
            }

            return monoBehaviour.StartCoroutine(Routine());
        }
    }
}