using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TouchThrottle.Samples
{
    public class TouchThrottleSample : MonoBehaviour
    {
        [SerializeField] private Button _button1;
        [SerializeField] private Button _button2;
        [SerializeField] private Text _logText;
        [SerializeField] private Button _toggleButton;

        [SerializeField] private TouchThrottledInputModule _inputModule;

        private readonly Queue<string> _logs = new Queue<string>();

        private bool _isOn = true;

        private Text _toggleText;

        private bool _button1Pressed;
        private bool _button2Pressed;

        private void Start()
        {
            _toggleText = _toggleButton.GetComponentInChildren<Text>();

            OnIsOnUpdated();
            _toggleButton.onClick.AddListener(() =>
            {
                _isOn = !_isOn;
                OnIsOnUpdated();
            });

            _button1.onClick.AddListener(() => _button1Pressed = true);
            _button2.onClick.AddListener(() => _button2Pressed = true);

            StartCoroutine(EndOfFrameCoroutine());
        }

        private IEnumerator EndOfFrameCoroutine()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (_button1Pressed && _button2Pressed)
                {
                    _logs.Enqueue("button1 and 2 clicked");
                }
                else if (_button1Pressed)
                {
                    _logs.Enqueue("button1 clicked");
                }
                else if (_button2Pressed)
                {
                    _logs.Enqueue("button2 clicked");
                }

                if (_logs.Count > 9) _logs.Dequeue();
                _logText.text = string.Join(Environment.NewLine, _logs);

                _button1Pressed = false;
                _button2Pressed = false;
            }
        }

        private void OnIsOnUpdated()
        {
            _inputModule.ToggleThrottling(_isOn);
            _toggleText.text = _isOn ? "Enabled" : "Disabled";
        }
    }
}