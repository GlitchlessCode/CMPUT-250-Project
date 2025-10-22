using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundSwapper : MonoBehaviour
{
    public Image backgroundImage;  // assign your UI Image here
    public Sprite sprite1;
    public Sprite sprite2;
    public float swapInterval = 3f;  // seconds between swaps

    private bool showingSprite1 = true;

    void Start()
    {
        // Set starting image
        backgroundImage.sprite = sprite1;
        StartCoroutine(SwapRoutine());
    }

    IEnumerator SwapRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(swapInterval);

            showingSprite1 = !showingSprite1;
            backgroundImage.sprite = showingSprite1 ? sprite1 : sprite2;
        }
    }
}
