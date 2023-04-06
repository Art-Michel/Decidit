// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using UnityEditor;
// using UnityEditor.Rendering;
// using UnityEngine;
// using UnityEngine.UIElements;

// [CustomEditor(typeof(LD_Analytics))]
// public class LD_Analytics_UI : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();

//         LD_Analytics LD_analytics = (LD_Analytics)target;

//         #region Alive Timer
//         if (PlayerPrefs.GetInt("dead") == 1 || LD_analytics.seeMore)
//         {
//             float minutes = (int)PlayerPrefs.GetFloat("alive_Duration") / 60f;
//             float secondes = (minutes - Mathf.FloorToInt(minutes)) * 60f;
//             GUILayout.Label("alive_Duration = " + Mathf.FloorToInt(minutes) + "min" + secondes + "s", EditorStyles.boldLabel);
//         }
//         #endregion

//         #region Display player path
//         if (GUILayout.Button("data"))
//         {
//             Vector3[] pos = LD_analytics.GetPositions();
//             LineRenderer instance = Instantiate(LD_analytics.playerPath, pos[0], Quaternion.identity);
//             instance.positionCount = pos.Length;
//             instance.SetPositions(pos);
//         }
//         #endregion
//     }
// }
