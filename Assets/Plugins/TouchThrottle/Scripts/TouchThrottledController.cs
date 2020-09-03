using System.Collections;
using UnityEngine;

namespace TouchThrottle
{
    public class TouchThrottledController : MonoBehaviour
    {
        // cache
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

        private TouchThrottledParameter _parameter;

        public void RegisterParameter(TouchThrottledParameter parameter)
        {
            _parameter = parameter;
        }

        private float _previousTouchedTimeInSeconds;
        private int _previousTouchedFrame;
        private int _currentTouchedCount;

        public void OnTouchReleased()
        {
            _previousTouchedTimeInSeconds = _parameter.GetCurrentTimeInSeconds();
            _previousTouchedFrame = Time.frameCount;
            _currentTouchedCount += 1;
        }
        
        public bool ThrottlingEnabled => enabled;

        public bool TouchAllowed => !enabled || _currentTouchedCount <= 0;

        private void Awake()
        {
            StartCoroutine(ClearCountCoroutine());
        }

        private void OnEnable()
        {
            Reset();
        }

        private void OnDisable()
        {
            Reset();
        }

        private void Reset()
        {
            _previousTouchedTimeInSeconds = 0;
            _previousTouchedFrame = 0;
            _currentTouchedCount = 0;
        }

        private IEnumerator ClearCountCoroutine()
        {
            while (true)
            {
                yield return WaitForEndOfFrame;
                if (TouchAllowed) continue;

                var currentTime = _parameter.GetCurrentTimeInSeconds();
                var diffTime = currentTime - _previousTouchedTimeInSeconds;
                if (diffTime < _parameter.ThrottleTimeInSeconds) continue;
                var diffFrame = Time.frameCount - _previousTouchedFrame;
                if (diffFrame < _parameter.ThrottleFrame) continue;
                Reset();
            }
        }
    }
}