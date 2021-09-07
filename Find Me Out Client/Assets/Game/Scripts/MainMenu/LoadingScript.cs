using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    public static LoadingScript instance;

    [Header("Loading")]
    public Image slider;
    public string sceneName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void LoadingStart()
    {
        GetComponent<Canvas>().enabled = true;
        GetComponent<Animator>().SetTrigger("start");
    }

    private IEnumerator LoadAsyncScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.fillAmount = progress;

            yield return null;
        }
    }
}
