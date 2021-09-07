using System.Collections;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyItem());
    }

    private IEnumerator DestroyItem()
    {
        yield return new WaitForSeconds(6);
        DestroyObj();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Seeker")
        {
            DestroyObj();
        }
    }

    public void DestroyObj()
    {
        Destroy(this.gameObject);
        GameSceneManager.instance.itemSpawned--;
    }
}
