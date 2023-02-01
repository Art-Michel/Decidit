using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonGenerator dGenerator = target as DungeonGenerator;

        // if (GUILayout.Button("Generate"))
        // {
        //     dGenerator.Generate();
        // }
    }
}
