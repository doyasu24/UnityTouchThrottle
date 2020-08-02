using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TouchThrottle
{
    public class TouchThrottledInputModule : StandaloneInputModule
    {
        // cache
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

        [SerializeField] private int _numberOfTouchesAllowedAtTheSameTime = 1;
        [SerializeField] private float _throttleTimeInSeconds = 0.15f;
        [SerializeField] private bool _countUnscaledTime = false;

        private bool _throttlingEnabled = true;

        private float GetCurrentTimeInSeconds()
        {
            return _countUnscaledTime ? Time.unscaledTime : Time.time;
        }

        private int _currentTouchedCount;

        private bool TouchAllowed => _numberOfTouchesAllowedAtTheSameTime > _currentTouchedCount;

        public void ToggleThrottling(bool isOn)
        {
            if (isOn)
            {
                _throttlingEnabled = true;
            }
            else
            {
                _throttlingEnabled = false;
                _currentTouchedCount = 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(ClearCountCoroutine());
        }

        private IEnumerator ClearCountCoroutine()
        {
            var previousTime = GetCurrentTimeInSeconds();
            while (true)
            {
                yield return WaitForEndOfFrame;
                if (_throttlingEnabled && _currentTouchedCount > 0)
                {
                    var currentTime = GetCurrentTimeInSeconds();
                    var diff = currentTime - previousTime;
                    if (diff > _throttleTimeInSeconds)
                    {
                        _currentTouchedCount = 0;
                        previousTime = currentTime;
                    }
                }
            }
        }

        public override void Process()
        {
            if (_throttlingEnabled && ProcessTouchEvents())
            {
                return;
            }

            base.Process();
        }

        private bool ProcessTouchEvents()
        {
            var result = input.touchCount > 0;
            for (var index = 0; index < input.touchCount; ++index)
            {
                var touch = input.GetTouch(index);
                if (touch.type == TouchType.Indirect) continue;
                var pointerEventData = GetTouchPointerEventData(touch, out var pressed, out var released);
                if (pressed && !TouchAllowed)
                {
                    ProcessTouchPress(pointerEventData, false, released); // 押してないことにする
                }
                else
                {
                    ProcessTouchPress(pointerEventData, pressed, released);
                    if (pressed) _currentTouchedCount += 1;
                }

                if (!released)
                {
                    ProcessMove(pointerEventData);
                    ProcessDrag(pointerEventData);
                }
                else
                    RemovePointerData(pointerEventData);
            }

            return result;
        }
    }
}