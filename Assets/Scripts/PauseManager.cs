using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; 
    private bool isPaused = false;

    void Awake()
    {
        
        DontDestroyOnLoad(gameObject);

       
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}