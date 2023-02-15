using UnityEngine;

public class Material_Instances : MonoBehaviour
{
    public GameObject MeshGameObject;
    public Color ColorBase;
    public Color ColorPreAtatck;
    public Material Material;

    public Texture2D TextureBase;
    public Texture2D TexturePreAttack;

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

        ChangeColorTexture(ColorBase);

        if (TextureBase == null)
        {
            Debug.LogWarning("Aucune texture trouvee dans le shader, creation d'une nouvelle");
        }
    }

    public void ChangeColorTexture(Color color)
    {
        if (color == ColorBase)
        {
            Material.SetTexture("_TextureToDissolve", TextureBase);
        }
        else if (color == ColorPreAtatck)
        {
            Material.SetTexture("_TextureToDissolve", TexturePreAttack);
        }

        /* Texture = new Texture2D(128, 128);
         for (int y = 0; y < Texture.height; y++)
         {
             for (int x = 0; x < Texture.width; x++)
             {
                 Texture.SetPixel(x, y, color);
             }
         }
         Texture.Apply();
         Material.SetTexture("_TextureToDissolve", Texture);*/
    }

    // Update is called once per frame
    void Update()
    {
        Material.color = ColorBase;
    }
}