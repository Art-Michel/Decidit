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

        if (Selection.activeGameObject.GetComponent<MeshRenderer>() == null && Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>() == null
            && Selection.activeGameObject.GetComponent<ParticleSystemRenderer>() == null)
        {
            Debug.LogError("No Skinned Mesh Renderer or No renderer on this GameObject");
            return;
        }

        if(Selection.activeGameObject.GetComponent<MeshRenderer>() != null)
        {
            Material mat = Selection.activeGameObject.GetComponent<MeshRenderer>().sharedMaterial;
            Selection.activeGameObject.GetComponent<MeshRenderer>().sharedMaterial = new Material(mat);
        }
        else if(Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>() != null)
        {
            Material mat = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
            Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = new Material(mat);
        }
        else if(Selection.activeGameObject.GetComponent<ParticleSystemRenderer>())
        {
            Material mat2 = Selection.activeGameObject.GetComponent<ParticleSystemRenderer>().trailMaterial;
            Selection.activeGameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = new Material(mat2);
        }
    }
}