using System.Collections;
using System.Collections.Generic;
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
        DestryObj();
    }

    public void DestryObj()
    {
        Instantiate(PlayerManager.instance.pumkinParticle, transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity);
        Destroy(this.gameObject);
    }
}
