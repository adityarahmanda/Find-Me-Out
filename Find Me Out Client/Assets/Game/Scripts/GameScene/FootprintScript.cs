using System.Collections;
using UnityEngine;

public class FootprintScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(AutoRemoveFootprint());
    }

    public IEnumerator AutoRemoveFootprint()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }
}
