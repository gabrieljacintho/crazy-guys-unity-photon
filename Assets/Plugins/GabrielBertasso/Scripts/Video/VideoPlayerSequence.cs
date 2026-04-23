using System;
using System.Collections.Generic;
#if FMOD
using GabrielBertasso.FMODIntegration;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace GabrielBertasso.Video
{
    public class VideoPlayerSequence : MonoBehaviour
    {
        [Serializable]
        public struct Video
        {
            public VideoPlayer VideoPlayer;
#if FMOD
            public FMODAudioSource AudioSource;
#else
            public AudioSource AudioSource;
#endif
        }

        [SerializeField] private List<Video> _videos;

        private int _currentIndex;

        [Space]
        public UnityEvent OnEnd;


        private void Start()
        {
#if UNITY_STANDALONE_WIN
            Play();
#else
            OnEnd?.Invoke();
#endif
        }

        public void Play()
        {
            _currentIndex = 0;
            Play(_videos[_currentIndex]);
        }

        private void Play(Video video)
        {
            video.VideoPlayer.gameObject.SetActive(true);

            if (video.VideoPlayer.isPrepared)
            {
                VideoPlayer_prepareCompleted(video.VideoPlayer);
            }
            else
            {
                video.VideoPlayer.Prepare();
                video.VideoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
            }

            video.VideoPlayer.loopPointReached += VideoPlayer_loopPointReached;
        }

        private void VideoPlayer_prepareCompleted(VideoPlayer video)
        {
            video.prepareCompleted -= VideoPlayer_prepareCompleted;

            video.Play();
            _videos.Find(v => v.VideoPlayer == video).AudioSource.Play();
        }

        private void VideoPlayer_loopPointReached(VideoPlayer video)
        {
            video.gameObject.SetActive(false);
            video.loopPointReached -= VideoPlayer_loopPointReached;

            _currentIndex++;
            if (_currentIndex < _videos.Count)
            {
                Play(_videos[_currentIndex]);
            }
            else
            {
                OnEnd?.Invoke();
            }
        }
    }
}