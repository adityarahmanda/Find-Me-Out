
using UnityEngine;

namespace FindMeOut.Lobby
{
    public class CanvasController : MonoBehaviour
    {
        private Canvas canvas;
        private bool isEnabled = false;

        public CanvasType type;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        public void EnabledCanvas(bool _value)
        {
            if(_value != isEnabled)
            {
                canvas.enabled = _value;
                isEnabled = _value;
            }
        }
    }
}