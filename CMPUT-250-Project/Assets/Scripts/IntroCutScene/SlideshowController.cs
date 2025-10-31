using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SlideshowController : MonoBehaviour
{
    [Header("Target UI")]
    public Image targetImage;

    [Header("Slides")]
    public List<Sprite> slides = new List<Sprite>();

    [Header("Start Options")]
    public int startIndex = 0;
    public bool wrapAround = true;

    [Header("Optional: UI Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Optional: Keyboard/Gamepad")]
    public KeyCode nextKey = KeyCode.RightArrow;
    public KeyCode prevKey = KeyCode.LeftArrow;
    public KeyCode alsoNextKey = KeyCode.Space;

    [Tooltip("If true, pressing Next on the last slide will fade+load the scene.")]
    public bool loadSceneAtEnd = false;

    [Header("Panels")]
    public GameObject PowerOff;

    [Header("Final Slide UI")]
    [Tooltip("Button shown only on the last slide.")]
    public Button beginButton; // assign your BeginButton

    [Header("Audio")]
    public Audio SlideSwitchAudio;
    public Audio ShutdownAudio;

    [Header("Events")]
    public AudioGameEvent AudioBus;

    private int _index = -1;
    private bool _isFading = false;

    void Awake()
    {
        if (targetImage == null)
        {
            Debug.LogError("[SlideshowController] No target Image assigned.");
            enabled = false;
            return;
        }
        if (slides == null || slides.Count == 0)
        {
            Debug.LogWarning("[SlideshowController] No slides provided.");
        }

        if (nextButton)
            nextButton.onClick.AddListener(Next);
        if (prevButton)
            prevButton.onClick.AddListener(Prev);
        if (beginButton)
            beginButton.onClick.AddListener(Begin);
    }

    void Start()
    {
        targetImage.preserveAspect = true;

        if (slides.Count > 0)
        {
            startIndex = Mathf.Clamp(startIndex, 0, slides.Count - 1);
            Show(startIndex);
        }

        UpdateBeginButtonVisibility();
    }

    void OnDestroy()
    {
        if (nextButton)
            nextButton.onClick.RemoveListener(Next);
        if (prevButton)
            prevButton.onClick.RemoveListener(Prev);
        if (beginButton)
            beginButton.onClick.RemoveListener(Begin);
    }

    void Update()
    {
        if (_isFading)
            return; // lock input during fade

        bool isLastSlide = (_index == slides.Count - 1);

        // On all slides except the last — allow arrow and space to go forward
        if (!isLastSlide)
        {
            if (
                Input.GetKeyDown(nextKey)
                || Input.GetKeyDown(alsoNextKey)
                || Input.GetButtonDown("Submit")
            )
                Next();

            if (Input.GetKeyDown(prevKey))
                Prev();
        }
        else
        {
            // On the last slide — pressing Enter simulates a proper BeginButton click
            if (Input.GetButtonDown("Submit"))
            {
                if (
                    beginButton != null
                    && beginButton.gameObject.activeSelf
                    && beginButton.interactable
                )
                    StartCoroutine(SimulateBeginButtonPress());
                else
                    Begin(); // fallback if button missing
            }

            // Optional: still allow going backward if you want
            if (Input.GetKeyDown(prevKey))
                Prev();
        }
    }

    private IEnumerator SimulateBeginButtonPress()
    {
        var btn = beginButton;
        if (btn == null)
            yield break;

        if (EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // Create a fake click
        var ped = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left,
            clickCount = 1,
        };

        // Trigger pressed visual
        ExecuteEvents.Execute(btn.gameObject, ped, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSeconds(0.1f);

        // Release and invoke the click
        ExecuteEvents.Execute(btn.gameObject, ped, ExecuteEvents.pointerUpHandler);
        btn.onClick.Invoke();
    }

    public void Next()
    {
        if (slides == null || slides.Count == 0)
            return;

        // If currently last slide:
        if (_index >= slides.Count - 1)
        {
            if (loadSceneAtEnd)
            {
                Begin(); // reuse same fade+load path
                return;
            }
            if (!wrapAround)
            {
                // Stay on last slide
                return;
            }
        }

        if (SlideSwitchAudio.clip != null)
        {
            AudioBus?.Emit(SlideSwitchAudio);
        }

        int next = (_index + 1) % Mathf.Max(1, slides.Count);
        Show(next);
    }

    public void Prev()
    {
        if (slides == null || slides.Count == 0)
            return;

        if (_index <= 0 && !wrapAround)
        {
            // Stay on first slide
            return;
        }

        if (SlideSwitchAudio.clip != null)
        {
            AudioBus?.Emit(SlideSwitchAudio);
        }

        int prev = (_index - 1 + slides.Count) % Mathf.Max(1, slides.Count);
        Show(prev);
    }

    public void GoTo(int index)
    {
        if (slides == null || slides.Count == 0)
            return;
        Show(Mathf.Clamp(index, 0, slides.Count - 1));
    }

    public void Begin()
    {
        if (_isFading)
            return;
        StartCoroutine(RunPowerOff());
    }

    private void Show(int index)
    {
        if (index == _index)
            return;
        _index = index;

        var s = slides[_index];
        if (s == null)
        {
            Debug.LogWarning($"[SlideshowController] Slide {_index} is null.");
            return;
        }

        targetImage.sprite = s;

        // Ensure fully visible (in case something faded it elsewhere)
        var c = targetImage.color;
        c.a = 1f;
        targetImage.color = c;

        UpdateBeginButtonVisibility();
    }

    private void UpdateBeginButtonVisibility()
    {
        if (!beginButton)
            return;
        bool isLast = (slides != null && slides.Count > 0 && _index == slides.Count - 1);
        beginButton.gameObject.SetActive(isLast);
    }

    private IEnumerator RunPowerOff()
    {
        _isFading = true;
        yield return new WaitForSeconds(0.15f);
        if (ShutdownAudio.clip != null)
        {
            AudioBus?.Emit(ShutdownAudio);
        }
        PowerOff.SetActive(true);
        PowerOff.GetComponent<Animator>().SetTrigger("PlayPowerOff");
    }
}
