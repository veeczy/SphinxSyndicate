using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public bool onTrigger = false;
    public string sceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && onTrigger)
        changeScene(sceneName);
    }
    void changeScene(string index)
    {
        SceneManager.LoadScene(index);
    }
}
