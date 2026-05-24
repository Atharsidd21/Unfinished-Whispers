using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;
    public string targetSpawnID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoader.LoadScene(
                targetScene,
                targetSpawnID
            );
        }
    }
}