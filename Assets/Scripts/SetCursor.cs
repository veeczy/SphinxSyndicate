using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public bool autoCenterHotSpot = false;
    private float textureWidth;
    private float textureHeight;
    private Vector2 hotspotAuto;

    void Start()
    {
        textureWidth = cursorTexture.width * 0.5f;
        textureHeight = cursorTexture.height * 0.5f;

        if(autoCenterHotSpot)
        {
            hotspotAuto = new Vector2 (textureWidth, textureHeight);
        }
        else { hotspotAuto = hotSpot; }
        
        Cursor.SetCursor(cursorTexture, hotspotAuto, cursorMode);
    }
    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotspotAuto, cursorMode);
    }

    void OnMouseExit()
    {
        // Pass 'null' to the texture parameter to use the default system cursor.
        //Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
