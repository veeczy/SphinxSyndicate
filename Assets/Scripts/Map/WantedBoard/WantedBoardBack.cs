using UnityEngine;
using UnityEngine.SceneManagement;

public class WantedBoardBack : MonoBehaviour
{
    public void GoBack()
    {
        if (PlayerPrefs.HasKey("PreviousScene"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("PreviousScene"));
        }
        else
        {
            Debug.LogWarning("PreviousScene not found!");
        }
    }
}

