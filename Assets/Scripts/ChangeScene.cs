using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public bool onTrigger = false;
    public int sceneIndex = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(onTrigger)
        changeScene(sceneIndex);
    }
    void changeScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
