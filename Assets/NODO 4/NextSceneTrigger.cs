using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    public string sceneName; // The name of the scene to load

    // This is triggered when something enters the 2D trigger collider
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the object has the "Player" tag
        {
            SceneManager.LoadScene(sceneName); // Load the specified scene
        }
    }
}
