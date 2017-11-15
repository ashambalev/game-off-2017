using UnityEngine;
using System.Collections;

/// <summary>
/// The script adjusts the Canvas's scale factor so that it matches the PixelPerfectCamera's.
/// <para />
/// The Canvas render mode must be "Screen Space - Camera". The selected render camera should use the "PixelPerfectCamera" script.
/// </summary>
/// <remarks>
/// Even if you don't use this script, setting integer values to Canvas Scaler's scale factor will result in pixel perfect rendering. However, 
/// the canvas' scale will not match the camera's. Use this script to have the Canvas' scale match the PixelPerfectCamera's.
/// </remarks>
[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
public class CanvasScreenSpaceScaler : MonoBehaviour {

	Canvas _canvas;
	PixelPerfectCamera _pixelPerfectCamera;

    bool _isInitialized;

	void Initialize(bool warn)
    {
#if UNITY_EDITOR
        if (!gameObject.activeInHierarchy)
            return;
#endif
        _canvas = GetComponent<Canvas> ();

		if (_canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            if (warn) Debug.Log("Render mode: " + _canvas.renderMode + " is not supported by CanvasScreenSpaceScaler");
			return;
		}
			
		Camera uiCamera = GetComponent<Canvas> ().worldCamera;

		_pixelPerfectCamera = uiCamera.GetComponent<PixelPerfectCamera> ();

        if (_pixelPerfectCamera == null)
        {
            if (warn) Debug.Log("You have to use the PixelPerfectCamera script on the canvas' render camera!");
            return;
        }

        _isInitialized = true;

        AdjustCanvas();
    }

    void OnEnable()
    {
        Initialize(true);
    }

    void OnValidate()
    {
        Initialize(true);
    }

	//#if UNITY_EDITOR
	void Update ()
    {
#if UNITY_EDITOR
        if (!gameObject.activeInHierarchy)
            return;
#endif
        // Initialized? Try to initialize
        if (!_isInitialized)
            Initialize(false);
        if (!_isInitialized || _canvas.renderMode != RenderMode.ScreenSpaceCamera)
            return;

        // Detect changes in ratio
        if (_canvas.scaleFactor != _pixelPerfectCamera.ratio)
            AdjustCanvas();
    }
    //#endif

    void AdjustCanvas()
    {
        if (!_pixelPerfectCamera.isInitialized || _pixelPerfectCamera.ratio == 0)
            return;

        _canvas.scaleFactor = _pixelPerfectCamera.ratio;
    }
}
