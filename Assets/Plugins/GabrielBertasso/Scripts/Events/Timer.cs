using System.Collections;
using GabrielBertasso.GameManagement;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GabrielBertasso.Events
{
    public class Timer : MonoBehaviour
    {
        [HideIf("_random")]
        [SerializeField] private FloatReference _duration = new FloatReference(1f);
        [ShowIf("_random")]
        [SerializeField] private FloatReference _minDuration = new FloatReference(1f);
        [ShowIf("_random")]
        [SerializeField] private FloatReference _maxDuration = new FloatReference(2f);
        [SerializeField] private bool _random;
        [SerializeField] private bool _loop;
        [FormerlySerializedAs("_useUnscaledDeltaTime")]
        [SerializeField] private bool _ignoreTimeScale;
        [SerializeField] private bool _onlyInGame;
        [SerializeField] private bool _playOnEnable;

        private Coroutine _coroutine;

        [Space]
        public UnityEvent OnEnd;


        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        [Button]
        public void Play()
        {
            Stop();
            _coroutine = StartCoroutine(Routine());
        }

        [Button]
        public void Stop()
        {
            if (_coroutine == null)
            {
                return;
            }

            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private IEnumerator Routine()
        {
            float t = 0f;
            float duration = GetDuration();

            while (true)
            {
                if (_onlyInGame && !GameManager.InAnyGameState)
                {
                    yield return null;
                    continue;
                }

                if (t >= duration)
                {
                    OnEnd?.Invoke();

                    if (_loop)
                    {
                        t -= duration;
                        duration = GetDuration();
                    }
                    else
                    {
                        _coroutine = null;
                        yield break;
                    }
                }

                yield return null;
                t += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }

        private float GetDuration()
        {
            return _random ? UnityEngine.Random.Range(_minDuration.Value, _maxDuration.Value) : _duration.Value;
        }
    }
}