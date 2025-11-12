using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor options")]
    public Texture2D cursorNormal;
    public Texture2D cursorHover;

    private static CursorManager _instance;
    public static CursorManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Clickable()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Cursor.SetCursor(cursorHover, Vector2.zero, CursorMode.ForceSoftware);

#else
        Cursor.SetCursor(cursorHover, Vector2.zero, CursorMode.Auto);
#endif
    }

    public void Default()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.ForceSoftware);

#else
        Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.Auto);
#endif
    }
}
