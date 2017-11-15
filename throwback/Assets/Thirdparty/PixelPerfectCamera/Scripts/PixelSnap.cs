//#define REDUCE_JITTER 
using UnityEngine;
using System.Collections;


/// <summary>
/// The script enables RetroSnap for pixel perfect resolutions or eliminates jittering when traslating an object under a non pixel perfect resolution. It can also help with artifacts
/// related to subpixel positioning.
/// <para />
/// When the current object gets rendered by a camera, the script will look if it is a PixelPerfectCamera. If it is, it will read the retroSnap setting 
/// and render accordingly. RetroSnap will make the position of the object snap to the pixel grid as defined by the asset's pixels per unit. This will 
/// make the object move to multiples of screen pixels at once making a more "snappy" movement.
/// <para />
/// When the REDUCE_JITTER preprocessor symbol is enabled, reduce-jitter mode is used instead.This will work under any camera, 
/// regardless if it has the pixel perfect camera script attached or not. This can be helpful when translating pixelart objects in a non pixel-pefrect resolution
/// and point-filtering is used. Make sure that the RetroSnap option is disabled for your PixelPerfectCamera(s), because RetroSnap takes precedence over reduce-jitter mode.
/// </summary>
/// <remarks>
/// The script adjusts the object's position (while rendering) so that it snaps. It then restores the original position.
/// <para />
/// For Sprites only: the script takes into account the pivot point and screen resolution for proper texel to screen-pixel placement. This can help with artifacts where 2 sprites 
/// appear to move in different speeds due to different pivot point precision.
/// <para />
/// It works only when playing.
/// </remarks>
public class PixelSnap : MonoBehaviour
{
    private Sprite sprite;
    private Vector3 actualPosition;
    private bool shouldRestorePosition;

    // Use this for initialization
    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            sprite = renderer.sprite;
        }
        else
        {
            sprite = null;
        }
    }


    void OnWillRenderObject()
    {
        //Debug.Log("on will" + Camera.current);
        Camera cam = Camera.current;
        if (!cam)
            return;

        PixelPerfectCamera pixelPerfectCamera = cam.GetComponent<PixelPerfectCamera>();
        bool retroSnap = (pixelPerfectCamera == null) ? false : pixelPerfectCamera.retroSnap;

#if !REDUCE_JITTER
        if (!retroSnap)
            return;
#endif

        shouldRestorePosition = true;
        actualPosition = transform.position;

        float cameraPPU = (float)cam.pixelHeight / (2f * cam.orthographicSize);
        float cameraUPP = 1.0f / cameraPPU;

        Vector2 camPos = cam.transform.position.xy();
        Vector2 pos = actualPosition.xy();
        Vector2 relPos = pos - camPos;

        Vector2 offset = new Vector2(0, 0);
        // offset for screen pixel edge if screen size is odd
        offset.x = (cam.pixelWidth % 2 == 0) ? 0 : 0.5f;
        offset.y = (cam.pixelHeight % 2 == 0) ? 0 : 0.5f;
        // offset for pivot in Sprites
        Vector2 pivotOffset = new Vector2(0, 0);
        if (sprite != null)
        {
            pivotOffset = sprite.pivot - new Vector2(Mathf.Floor(sprite.pivot.x), Mathf.Floor(sprite.pivot.y)); // the fractional part in texture pixels           
            if (retroSnap)
            {
                // Nothing to do here. Pivot offset is already in asset pixels.
            }
            else
            {
                float camPixelsPerAssetPixel = cameraPPU / sprite.pixelsPerUnit;
                pivotOffset *= camPixelsPerAssetPixel; // convert to screen pixels
            }
        }
        if (retroSnap)
        {
            float assetPPU = pixelPerfectCamera.assetsPixelsPerUnit;
            float assetUPP = 1.0f / assetPPU;
            float camPixelsPerAssetPixel = cameraPPU / assetPPU;

            offset.x /= camPixelsPerAssetPixel; // zero or half a screen pixel in texture pixels
            offset.y /= camPixelsPerAssetPixel;
            // We don't take into account the pivot offset when rounding to avoid the artifact where 2 sprites appear to move at different times because of the fractional part of their pivots.
            relPos.x = (Mathf.Round(relPos.x / assetUPP - offset.x) + offset.x + pivotOffset.x) * assetUPP;
            relPos.y = (Mathf.Round(relPos.y / assetUPP - offset.y) + offset.y + pivotOffset.y) * assetUPP;

        }
        else
        {
            // Convert the units to pixels, round them, convert back to units. The offsets make sure that the distance we round is from screen pixel (fragment) edges to texel edges.
            relPos.x = (Mathf.Round(relPos.x / cameraUPP - offset.x) + offset.x + pivotOffset.x) * cameraUPP;
            relPos.y = (Mathf.Round(relPos.y / cameraUPP - offset.y) + offset.y + pivotOffset.y) * cameraUPP;
        }

        pos = relPos + camPos;

        transform.position = new Vector3(pos.x, pos.y, actualPosition.z);
    }

    // This scripts is based on the assumption that every camera that calls OnWillRenderObject(), will call OnRenderObject() before any other
    // camera calls any of these methods.
    void OnRenderObject()
    {
        //Debug.Log("on did" + Camera.current);
        if (shouldRestorePosition)
        {
            shouldRestorePosition = false;
            transform.position = actualPosition;
        }
    }

}
