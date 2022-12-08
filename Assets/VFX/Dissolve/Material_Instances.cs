using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Instances : MonoBehaviour
{
    public GameObject go;
    public Color color;
    public Color colorPreAtatck;
    public Material material;

    public Texture2D texture;

    public Vector2 setPixelTexture;

    void Awake()
    {
        go = this.gameObject;
        try
        {
            material = go.GetComponent<MeshRenderer>().material;
        }
        catch
        {
            material = go.GetComponent<SkinnedMeshRenderer>().material;
        }

        if (texture == null)
        {
            Debug.LogWarning("Aucune texture trouvé dans le shader, creation d une nouvelle");
            ChangeColorTexture(color);
        }
    }

    public void ChangeColorTexture(Color color)
    {
        texture = new Texture2D(128, 128);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        material.SetTexture("_TextureToDissolve", texture);
    }

    // Update is called once per frame
    void Update()
    {
        material.color = color;
    }
}