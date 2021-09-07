using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FindMeOut.Lobby
{
    public enum CanvasType
    {
        VoteMap,
        Wardrobe
    }

    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager instance;

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
            for(int i = 0; i < canvasControllers.Length; i++)
            {
                canvasControllers[i].EnabledCanvas(false);
            }
        }

        public void OpenCanvas(CanvasType _canvasType, bool _value)
        {   
            for(int i = 0; i < canvasControllers.Length; i++)
            {
                if(canvasControllers[i].type == _canvasType)
                {
                    canvasControllers[i].EnabledCanvas(_value);
                    return;
                }
            }
            Debug.Log("Cannot find desired canvas");
        }
    }
}