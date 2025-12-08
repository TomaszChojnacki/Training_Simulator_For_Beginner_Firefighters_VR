using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneLoaderWithProgress : MonoBehaviour
{
    [Header("UI ³adowania")]
    public GameObject panelLoading;
    public Slider loadingBar;
    public TMP_Text loadingText;

    [Header("Minimalny czas pokazania ekranu ³adowania")]
    public float minimalnyCzasLoadingu = 2.0f; // sekundy

    public void ZaladujSceneZProgress(int indexSceny)
    {
        panelLoading.SetActive(true);
        loadingBar.value = 0f;
        loadingText.text = "£adowanie... 0%";

        StartCoroutine(LoadSceneAsync(indexSceny));
    }

    private IEnumerator LoadSceneAsync(int indexSceny)
    {
        // Dajemy Unity chwilê, ¿eby UI siê narysowa³o
        yield return null; // 1 klatka
        yield return new WaitForEndOfFrame();

        float timer = 0f;

        AsyncOperation operacja = SceneManager.LoadSceneAsync(indexSceny);
        operacja.allowSceneActivation = false;

        while (!operacja.isDone)
        {
            timer += Time.deltaTime;

            // progress realny (0–0.9)
            float progress = Mathf.Clamp01(operacja.progress / 0.9f);

            // sztuczne spowolnienie – UI roœnie p³ynnie
            float smoothProgress = Mathf.Min(progress, timer / minimalnyCzasLoadingu);

            loadingBar.value = smoothProgress;
            loadingText.text = "£adowanie... " + Mathf.RoundToInt(smoothProgress * 100f) + "%";

            // Warunek przejœcia do sceny
            if (smoothProgress >= 1f && operacja.progress >= 0.9f)
            {
                operacja.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
