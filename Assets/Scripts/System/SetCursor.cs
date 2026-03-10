using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public static SetCursor Instance;
    public Transform cursorPrefab;
    public Canvas cursorCanvasPrefab;
    private Canvas cursorCanvas;
    private Transform cursorObj;

    void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        cursorCanvas = Instantiate(cursorCanvasPrefab, transform.position, transform.rotation);
        cursorObj = Instantiate(cursorPrefab, cursorCanvas.transform);
        Instance = this;
        DontDestroyOnLoad(cursorObj);
    }
    public void SetCrosshair(Vector2 pos)
    {
        RectTransform rt = cursorObj.GetComponent<RectTransform>();
        rt.position = Camera.main.WorldToScreenPoint(pos);
    }

}
