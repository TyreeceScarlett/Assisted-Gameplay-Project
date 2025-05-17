using UnityEngine;

public class TextureToMaterial : MonoBehaviour
{
    public Texture2D texture; // Drag your texture here
    public Shader shader; // Optionally set a custom shader (or use "Standard")

    void Start()
    {
        // Use the Standard shader if none provided
        if (shader == null)
            shader = Shader.Find("Standard");

        // Create the material
        Material material = new Material(shader);
        material.mainTexture = texture;

        // Apply it to this object's renderer (optional)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = material;
    }
}
