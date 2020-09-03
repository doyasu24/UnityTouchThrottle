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

        // see https://docs.unity3d.com/2019.4/Documentation/ScriptReference/UIElements.PointerEventBase_1-pointerId.html
        private int? _handingPointerId = null;

        private bool ProcessTouchEvents()
        {
            for (var index = 0; index < input.touchCount; ++index)
            {
                var touch = input.GetTouch(index);
                if (touch.type == TouchType.Indirect) continue;
                var pointerEventData = GetTouchPointerEventData(touch, out var pressed, out var released);

                if (_handingPointerId.HasValue)
                {
                    if (_handingPointerId.Value != pointerEventData.pointerId) continue;

                    if (released)
                    {
                        _handingPointerId = null;
                        _controller.OnTouchReleased();
                    }
                }
                else
                {
                    if (pressed && _controller.TouchAllowed)
                    {
                        _handingPointerId = pointerEventData.pointerId;
                    }
                    else
                    {
                        continue;
                    }
                }

                ProcessTouchPress(pointerEventData, pressed, released);

                if (!released)
                {
                    ProcessMove(pointerEventData);
                    ProcessDrag(pointerEventData);
                }
                else
                    RemovePointerData(pointerEventData);
            }

            return input.touchCount > 0;
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