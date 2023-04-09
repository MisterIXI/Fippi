using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingsManager))]
public class SettingsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Save Player Settings"))
            SettingsManager.SavePlayerSettings();
        if (GUILayout.Button("Load Player Settings"))
            SettingsManager.LoadPlayerSettings();
    }
}