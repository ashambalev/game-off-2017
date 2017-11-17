using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenEffect : MonoBehaviour
{

    [Range(-1f, 1f)]
    public float intensity;
    public Color FadeColor;
    private Material material;
    public Texture NoiseTexture;

    void Awake()
    {
        material = new Material(Shader.Find("Hidden/ScreenShader"));
        material.SetTexture("_noiseTex", NoiseTexture);
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        intensity = 0.5f * (Mathf.Max(GameManager.Instance.CurrentLevelSpeed, 1f) - 1f);
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }
        source.wrapMode = TextureWrapMode.Repeat;
        material.SetFloat("_distortionValue", intensity);
        material.SetColor("_fadeColor", FadeColor);
        Graphics.Blit(source, destination, material);
    }
}
