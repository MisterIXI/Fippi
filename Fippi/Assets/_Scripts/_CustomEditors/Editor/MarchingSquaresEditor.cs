using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MarchingSquares))]
public class MarchingSquaresEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Cave Walls"))
            ((MarchingSquares)target).GenerateCaveWalls();

        if (GUILayout.Button("Completely Random Map"))
            ((MarchingSquares)target).RegenerateMeshWithRandomSeed();

    }

    // button for perlin noise

}