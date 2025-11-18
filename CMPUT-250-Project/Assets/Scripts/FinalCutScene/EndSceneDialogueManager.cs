using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// One line of end-scene dialogue, loaded from JSON
[System.Serializable]
public struct EndSceneLine
{
    public string message;
    public int spriteIndex; // -1 if no sprite change on this line
}

public class EndSceneDialogueManager : Subscriber
{
    [Header("JSON Loading")]
    [Tooltip(
        "Subfolder inside StreamingAssets, e.g. 'EndScene' -> Assets/StreamingAssets/EndScene"
    )]
    public string directory = "EndScene";

    [Tooltip("JSON filenames WITHOUT .json, in the order you want them played.")]
    public string[] dialogueFiles;

    private Dictionary<string, EndSceneLine> lines;
    private List<EndSceneLine> orderedLines = null;
    private int currentLineIndex = -1;
    private bool hasRequestedLoad = false;

    [Header("Event Listeners")]
    [Tooltip("Emitted by your async loading system when everything is ready.")]
    public UnitGameEvent AsyncComplete;

    [Header("UI References")]
    public CanvasGroup textBoxGroup; // CanvasGroup on the text box panel
    public Text dialogueText; // regular UnityEngine.UI.Text
    public Button nextButton;
    public Image characterImage; // character portrait image

    [Header("Character Sprites")]
    public Sprite[] characterSprites; // indices used by spriteIndex

    [Header("Typing Settings")]
    public float charactersPerSecond = 30f;

    [Tooltip("Play typing SFX every N characters (1 = every char).")]
    public int charsPerTypingSfx = 2;

    [Header("Audio Buses")]
    public AudioGameEvent MusicBus; // background music bus
    public AudioGameEvent SfxBus; // SFX bus (typing, click, etc.)

    [Header("Audio Clips")]
    public Audio EndSceneMusic; // background track for this scene
    public Audio TypingSfx; // small bleep for typing
    public Audio NextLineSfx; // optional click when moving to next line

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool textBoxVisible = false;

    // ---------------- Subscriber setup ----------------

    public override void Subscribe()
    {
        // Listen for the async-complete signal
        AsyncComplete?.Subscribe(OnAsyncComplete);

        // Basic UI init
        if (textBoxGroup != null)
        {
            textBoxGroup.alpha = 0f;
            textBoxVisible = false;
        }

        if (dialogueText != null)
            dialogueText.text = string.Empty;

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextClicked);
            nextButton.interactable = false; // we enable this once lines are loaded
        }

        // Start end-scene music via the audio bus (if configured)
        if (MusicBus != null && EndSceneMusic.clip != null) // CHANGED
        {
            MusicBus.Emit(EndSceneMusic);
        }

        // Start loading our end-scene JSON via the existing async importer
        StartCoroutine(
            JSONImporter.ImportFiles<EndSceneLine>(
                Path.Combine("lang", "en", directory),
                dialogueFiles,
                (dialogueOut) =>
                {
                    lines = dialogueOut;
                    AsyncComplete?.Emit();
                }
            )
        );
    }

    // ---------------- JSON load callback ----------------
    private void OnAsyncComplete()
    {
        Debug.Log(lines.ToString());
        if (hasRequestedLoad)
            return; // avoid double-start if event emitted twice
        hasRequestedLoad = true;

        // Preserve the order specified in dialogueFiles
        foreach (string file in lines.Keys)
        {
            if (lines.TryGetValue(file, out EndSceneLine line))
            {
                orderedLines.Add(line);
            }
            else
            {
                Debug.LogWarning("EndSceneDialogueManager: Missing dialogue file: " + file);
            }
        }

        if (lines.Count == 0)
        {
            Debug.LogError("EndSceneDialogueManager: No dialogue lines loaded!");
            return;
        }

        if (nextButton != null)
            nextButton.interactable = true;

        currentLineIndex = -1;
        ShowNextLine();
    }

    // ---------------- Input / flow ----------------

    private void OnNextClicked()
    {
        // Optional SFX for going to next line (only when not skipping)
        if (!isTyping && SfxBus != null && NextLineSfx.clip != null) // CHANGED
        {
            SfxBus.Emit(NextLineSfx);
        }

        if (isTyping)
        {
            FinishCurrentLineInstantly();
        }
        else
        {
            ShowNextLine();
        }
    }

    private void ShowNextLine()
    {
        currentLineIndex++;

        // No more lines: fade out textbox and disable button
        if (currentLineIndex >= lines.Count)
        {
            if (nextButton != null)
                nextButton.interactable = false;

            if (textBoxGroup != null)
                StartCoroutine(FadeTextBox(1f, 0f, 0.5f));

            // TODO: trigger scene change / credits here if needed
            return;
        }

        // Ensure textbox visible
        if (!textBoxVisible && textBoxGroup != null)
        {
            StartCoroutine(FadeTextBox(0f, 1f, 0.5f));
        }

        EndSceneLine line = orderedLines[currentLineIndex];

        // Handle sprite change
        if (
            characterImage != null
            && characterSprites != null
            && line.spriteIndex >= 0
            && line.spriteIndex < characterSprites.Length
        )
        {
            characterImage.sprite = characterSprites[line.spriteIndex];
        }

        // Start typing animation
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line.message));
    }

    // ---------------- Typing effect ----------------

    private IEnumerator TypeLine(string fullText)
    {
        isTyping = true;

        if (dialogueText != null)
            dialogueText.text = string.Empty;

        float delay = 1f / Mathf.Max(1f, charactersPerSecond);
        int charCount = 0;

        foreach (char c in fullText)
        {
            if (dialogueText != null)
                dialogueText.text += c;

            charCount++;

            // Play typing sound every few chars
            if (
                SfxBus != null
                && TypingSfx.clip != null
                && charCount % Mathf.Max(1, charsPerTypingSfx) == 0
            )
            {
                SfxBus.Emit(TypingSfx);
            }

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void FinishCurrentLineInstantly()
    {
        if (currentLineIndex < 0 || currentLineIndex >= lines.Count)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText != null)
            dialogueText.text = orderedLines[currentLineIndex].message;

        isTyping = false;
    }

    // ---------------- Fading textbox ----------------

    private IEnumerator FadeTextBox(float from, float to, float duration)
    {
        if (textBoxGroup == null)
            yield break;

        textBoxVisible = true;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            textBoxGroup.alpha = Mathf.Lerp(from, to, lerp);
            yield return null;
        }

        textBoxGroup.alpha = to;

        if (Mathf.Approximately(to, 0f))
        {
            textBoxVisible = false;
        }
    }
}
