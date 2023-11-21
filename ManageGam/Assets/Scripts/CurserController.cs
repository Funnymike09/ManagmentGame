using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurserController : MonoBehaviour
{
    public Texture2D cursor;
    public Texture2D cursorClicked;

    private void Awake()
    {
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }
    public void OnMouseEnter()
    {
        ChangeCursor(cursorClicked);
    }

    public void OnMouseExit()
    {
        ChangeCursor(cursor);
    }
}
