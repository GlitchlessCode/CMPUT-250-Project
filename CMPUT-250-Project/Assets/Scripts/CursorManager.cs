using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Cursor options")]
    public Sprite cursorNormal;
    public Sprite cursorHover;


    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Cursor.SetCursor(SpriteToTexture2D(cursorHover), Vector2.zero, CursorMode.ForceSoftware);

#else
        Cursor.SetCursor(SpriteToTexture2D(cursorHover), Vector2.zero, CursorMode.Auto);
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Cursor.SetCursor(SpriteToTexture2D(cursorNormal), Vector2.zero, CursorMode.ForceSoftware);

#else
        Cursor.SetCursor(SpriteToTexture2D(cursorNormal), Vector2.zero, CursorMode.Auto);
#endif
    }

    public static Texture2D SpriteToTexture2D(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] pixels = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );
            newTexture.SetPixels(pixels);
            newTexture.Apply();
            return newTexture;
        }
        else
        {
            return sprite.texture;
        }
    }
}
