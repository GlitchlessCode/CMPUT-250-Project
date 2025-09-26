using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public StringGameEvent SubmitEvent;
    public UnitGameEvent ClearEvent;
    public IntGameEvent AddEvent;

    public InputField InputBox;

    public void SubmitText()
    {
        if (InputBox != null)
        {
            string text = InputBox.text;
            InputBox.text = "";
            SubmitEvent.Emit(text);
        }
    }

    public void ClearText()
    {
        ClearEvent.Emit();
    }

    public void Add(int amount)
    {
        AddEvent.Emit(amount);
    }
}
