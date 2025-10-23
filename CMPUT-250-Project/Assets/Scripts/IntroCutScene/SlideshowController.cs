using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        if (
            Input.GetKeyDown(nextKey)
            || Input.GetKeyDown(alsoNextKey)
            || Input.GetButtonDown("Submit")
        )
            Next();

        if (Input.GetKeyDown(prevKey))
            Prev();
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
