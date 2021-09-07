using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUsername : MonoBehaviour
{
    private Transform player;
    private float yPos;

    private void Start()
    {
        yPos = transform.localPosition.y;
        
        //get player transform and unchild from it
        player = transform.parent;
        transform.SetParent(PlayerManager.instance.transform);
    }

    public void LateUpdate()
    {
        transform.position = player.position + new Vector3(0, yPos, 0);
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, 0, 0);
    }
}
