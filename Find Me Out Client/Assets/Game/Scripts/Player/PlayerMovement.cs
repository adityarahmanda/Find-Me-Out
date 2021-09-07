using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private void Awake() 
    {
        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if(player.isCaught) return;
        
        SendInputToServer();
    }

    public void SendInputToServer()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        
        float inputMagnitude = new Vector2(inputX, inputZ).sqrMagnitude;

        if(inputMagnitude > 0.1f)
        {
            ClientSend.PlayerInput(inputX, inputZ);
        }
        else if(player.IsWalk)
        {        
            ClientSend.PlayerInput(0, 0);
        }
    }
}
