
using UnityEngine;
using UnityEngine.SceneManagement;


public class NexttLevelBox : MonoBehaviour
{
    [Tooltip("Optional: manually specify the next scene name. If empty, will auto-increment based on current scene.")]
    public string nextSceneName = "";

    [Tooltip("Optional: name of the spawn point in the next scene.")]
    public string spawnPointName = "SpawnPoint";

    public void OnInteract()
    {
        Debug.Log($"[NextLevelBox] Interacted with {gameObject.name}");
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        // Use the manually assigned name if provided
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManagerController.Instance.LoadSceneAdditive(nextSceneName, spawnPointName);
            return;
        }

        // Otherwise, auto-increment the current scene name if NextSceneName left blank
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Expected format: "Scene_00", "Scene_01", etc.
        // This locic adds 1 to current name so Scene_02 will look for Scene_03 (02 + 1)
        string[] parts = currentSceneName.Split('_');
        if (parts.Length == 2 && int.TryParse(parts[1], out int sceneNumber))
        {
            int nextSceneNumber = sceneNumber + 1;
            string autoName = $"{parts[0]}_{nextSceneNumber:D2}";

            Debug.Log($"[NextLevelBox] Auto-loading next scene: {autoName}");
            SceneManagerController.Instance.LoadSceneAdditive(autoName, spawnPointName);
        }
        else
        {
            Debug.LogWarning($"[NextLevelBox] Scene name format not recognized: {currentSceneName}. Expected 'Scene_XX'.");
        }
    }
}
