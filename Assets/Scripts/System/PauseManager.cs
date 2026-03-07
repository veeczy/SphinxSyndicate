using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Scene to Load When Retry is Pressed")]
    public string retrySceneName = "Level1";

    private GameObject pauseMenuUI;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log("[PauseManager] Awake called");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the PAUSED UI in the new scene (active or inactive)
        pauseMenuUI = FindInactiveObjectByName("PAUSED");

        if (pauseMenuUI == null)
        {
            Debug.LogWarning("[PauseManager] No PAUSED UI found in scene: " + scene.name);
        }
        else
        {
            pauseMenuUI.SetActive(false);

            // Assign buttons dynamically every time scene loads
            Button[] buttons = pauseMenuUI.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                btn.onClick.RemoveAllListeners(); // remove old listeners

                if (btn.name.ToLower().Contains("resume"))
                    btn.onClick.AddListener(Resume);
                else if (btn.name.ToLower().Contains("retry"))
                    btn.onClick.AddListener(RetryLevel);
                else if (btn.name.ToLower().Contains("quit"))
                    btn.onClick.AddListener(QuitGame);
            }

            Debug.Log("[PauseManager] Buttons reassigned in scene: " + scene.name);
        }

        // Ensure EventSystem exists
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (pauseMenuUI == null)
        {
            Debug.LogError("[PauseManager] Cannot pause � PAUSED UI not found!");
            return;
        }

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("[PauseManager] Paused");
    }

    public void Resume()
    {
        if (pauseMenuUI == null)
        {
            Debug.LogError("[PauseManager] Cannot resume � PAUSED UI not found!");
            return;
        }

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("[PauseManager] Resumed");
    }

    public void RetryLevel()
    {
        Debug.Log("[PauseManager] Retry pressed. Reloading scene: " + retrySceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(retrySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("[PauseManager] Quit pressed.");

#if UNITY_EDITOR
        Debug.Log("[PauseManager] Application.Quit() would run in build.");
#else
        Application.Quit();
#endif
    }

    // Finds active or inactive object anywhere in scene
    private GameObject FindInactiveObjectByName(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name && obj.scene.isLoaded)
            {
                return obj;
            }
        }
        return null;
    }
}