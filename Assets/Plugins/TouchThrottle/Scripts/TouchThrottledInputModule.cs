using UnityEngine;
using UnityEngine.EventSystems;

namespace TouchThrottle
{
    public class TouchThrottledInputModule : StandaloneInputModule
    {
        [SerializeField] private TouchThrottledParameter _parameter = default;

        private TouchThrottledController _controller;

        protected override void Awake()
        {
            base.Awake();
            _controller = GetOrAddComponent<TouchThrottledController>(gameObject);
            _controller.RegisterParameter(_parameter);
        }

        public void ToggleThrottling(bool isOn)
        {
            _controller.enabled = isOn;
        }

        public override void Process()
        {
            if (_controller.ThrottlingEnabled && ProcessTouchEvents())
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
                if (pressed && !_controller.TouchAllowed)
                {
                    ProcessTouchPress(pointerEventData, false, released); // 押してないことにする
                }
                else
                {
                    ProcessTouchPress(pointerEventData, pressed, released);
                    if (pressed) _controller.OnTouchPressed();
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

        private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}