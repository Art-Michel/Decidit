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

        #region Alive Timer
        if (PlayerPrefs.GetInt("dead") == 1 || LD_analytics.seeMore)
        {
            float minutes = (int)PlayerPrefs.GetFloat("alive_Duration") / 60f;
            float secondes = (minutes - Mathf.FloorToInt(minutes)) * 60f;
            GUILayout.Label("alive_Duration = " + Mathf.FloorToInt(minutes) + "min" + secondes + "s", EditorStyles.boldLabel);
        }
        #endregion
    }
}
