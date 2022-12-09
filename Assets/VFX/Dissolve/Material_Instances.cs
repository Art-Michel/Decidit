using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Instances : MonoBehaviour
{
    public GameObject MeshGameObject;
    public Color Color;
    public Color ColorPreAtatck;
    public Material Material;

    public Texture2D Texture;

    public Vector2 SetPixelTexture;

    void Awake()
    {
        //on prend ce gameobject que si yen a pas déja un de référencé comme ça si l'ia a son mesh renderer sur un autre gameobject on le référence juste
        if (MeshGameObject == null) MeshGameObject = this.gameObject;
        try
        {
            Material = MeshGameObject.GetComponent<MeshRenderer>().material;
        }
        catch
        {
            Material = MeshGameObject.GetComponent<SkinnedMeshRenderer>().material;
        }

        if (Texture == null)
        {
            Debug.LogWarning("Aucune texture trouvee dans le shader, creation d'une nouvelle");
            ChangeColorTexture(Color);
        }
    }

    public void ChangeColorTexture(Color color)
    {
        Texture = new Texture2D(128, 128);
        for (int y = 0; y < Texture.height; y++)
        {
            for (int x = 0; x < Texture.width; x++)
            {
                Texture.SetPixel(x, y, color);
            }
        }
        Texture.Apply();
        Material.SetTexture("_TextureToDissolve", Texture);
    }

    // Update is called once per frame
    void Update()
    {
        Material.color = Color;
    }
}