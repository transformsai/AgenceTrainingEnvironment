using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StringListView : MonoBehaviour
{
    [Serializable]
    public class StringListClickedEvent : UnityEvent<int, string>{}

    public string SelectedString => TargetStrings[SelectedIndex];

    public int Count => TargetStrings.Count;


    [SerializeField]
    private Button TemplateButton;

    [SerializeField]
    private Transform ButtonContainer;
    
    [SerializeField]
    private List<string> TargetStrings;
    
    private List<string> OldList = new List<string>();

    public int SelectedIndex = -1;
    
    [HideInInspector, SerializeField]
    private List<Button> Buttons;
    
    public StringListClickedEvent ElementClicked;

    private void Awake()
    {
        RecalculateButtons();
    }

    public void RecalculateButtons()
    {
        if (OldList != null && OldList.SequenceEqual(TargetStrings)) return;
        
        
        foreach (var child in ButtonContainer)
        {
            var transformChild = child as Transform;
            if (transformChild == null) continue;
            var btn = transformChild.GetComponent<Button>();
            if (!Buttons.Contains(btn)) continue;
            Destroy(btn.gameObject);
            
            Buttons.Remove(btn);

        }
        Buttons.Clear();

        foreach (var targetString in TargetStrings)
        {
            var newButton = Instantiate(TemplateButton, ButtonContainer);
            newButton.GetComponentInChildren<TMP_Text>().text = targetString;
            Buttons.Add(newButton);
            newButton.gameObject.SetActive(true);
        }
        OldList.Clear();
        OldList.AddRange(TargetStrings);
    }

    public void UpdateButtons(List<string> targetStrings)
    {
        TargetStrings = targetStrings;
        RecalculateButtons();
    }

    public void Event_ElementClicked(Button element)
    {
        var index = Buttons.IndexOf(element);
        if(index < 0) return;

        SelectedIndex = index;
        ElementClicked?.Invoke(index, TargetStrings[index]);
    }

    private void Update()
    {

        if(!OldList.SequenceEqual(TargetStrings)) RecalculateButtons();

        for (var i = 0; i < Buttons.Count; i++)
        {
            var button = Buttons[i];
            button.interactable = (i != SelectedIndex);
        }
    }
}
