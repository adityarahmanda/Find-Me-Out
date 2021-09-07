using UnityEngine;

namespace FindMeOut.MainMenu
{
    public enum CanvasType
    {
        Connection,
        Login,
        SignUp,
        Dashboard,
        Credit,
        Setting
    }

    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager instance;

        public CanvasType defaultCanvas;
        private CanvasController lastActiveCanvas;

        private CanvasController[] canvasControllers;

        
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
            
            canvasControllers = GetComponentsInChildren<CanvasController>();
        }

        private void Start()
        {
            foreach(CanvasController _canvas in canvasControllers)
            {
                if(_canvas.type != defaultCanvas) 
                {
                    _canvas.EnabledCanvas(false);
                } 
                else 
                {
                    _canvas.EnabledCanvas(true);
                    lastActiveCanvas = _canvas;
                }
            }
        }

        public void SwitchCanvas(CanvasType _canvasType)
        {
            CanvasController desiredCanvas = FindCanvasController(_canvasType);
            
            if(desiredCanvas != null) 
            {
                lastActiveCanvas.EnabledCanvas(false);
                desiredCanvas.EnabledCanvas(true);

                lastActiveCanvas = desiredCanvas;
            }
            else 
            {
                Debug.Log("Cannot find desired canvas");
            }
        }

        public CanvasController FindCanvasController(CanvasType _canvasType)
        {
            for(int i = 0; i < canvasControllers.Length; i++)
            {
                if(canvasControllers[i].type == _canvasType) return canvasControllers[i];
            }

            return null;
        }
    }
}