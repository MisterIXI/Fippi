using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitMovement))]
public class UnitMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Debug Find Path"))
            ((UnitMovement)target).DebugFindPath();

        if(GUILayout.Button("Print Path"))
            ((UnitMovement)target).PrintPath();
    }
}