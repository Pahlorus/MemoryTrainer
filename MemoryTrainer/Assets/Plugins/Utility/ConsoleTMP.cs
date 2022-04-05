using System.Collections.Generic;
using System.Text;

using TMPro;

using UnityEngine;

using Utility.Attributes;

namespace Utility
{

    public class ConsoleTMP : MonoBehaviour
    {
        private static ConsoleTMP _instance;
        private const int _SIZE = 20;

        [SerializeField, Require] private TextMeshPro _text;
        private StringBuilder _stringBuilder;
        private Queue<string> _buffer;

        private void Awake()
        {
            _instance = this;
            gameObject.SetActive(false);
            _stringBuilder = new StringBuilder();
            _buffer = new Queue<string>(_SIZE);
        }

        private void Add_Internal(string text)
        {
            gameObject.SetActive(true);
            if (_buffer.Count > _SIZE)
                _buffer.Dequeue();
            _buffer.Enqueue(text);
            SetText();
        }

        private void SetText()
        {
            foreach (var s in _buffer)
                _stringBuilder.AppendLine(s);
            _text.text = _stringBuilder.ToString();
            _stringBuilder.Clear();
        }


        public static void Add(string text)
        {
            if (_instance != null)
                _instance.Add_Internal(text);
        }
    }
}