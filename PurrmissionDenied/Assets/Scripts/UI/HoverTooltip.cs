using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverTooltip : MonoBehaviour
{
    public static HoverTooltip Instance;

    [SerializeField]
    private float typeDelay = 0.05f;

    [SerializeField]
    private float hideDelay = 1.2f;

    [SerializeField]
    private CanvasGroup TooltipRoot;

    [SerializeField]
    private RectTransform TooltipLayoutRoot;

    [SerializeField]
    private Animator TooltipIcon;

    [SerializeField]
    private Text TooltipContent;

    private bool runMove = false;
    private Coroutine activeTyping;
    private Coroutine hideWait;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        lastPosition = Input.mousePosition;
    }

    private Vector3 lastPosition;

    void Update()
    {
        if (runMove)
        {
            MoveTo((Input.mousePosition + lastPosition) / 2);
        }
        lastPosition = Input.mousePosition;
    }

    public void Type(string text)
    {
        if (activeTyping != null)
        {
            StopCoroutine(activeTyping);
        }
        activeTyping = StartCoroutine(TypeEffect(text));
    }

    private IEnumerator TypeEffect(string text)
    {
        TooltipContent.text = "";
        foreach (char ch in text)
        {
            TooltipContent.text += ch;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(TooltipLayoutRoot);
            yield return new WaitForSeconds(typeDelay);
        }
        activeTyping = null;
    }

    public void SetText(string text)
    {
        if (activeTyping != null)
        {
            StopCoroutine(activeTyping);
            activeTyping = null;
        }
        TooltipContent.text = text;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(TooltipLayoutRoot);
    }

    public enum Icon
    {
        Throbber,
        Tick,
        Cross,
    }

    public void SetIcon(Icon icon)
    {
        switch (icon)
        {
            case Icon.Throbber:
                TooltipIcon.SetTrigger("Throbber");
                break;
            case Icon.Tick:
                TooltipIcon.SetTrigger("Tick");
                break;
            case Icon.Cross:
                TooltipIcon.SetTrigger("Cross");
                break;
        }
    }

    public void Show()
    {
        if (hideWait != null)
        {
            StopCoroutine(hideWait);
            hideWait = null;
        }
        TooltipRoot.alpha = 1f;
        runMove = true;
    }

    public void Hide()
    {
        hideWait = StartCoroutine(HideEffect());
        runMove = false;
    }

    private IEnumerator HideEffect()
    {
        yield return new WaitForSeconds(hideDelay);
        TooltipRoot.alpha = 0f;
    }

    private void MoveTo(Vector2 position)
    {
        transform.position = position;
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
