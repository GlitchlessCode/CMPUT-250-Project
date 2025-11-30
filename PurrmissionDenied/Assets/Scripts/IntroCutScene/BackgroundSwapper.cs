using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSwapper : MonoBehaviour
{
    [Header("Image")]
    public Image backgroundImage; // assign your UI Image here

    [Header("Sprites")]
    public Sprite sprite1;
    public Sprite sprite2;

    [Header("Swap Interval")]
    public float swapInterval = 3f; // seconds between swaps

    [Header("Panel")]
    public GameObject PowerOn;

    private bool showingSprite1 = true;

    void Start()
    {
        // Set starting image
        backgroundImage.sprite = sprite1;
        StartCoroutine(PowerOnDelayed());
        StartCoroutine(SwapRoutine());
    }

    IEnumerator PowerOnDelayed()
    {
        yield return new WaitForSeconds(0.2f);
        PowerOn.GetComponent<Animator>().SetTrigger("PlayPowerOn");
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
