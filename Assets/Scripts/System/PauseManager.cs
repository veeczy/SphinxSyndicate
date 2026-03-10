using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    private GameObject pauseMenuUI;
    private bool isPaused = false;

    private string lastGameplayScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Store gameplay scenes so Back button knows where to return
        if (scene.name != "Settings" && scene.name != "HowToPlay" && scene.name != "MainMenu")
        {
            lastGameplayScene = scene.name;
        }

        pauseMenuUI = FindInactiveObjectByName("PausedCanvas");

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);

            Button[] buttons = pauseMenuUI.GetComponentsInChildren<Button>(true);

            foreach (Button btn in buttons)
            {
                btn.onClick.RemoveAllListeners();

                if (btn.name.ToLower().Contains("resume"))
                    btn.onClick.AddListener(Resume);

                else if (btn.name.ToLower().Contains("retry"))
                    btn.onClick.AddListener(RetryLevel);

                else if (btn.name.ToLower().Contains("quit"))
                    btn.onClick.AddListener(QuitToMenu);

                else if (btn.name.ToLower().Contains("howtoplay"))
                    btn.onClick.AddListener(OpenHowToPlay);

                else if (btn.name.ToLower().Contains("settings"))
                    btn.onClick.AddListener(OpenSettings);
            }
        }

        // Back buttons in other scenes
        Button[] allButtons = FindObjectsOfType<Button>(true);

        foreach (Button btn in allButtons)
        {
            if (btn.name.ToLower().Contains("back"))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(BackToGame);
            }
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        if (pauseMenuUI == null) return;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RetryLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenHowToPlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HowToPlay");
    }

    public void OpenSettings()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Settings");
    }

    public void BackToGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(lastGameplayScene);
    }

    private GameObject FindInactiveObjectByName(string name)
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in objects)
        {
            if (obj.name == name && obj.scene.isLoaded)
                return obj;
        }

        return null;
    }
}