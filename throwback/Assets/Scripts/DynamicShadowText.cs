using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicShadowText : MonoBehaviour
{

    public int MaxRangeHorizontal = 2;
    public int MaxRangeVertical = 2;
    public float Quantize = 2.5f;
    public int FPS = 120;
    private float _changeTimeout = 0;

    public float MinAlpha = 1f;
    public float MaxAlpha = 1f;

    public Shadow Shadow;

    // Use this for initialization
    void Start()
    {

    }

    void ChangeShadow()
    {
        Shadow.effectDistance = new Vector2(Quantize * Random.Range(-MaxRangeHorizontal, MaxRangeHorizontal), Quantize * Random.Range(-MaxRangeVertical, MaxRangeVertical));
        Shadow.effectColor = Random.ColorHSV(0, 1f, 1f, 1f, 1f, 1f, MinAlpha, MaxAlpha);
    }

    // Update is called once per frame
    void Update()
    {
        if (_changeTimeout > 0)
        {
            _changeTimeout -= Time.deltaTime;
        }
        else
        {
            _changeTimeout += 1f / FPS;
            ChangeShadow();
        }
    }
}
