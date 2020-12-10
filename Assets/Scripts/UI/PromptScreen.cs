using System;
using TMPro;
using UnityEngine;

public class PromptScreen : MonoBehaviour
{

    public TMP_Text promptText;
    public TMP_InputField promptInput;

    private Action<string> OnComplete;
    private Action OnCancel;

    public void Show(String prompt, Action<string> onComplete, Action onCancel = null)
    {
        gameObject.SetActive(true);
        promptText.text = prompt;
        OnComplete = onComplete;
        OnCancel = onCancel;

    }

    public void Event_OkClicked()
    {
        var del = OnComplete;
        CloseScreen();
        del?.Invoke(promptInput.text);
    }

    public void Event_CancelClicked()
    {
        var del = OnCancel;
        CloseScreen();
        del?.Invoke();
    }

    private void CloseScreen()
    {
        
        promptText.text = "";
        OnComplete = null;
        OnCancel = null;
        gameObject.SetActive(false);
    }

}