using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public float camHeight = 6.2f;
    public float camDist = 5.8f;
    
    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 1f;
    public bool LookAtPlayer = true;

    private Transform localPlayer;
    private Vector3 cameraOffset;

    private void Start() 
    {
        StartCoroutine(GetLocalPlayer());
    }
    
	private void LateUpdate() 
    {
        if(localPlayer == null || !LookAtPlayer) return;

        Vector3 newPos = localPlayer.position + cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        transform.LookAt(localPlayer);
	}

    private IEnumerator GetLocalPlayer()
    {
        yield return new WaitUntil(() => PlayerManager.instance.players.ContainsKey(Client.instance.myId));

        localPlayer = PlayerManager.instance.players[Client.instance.myId].transform;
        transform.position = new Vector3(localPlayer.position.x, localPlayer.position.y + camHeight, localPlayer.position.z - camDist);
        cameraOffset = transform.position - localPlayer.position;
    }
}
