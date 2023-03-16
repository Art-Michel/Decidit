using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(LD_Analytics))]
public class LD_Analytics_UI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LD_Analytics LD_analytics = (LD_Analytics)target;

        GUILayout.Label("alive_Duration = " + PlayerPrefs.GetFloat("alive_Duration" + LD_analytics.gameObject.name), EditorStyles.boldLabel);
    }
}
