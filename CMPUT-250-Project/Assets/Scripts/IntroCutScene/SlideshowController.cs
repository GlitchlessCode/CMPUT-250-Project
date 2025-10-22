using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("Scene Transition")]
    [Tooltip("Name of the scene to load after the slideshow.")]
    public string nextSceneName = "";
    [Tooltip("If true, pressing Next on the last slide will fade+load the scene.")]
    public bool loadSceneAtEnd = false;

    [Header("Fade To Black")]
    [Tooltip("Full-screen black Image with alpha 0 at start.")]
    public Image fadeOverlay;        // assign your FadeOverlay image
    public float fadeDuration = 0.75f;

    [Header("Final Slide UI")]
    [Tooltip("Button shown only on the last slide.")]
    public Button beginButton;       // assign your BeginButton

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

        if (nextButton) nextButton.onClick.AddListener(Next);
        if (prevButton) prevButton.onClick.AddListener(Prev);
        if (beginButton) beginButton.onClick.AddListener(Begin);
    }

    void Start()
    {
        targetImage.preserveAspect = true;

        if (slides.Count > 0)
        {
            startIndex = Mathf.Clamp(startIndex, 0, slides.Count - 1);
            Show(startIndex);
        }

        // Ensure overlay starts transparent
        if (fadeOverlay)
        {
            var c = fadeOverlay.color; c.a = 0f; fadeOverlay.color = c;
            // Keep it active so it can block clicks during fade
            fadeOverlay.raycastTarget = true;
        }
        UpdateBeginButtonVisibility();
    }

    void OnDestroy()
    {
        if (nextButton) nextButton.onClick.RemoveListener(Next);
        if (prevButton) prevButton.onClick.RemoveListener(Prev);
        if (beginButton) beginButton.onClick.RemoveListener(Begin);
    }

    void Update()
    {
        if (_isFading) return; // lock input during fade

        if (Input.GetKeyDown(nextKey) || Input.GetKeyDown(alsoNextKey) || Input.GetButtonDown("Submit"))
            Next();

        if (Input.GetKeyDown(prevKey))
            Prev();
    }

    public void Next()
    {
        if (slides == null || slides.Count == 0) return;

        // If currently last slide:
        if (_index >= slides.Count - 1)
        {
            if (loadSceneAtEnd && !string.IsNullOrEmpty(nextSceneName))
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

        int next = (_index + 1) % Mathf.Max(1, slides.Count);
        Show(next);
    }

    public void Prev()
    {
        if (slides == null || slides.Count == 0) return;

        if (_index <= 0 && !wrapAround)
        {
            // Stay on first slide
            return;
        }

        int prev = (_index - 1 + slides.Count) % Mathf.Max(1, slides.Count);
        Show(prev);
    }

    public void GoTo(int index)
    {
        if (slides == null || slides.Count == 0) return;
        Show(Mathf.Clamp(index, 0, slides.Count - 1));
    }

    public void Begin()
    {
        if (_isFading) return;
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("[SlideshowController] nextSceneName is empty; cannot load scene.");
            return;
        }
        StartCoroutine(FadeToBlackAndLoad());
    }

    private void Show(int index)
    {
        if (index == _index) return;
        _index = index;

        var s = slides[_index];
        if (s == null)
        {
            Debug.LogWarning($"[SlideshowController] Slide {_index} is null.");
            return;
        }

        targetImage.sprite = s;

        // Ensure fully visible (in case something faded it elsewhere)
        var c = targetImage.color; c.a = 1f; targetImage.color = c;

        UpdateBeginButtonVisibility();
    }

    private void UpdateBeginButtonVisibility()
    {
        if (!beginButton) return;
        bool isLast = (slides != null && slides.Count > 0 && _index == slides.Count - 1);
        beginButton.gameObject.SetActive(isLast);
    }

    private IEnumerator FadeToBlackAndLoad()
    {
        _isFading = true;

        if (fadeOverlay == null)
        {
            Debug.LogWarning("[SlideshowController] No fadeOverlay assigned; loading scene immediately.");
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        float t = 0f;
        Color c = fadeOverlay.color;

        // Fade alpha 0 -> 1 using unscaled time (works if timeScale = 0)
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
