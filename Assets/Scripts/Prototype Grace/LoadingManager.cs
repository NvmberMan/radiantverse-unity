using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loading : MonoBehaviour
{
    public GameObject LoadingPanel;
    public Slider slider;

    public void LoadLevel(int scenceIndex)
    {
        StartCoroutine(LoadAsynchronously(scenceIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        LoadingPanel.SetActive(true);

        while (operation.isDone == false)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;

            yield return null;
        }

    }
}
