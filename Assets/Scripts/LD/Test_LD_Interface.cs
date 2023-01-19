using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(Test_LD))]
public class Test_LD_Interface : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("\n                    CHOOSE YOUR ROOM          \n", EditorStyles.boldLabel);

        Test_LD ld = target as Test_LD;

        foreach (GameObject room in ld.rooms)
        {
            if (GUILayout.Button(room.name))
            {
                ld.HideRooms(room);
                //Debug.Log(room.name);
                ld.SetSpawn(room.GetComponent<Test_LD_Room>().spawn);
            }
        }

    }
}
