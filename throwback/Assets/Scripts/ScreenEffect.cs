using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenEffect : MonoBehaviour
{

    [Range(-1f, 1f)]
    public float intensity;
    [Range(0f, 1f)]
    public float fade;
    public Color FadeColor;
    public Material material;
    public Texture NoiseTexture;

    void Awake()
    {
        material.SetTexture("_noiseTex", NoiseTexture);
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        if (!GameManager.Instance.GameFinished)
            intensity = 0.5f * (Mathf.Max(GameManager.Instance.CurrentLevelSpeed + GameManager.Instance.Player.SpeedBoost / 2f, 1f) - 1f);
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (GameManager.Instance.GameFinished)
        {
            intensity += 0.3f * Time.deltaTime;
            fade += 0.2f * Time.deltaTime;
        }
        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }
        source.wrapMode = TextureWrapMode.Repeat;
        material.SetFloat("_distortionValue", intensity);
        material.SetFloat("_fadeValue", Mathf.Clamp01(fade + intensity / 2f));
        material.SetColor("_fadeColor", FadeColor);
        Graphics.Blit(source, destination, material);
    }
}
