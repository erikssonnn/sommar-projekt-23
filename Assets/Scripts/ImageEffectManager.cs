using UnityEngine;

public class ImageEffectManager : MonoBehaviour
{
    [SerializeField] private Material material = null;
    [SerializeField] private int pixelCountMin, pixelCountMax = 0;
    [SerializeField] private int orthographicSizeMin, orthographicSizeMax = 0;
    [SerializeField] private bool linearPixelate = false;

    private new Camera camera = null;
    [SerializeField] private int pixelCount = 128;

    private void Start()
    {
        camera = Camera.main;
        if (camera == null)
        {
            throw new System.Exception("Main camera is null");
        }

        if (!camera.orthographic)
        {
            throw new System.Exception("Main camera is not orthographic");
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (camera == null) return;
        if (linearPixelate)
        {
            if (!camera.orthographic) return;
            float orthographicSize = camera.orthographicSize;
            float lerpValue = Mathf.InverseLerp(orthographicSizeMin, orthographicSizeMax, orthographicSize);
            pixelCount = Mathf.RoundToInt(Mathf.Lerp(pixelCountMin, pixelCountMax, lerpValue));
        }

        float aspect = camera.aspect;
        Vector2 count = new Vector2(pixelCount, pixelCount / aspect);
        Vector2 size = new Vector2(1.0f / count.x, 1.0f / count.y);

        material.SetVector("block_count", count);
        material.SetVector("block_size", size);
        material.SetTexture("main_tex", source);

        Graphics.Blit(source, destination, material);
    }
}