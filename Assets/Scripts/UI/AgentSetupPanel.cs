using System;
using System.Linq;
using UnityEngine;

public class AgentSetupPanel : MonoBehaviour
{
    public ToggleField TogglePrefab;
    public NumberField NumberFieldPrefab;

    public GameObject RewardHeader;
    public GameObject ActionHeader;
    public GameObject ObservationHeader;

    public AgentNames AgentName;
    

    public string[] RewardNames;
    public NumberField[] RewardFields;

    private string[] ActionNames => DefaultConfig.AllActions;
    public ToggleField[] ActionFields;

    public string[] ObservationNames;
    public ToggleField[] ObservationFields;

    public bool IsInitialized = false;


    private void OnEnable()
    {
        if (!IsInitialized) Initialize();
        
        var config = ConfigLoader.GetConfig(AgentName, true);
        
        for (var i = 0; i < ActionNames.Length; i++)
            ActionFields[i].Toggle.isOn = config.RawConfig.Actions.Contains(ActionNames[i]);
        
        for (var i = 0; i < ObservationNames.Length; i++)
            ObservationFields[i].Toggle.isOn = config.RawConfig.Observations.Contains(ObservationNames[i]);

        for (var i = 0; i < RewardNames.Length; i++)
        {
            var val = 0f;
            config.RawConfig.Rewards.TryGetValue(RewardNames[i], out val);
            
            RewardFields[i].InputField.text = val.ToString();
        }
    }


    private void OnDisable()
    {
        if(!IsInitialized) return;
        
        var newConfig = new AgentConfig();

        for (var i = 0; i < ObservationFields.Length; i++)
            if (ObservationFields[i].Toggle.isOn)
                newConfig.Observations.Add(ObservationNames[i]);

        for (var i = 0; i < ActionFields.Length; i++)
            if (ActionFields[i].Toggle.isOn)
                newConfig.Actions.Add(ActionNames[i]);

        
        for (var i = 0; i < RewardFields.Length; i++)
            if (float.TryParse(RewardFields[i].InputField.text, out var val) && val != 0)
                newConfig.Rewards.Add(RewardNames[i], val);

        ConfigLoader.SaveConfig(AgentName, newConfig);
    }

    private void Initialize()
    {
        
        RewardNames = DefaultConfig.AllRewards.Keys.ToArray();
        RewardFields = new NumberField[RewardNames.Length];
        var rewardIndex = RewardHeader.transform.GetSiblingIndex() + 1;
        for (var i = RewardNames.Length - 1; i >= 0; i--)
        {
            var rewardName = RewardNames[i];
            var newField = Instantiate(NumberFieldPrefab, RewardHeader.transform.parent);
            newField.Label.text = rewardName;
            newField.transform.SetSiblingIndex(rewardIndex);
            RewardFields[i] = newField;
        }

        ActionFields = new ToggleField[ActionNames.Length];
        var actionIndex = ActionHeader.transform.GetSiblingIndex() + 1;
        for (var i = ActionNames.Length - 1; i >= 0; i--)
        {
            var actionName = ActionNames[i];
            var newField = Instantiate(TogglePrefab, ActionHeader.transform.parent);
            newField.Label.text = actionName;
            newField.transform.SetSiblingIndex(actionIndex);
            ActionFields[i] = newField;
            newField.gameObject.SetActive(false);
        }


        ObservationNames = DefaultConfig.AllObservations.Keys.ToArray();
        ObservationFields =new ToggleField[ObservationNames.Length];
        var observationIndex = ObservationHeader.transform.GetSiblingIndex() + 1;
        for (var i = ObservationNames.Length - 1; i >= 0; i--)
        {
            var obsName = ObservationNames[i];
            var newField = Instantiate(TogglePrefab, ObservationHeader.transform.parent);
            newField.Label.text = obsName;
            newField.transform.SetSiblingIndex(observationIndex);
            ObservationFields[i] = newField;
            newField.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
        IsInitialized = true;
    }


    
    public void Event_Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);

    }
    
    public void Event_HeaderClick(GameObject headerRef)
    {
        if (headerRef == ActionHeader)
            foreach (var actionField in ActionFields)
                actionField.gameObject.SetActive(!actionField.gameObject.activeSelf);
        if (headerRef == ObservationHeader)
            foreach (var obsField in ObservationFields)
                obsField.gameObject.SetActive(!obsField.gameObject.activeSelf);
        if (headerRef == RewardHeader)
            foreach (var rewardField in RewardFields)
                rewardField.gameObject.SetActive(!rewardField.gameObject.activeSelf);

    }

}
