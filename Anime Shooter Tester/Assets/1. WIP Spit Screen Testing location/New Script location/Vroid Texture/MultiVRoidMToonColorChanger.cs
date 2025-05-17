using UnityEngine;

public class MultiVRoidMToonColorChanger : MonoBehaviour
{
    [Header("Assign all your VRoid model roots here")]
    public GameObject[] vroidModels;

    [Header("Main lit color (MToon _Color)")]
    public Color litColor = Color.white;

    [Header("Shade color (MToon _ShadeColor)")]
    public Color shadeColor = Color.gray;

    [Header("Apply color automatically on Start")]
    public bool applyOnStart = false;

    void Start()
    {
        if (applyOnStart)
        {
            ChangeColors();
        }
    }

    public void ChangeColors()
    {
        if (vroidModels == null || vroidModels.Length == 0)
        {
            Debug.LogWarning("No VRoid models assigned.");
            return;
        }

        int modelIndex = 0;
        int totalMaterialsChanged = 0;

        foreach (GameObject model in vroidModels)
        {
            if (model == null)
                continue;

            Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    bool changed = false;

                    if (mat.HasProperty("_Color"))
                    {
                        mat.SetColor("_Color", litColor);
                        changed = true;
                    }

                    if (mat.HasProperty("_ShadeColor"))
                    {
                        mat.SetColor("_ShadeColor", shadeColor);
                        changed = true;
                    }

                    if (changed) totalMaterialsChanged++;
                }
            }

            modelIndex++;
        }

        Debug.Log($"Changed colors on {modelIndex} VRoid models, total materials affected: {totalMaterialsChanged}");
    }
}
