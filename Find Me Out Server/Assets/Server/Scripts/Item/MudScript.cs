using UnityEngine;

public class MudScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hider")
        {
            other.gameObject.GetComponent<Player>().speed = 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hider")
        {
            Player _player = other.gameObject.GetComponent<Player>();
            _player.speed = 4;
            ServerSend.SendFootprintData(_player.id);
        }
    }
}
