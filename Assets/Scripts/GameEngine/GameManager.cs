using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = FindObjectOfType<GameManager>();
            if (_instance) return _instance;
            _instance = new GameObject("GameManager").AddComponent<GameManager>();
            return _instance;
        }
    }

    public static SettingsContainer Settings => Instance._settings;
    
    [SerializeField] private SettingsContainer _settings = null;

    private void Awake()
    {
        var filename = "EngineConfig.json";
        if (File.Exists(filename))
        {
            _settings = JsonUtility.FromJson<SettingsContainer>(File.ReadAllText(filename));
        }
        else
        {
            _settings = new SettingsContainer();
            File.WriteAllText(filename, JsonUtility.ToJson(_settings, true));
        }
    }
}