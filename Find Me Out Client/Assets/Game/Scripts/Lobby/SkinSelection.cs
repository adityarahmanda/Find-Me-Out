using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelection : MonoBehaviour
{
    [HideInInspector]
    public Button button;
    
    [HideInInspector]
    public Image image;
    
    private void Awake() {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }
}
