using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MaterialInstance))]
public class MaterialInstance : Editor
{

    [MenuItem("GameObject/Instance Material")]
    public static void Instance()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("No Valid Object Selected");
            return;
        }

        if (Selection.activeGameObject.GetComponent<MeshRenderer>() == null && Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>() == null)
        {
            Debug.LogError("No Skinned Mesh Renderer or No renderer on this GameObject");
            return;
        }

        Material mat = Selection.activeGameObject.GetComponent<MeshRenderer>().sharedMaterial;
        Selection.activeGameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(mat);
    }
}