using UnityEngine;

public class MagneticVisualManager : MonoBehaviour
{
    [Header("î≠åıêF")]
    [SerializeField] private Color northEmissionColor = Color.red;
    [SerializeField] private Color southEmissionColor = Color.blue;

    [Header("î≠åıÇÃã≠Ç≥")]
    [SerializeField] private float emissionStrength = 2f;

    private void Awake()
    {
        ApplyMagneticVisuals();
    }

    private void ApplyMagneticVisuals()
    {
        GameObject[] northObjects = GameObject.FindGameObjectsWithTag("Nã…");
        GameObject[] southObjects = GameObject.FindGameObjectsWithTag("Sã…");

        foreach (GameObject obj in northObjects)
        {
            ApplyEmission(obj, northEmissionColor);
        }

        foreach (GameObject obj in southObjects)
        {
            ApplyEmission(obj, southEmissionColor);
        }
    }

    private void ApplyEmission(GameObject obj, Color emissionColor)
    {
        Renderer[] renderers =
            obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;

            material.EnableKeyword("_EMISSION");

            material.SetColor(
                "_EmissionColor",
                emissionColor * emissionStrength
            );
        }
    }
}