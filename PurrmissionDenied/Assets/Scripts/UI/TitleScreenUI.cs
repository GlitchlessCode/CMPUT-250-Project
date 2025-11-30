using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject PowerOff;

    [Header("Buttons")]
    public Button StartButton;

    [Header("Audio")]
    public Audio ShutdownAudio;

    [Header("Events")]
    public AudioGameEvent AudioBus;

    // Start is called before the first frame update
    void Start()
    {
        PowerOff.SetActive(false);
        StartButton.onClick.AddListener(OnStartButton);
    }

    private void OnStartButton()
    {
        StartCoroutine(Shutdown());
    }

    private IEnumerator Shutdown()
    {
        yield return new WaitForSeconds(0.15f);
        if (ShutdownAudio.clip != null)
        {
            AudioBus?.Emit(ShutdownAudio);
        }
        PowerOff.SetActive(true);
        PowerOff.GetComponent<Animator>().SetTrigger("PlayPowerOff");
    }

    private void Update()
    {
        if (StartButton != null && StartButton.gameObject.activeSelf && StartButton.interactable)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // Visually simulate the button press
                StartCoroutine(SimulateButtonPress());
            }
        }
    }

    private IEnumerator SimulateButtonPress()
    {
        var btn = StartButton;
        if (btn == null)
            yield break;

        // Ensure there’s an EventSystem (your scene should already have one;
        // but this makes it robust if it’s missing).
        if (EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            // Optionally: yield return null; // give it a frame to initialize
        }

        // Create a fake left-click event
        var ped = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left,
            clickCount = 1,
        };

        // Visually go to Pressed state
        ExecuteEvents.Execute(btn.gameObject, ped, ExecuteEvents.pointerDownHandler);

        // Brief pressed time so the sprite/transition is visible
        yield return new WaitForSeconds(0.1f);

        // Release and invoke the click
        ExecuteEvents.Execute(btn.gameObject, ped, ExecuteEvents.pointerUpHandler);
        btn.onClick.Invoke();
    }
}
