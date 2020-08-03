using System;
using UnityEngine;

namespace TouchThrottle
{
    [Serializable]
    public class TouchThrottledParameter
    {
        [SerializeField] private int _throttleFrame = 3;
        public int ThrottleFrame => _throttleFrame;

        [SerializeField] private float _throttleTimeInSeconds = 0.15f;
        public float ThrottleTimeInSeconds => _throttleTimeInSeconds;

        [SerializeField] private bool _countUnscaledTime = false;
        public bool CountUnscaledTime => _countUnscaledTime;

        public float GetCurrentTimeInSeconds()
        {
            return CountUnscaledTime ? Time.unscaledTime : Time.time;
        }
    }
}