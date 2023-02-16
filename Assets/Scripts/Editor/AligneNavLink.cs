using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(AlignNavLink))]
public class AligneNavLink : Editor
{
    [SerializeField] bool aligneToX;
    [SerializeField] bool aligneToZ;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AlignNavLink myScript = (AlignNavLink)target;
        if (GUILayout.Button("Aligne Axe"))
        {
            myScript.AligneAxe();
        }
    }
}