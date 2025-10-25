using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    public static SceneManagerController Instance;   // defines this script as a singleton and persistant
    public GameObject player;
    public string startSceneName = "SampleScene"; // Set your start scene name here

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  // <-- actions the singleton so evrything else can see it
            DontDestroyOnLoad(gameObject); // Optional - failsafe to stop this iobject being destroyed when loading scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManagerController.Instance.LoadSceneAdditive("Scene_01");  // load Scene_01 when level starts
    }

    // follwing method is called from the NextLevelBox script to load the scene with the supplied name 
    public void LoadSceneAdditive(string sceneName, string spawnPointName = "SpawnPoint")
    {
        StartCoroutine(LoadSceneAndMovePlayer(sceneName, spawnPointName));
    }


    // the following coroutine loads the new scene and uses a few pauses (yield return null) to ensure everything is loaded
    // before finding the new spawnpoint and moving the player to it, it has a few failsafes in it to ensure the player is found
    // also the players character controller / rigidbody (it will check for either) will be disabled to prevent any movement 
    // until the player has been moved, then finally it will unload the previous level

    private IEnumerator LoadSceneAndMovePlayer(string sceneName, string spawnPointName)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // Load the new scene additively
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Activate the new scene
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        if (newScene.IsValid())
        {
            SceneManager.SetActiveScene(newScene);
        }

        // Wait a couple of frames to ensure everything in the new scene is initialized
        yield return null;
        yield return null;

        // Find the spawn point within the new scene
        GameObject spawnPoint = null;
        foreach (GameObject rootObj in newScene.GetRootGameObjects())
        {
            if (rootObj.name == spawnPointName)
            {
                spawnPoint = rootObj;
                break;
            }

            Transform found = rootObj.transform.Find(spawnPointName);
            if (found != null)
            {
                spawnPoint = found.gameObject;
                break;
            }
        }

        // Fallback to global search if not found
        if (spawnPoint == null)
            spawnPoint = GameObject.Find(spawnPointName);

        // Move player safely to spawn point
        if (spawnPoint != null && player != null)
        {
            var movement = player.GetComponent<FPMovement>();
            var cc = player.GetComponent<CharacterController>();
            var rb = player.GetComponent<Rigidbody>();

            // Temporarily disable movement and physics
            if (movement != null) movement.enabled = false;
            if (cc != null) cc.enabled = false;
            if (rb != null) rb.isKinematic = true;

            // Reposition player
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;

            // Wait one frame before re-enabling
            yield return null;

            if (rb != null) rb.isKinematic = false;
            if (cc != null) cc.enabled = true;
            if (movement != null) movement.enabled = true;
        }

        // Unload the previous scene if it's not the persistent one
        if (currentScene.name != startSceneName)
        {
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }
    }

}

