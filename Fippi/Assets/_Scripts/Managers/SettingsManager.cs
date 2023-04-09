using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

public class SettingsManager : MonoBehaviour
{
    [field: SerializeField] private CameraSettings _cameraSettings;
    public static CameraSettings CameraSettings => Instance._cameraSettings;
    [field: SerializeField] private ChunkSettings _chunkSettings;
    public static ChunkSettings ChunkSettings => Instance._chunkSettings;
    [field: SerializeField] private ColliderSettings _colliderSettings;
    public static ColliderSettings ColliderSettings => Instance._colliderSettings;
    [field: SerializeField] private CommanderSettings _commanderSettings;
    public static CommanderSettings CommanderSettings => Instance._commanderSettings;
    [field: SerializeField] private SpawnSettings _spawnSettings;
    public static SpawnSettings SpawnSettings => Instance._spawnSettings;
    [field: SerializeField] private UnitSettings _unitSettings;
    public static UnitSettings UnitSettings => Instance._unitSettings;
    [field: SerializeField] private PoolSetttings _poolSetttings;
    public static PoolSetttings PoolSetttings => Instance._poolSetttings;
    [field: SerializeField] private PlayerSettings _playerSettings;
    public static PlayerSettings PlayerSettings => Instance._playerSettings;
    private string _playerSettingsPath => Application.persistentDataPath + "/" + PlayerSettings.PlayerProfileName + "_saveFile.json";


    private bool _isInitialized = false;
    private static SettingsManager _instance;
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SettingsManager>();
                if (_instance == null)
                    Debug.LogError("No SettingsManager found in scene");
            }
            if (!_instance._isInitialized)
            {

            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        if (!_isInitialized)
            Initialize();
    }

    private void Initialize()
    {
        if (_isInitialized)
            return;
        _isInitialized = true;
        CheckScriptableObjects();


    }
    public static void SavePlayerSettings()
    {
        string playerSettingsJSON = JsonUtility.ToJson(PlayerSettings);
        // save player json to application.persitentDataPath
        File.WriteAllText(Instance._playerSettingsPath, playerSettingsJSON);
        Debug.Log($"Saved player settings to {Instance._playerSettingsPath}");
    }
    public static bool LoadPlayerSettings()
    {
        if (File.Exists(Instance._playerSettingsPath))
        {
            string playerSettingsJSON = File.ReadAllText(Instance._playerSettingsPath);
            JsonUtility.FromJsonOverwrite(playerSettingsJSON, PlayerSettings);
            Debug.Log($"Loaded player settings from {Instance._playerSettingsPath}");
            return true;
        }
        Debug.LogWarning($"No player settings found at {Instance._playerSettingsPath}");
        return false;
    }

    private void CheckScriptableObjects()
    {
        foreach (FieldInfo field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (field.FieldType.IsSubclassOf(typeof(ScriptableObject)))
            {
                ScriptableObject scriptableObject = (ScriptableObject)field.GetValue(this);
                if (scriptableObject == null)
                {
                    Debug.LogWarning($"Field {field.Name} is null");
                    scriptableObject = ScriptableObject.CreateInstance(field.FieldType);
                }
            }
        }
    }


}