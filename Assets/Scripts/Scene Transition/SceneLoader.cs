using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public static string targetSpawnID;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;

    public float fadeDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint sp in spawnPoints)
        {
            if (sp.spawnID == targetSpawnID)
            {
                GameObject player =
                    GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    player.transform.position =
                        sp.transform.position;
                }

                break;
            }
        }
    }

    public static void LoadScene(
        string sceneName,
        string spawnID
    )
    {
        targetSpawnID = spawnID;

        Instance.StartCoroutine(
            Instance.Transition(sceneName)
        );
    }

    IEnumerator Transition(string sceneName)
    {
        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut()
    {
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            fadeCanvas.alpha =
                Mathf.Lerp(0, 1, time / fadeDuration);

            yield return null;
        }

        fadeCanvas.alpha = 1;
    }

    IEnumerator FadeIn()
    {
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            fadeCanvas.alpha =
                Mathf.Lerp(1, 0, time / fadeDuration);

            yield return null;
        }

        fadeCanvas.alpha = 0;
    }
}