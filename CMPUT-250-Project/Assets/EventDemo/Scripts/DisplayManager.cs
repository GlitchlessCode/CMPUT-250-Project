using UnityEngine;
using UnityEngine.UI;

public class DisplayManager : MonoBehaviour
{
    public StringGameEvent RecieveText;
    public UnitGameEvent ClearText;
    public IntGameEvent AddAmount;

    public Text TextDisplay;
    public Text CounterDisplay;

    private int counter = 0;

    public void Awake()
    {
        RecieveText.Subscribe(OnRecieveText);
        ClearText.Subscribe(OnClearText);
        AddAmount.Subscribe(OnAddAmount);
    }

    private void setText(string text)
    {
        if (TextDisplay != null)
        {
            TextDisplay.text = text;
        }
    }

    private void setCounter()
    {
        if (CounterDisplay != null)
        {
            CounterDisplay.text = counter.ToString();
        }
    }

    private void OnRecieveText(string text)
    {
        setText(text);
        counter += 1;
        setCounter();
    }

    private void OnClearText()
    {
        setText("");
    }

    private void OnAddAmount(int amount)
    {
        counter += amount;
        setCounter();
    }
}
