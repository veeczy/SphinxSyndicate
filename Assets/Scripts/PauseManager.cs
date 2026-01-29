using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Scene to Load When Retry is Pressed")]
    public string retrySceneName = "Level1";

    private GameObject pauseMenuUI;
    public GameObject howtoPlayUI; //added for the how to play button
    public GameObject pausemenutext;
    public GameObject buttons; // parent object containing HowToPlay + RetryQuitButtons
    public GameObject retryquitbutton;
    public bool ishowtoplay = false;
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
                btn.onClick.RemoveAllListeners();

                if (btn.name.ToLower().Contains("resume"))
                    btn.onClick.AddListener(Resume);
                else if (btn.name.ToLower().Contains("retry"))
                    btn.onClick.AddListener(RetryLevel);
                else if (btn.name.ToLower().Contains("quit"))
                    btn.onClick.AddListener(QuitGame);
                else if (btn.name.ToLower().Contains("howtoplay"))
                    btn.onClick.AddListener(HowToPlay);
                else if (btn.name.ToLower().Contains("back"))
                    btn.onClick.AddListener(BackToPause);  // This wires the back button
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
            if (isPaused)
            {
                if (ishowtoplay)      // check if HowToPlay panel is active
                    BackToPause();    // go back to main pause menu
                else
                    Resume();          // otherwise, resume the game
            }
            else
            {
                Pause();
            }
        }
    }


    public void Pause()
    {
        if (pauseMenuUI == null)
        {
            Debug.LogError("[PauseManager] Cannot pause — PAUSED UI not found!");
            return;
        }

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("[PauseManager] Paused");
    }
    //Function for back button in How To Play Panel
    public void BackToPause()
    {
        ishowtoplay = false;

        howtoPlayUI.SetActive(false);
        pausemenutext.SetActive(true);
        buttons.SetActive(true);
    }

    //Function to open how to play screen
    public void HowToPlay()
    {
        ishowtoplay = true;

        pausemenutext.SetActive(false);
        buttons.SetActive(false);
        howtoPlayUI.SetActive(true);
    }
    public void Resume()
    {
        if (pauseMenuUI == null)
        {
            Debug.LogError("[PauseManager] Cannot resume — PAUSED UI not found!");
            return;
        }

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("[PauseManager] Resumed");
    }
    //Altered the retry button to work for all scenes
    public void RetryLevel()
    {
        var currentScene = SceneManager.GetActiveScene();
        Debug.Log("[PauseManager] Retry pressed. Reloading current scene: " + currentScene.name);

        Time.timeScale = 1f;
        isPaused = false;

        // Reload the scene you are currently in
        SceneManager.LoadScene(currentScene.buildIndex);
        // (Alternative: SceneManager.LoadScene(currentScene.name);)
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