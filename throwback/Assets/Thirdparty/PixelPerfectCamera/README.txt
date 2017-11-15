# Pixel Perfect Camera
-----------------------

Support thread:
http://forum.unity3d.com/threads/released-free-pixel-perfect-camera.416141/

How to use video:
https://www.youtube.com/watch?v=OuTMcY3H2j8

Please read the documentation inside the scripts for more in-depth information: PixelPerfectCamera.cs, PixelSnap.cs, CanvasWorldScaler.cs, CanvasScreenSpaceScaler.

## Instructions for Pixel Perfect Camera:

1) Place the PixelPerfectCamera.cs script on a 2D orthographic camera.

2) Set the "Assets pixels per unit" to the the value used by your sprites. By default this is 100.

3) You can specify either the target width or height in units. This works as the default unity camera, where you specify half of the corresponding dimension.

4) If you enable "Pixel Perfect" mode, the script will adjust the camera size so that it is pixel perfect. The resulting size will not be the same size as the target size you specified, but it will be as close as possible (slightly bigger or smaller).

5) If you enable one or both of the "max width/height", the resulting camera width or height will never be larger that the specified values. 

6) When "PixelPerfect" mode is enabled, you can enable "Retro Snap" to make the position of game objects and sprites snap to the pixels of the assets. This will affect only the game objects that have PixelSnap.cs script added on them. Read more below.

7) Enable "show HUD" if you want to toggle "Pixel perfect" while the game is running.

Below the controls, you can see some numbers regarding the size calculation:

- Size: this is the resulting camera size in units: [width x height]. When a "max width/height" restricts the size, the corresponding dimension will be bold.

- Pixels Per Unit: the number of screen pixels a unit is rendered to (this is *not* the assets pixels per unit)

- Pixels: the resolution of the screen given as a multiple of 2 numbers. The first is the number of screen pixels an asset pixel will render to. The second is the camera resolution in asset pixels. For example: 4 x [100.5, 300.0] means that the screen resolution is [402, 1200] and it corresponds to a width of 100.5 pixels and a height of 300 pixels in asset pixels. A single asset pixel is rendered to 4 screen pixels. A non pixel-perfect resolution would have a non-integer in the place of "4".

- Coverage: the percentage of the resulting size to the target size. This is usually 100%, which means that the resulting size is the same as the targeted one. However, if "Pixel Perfect" is enabled, the percentage may be higher or lower than 100% if the resulting size is bigger or smaller than the target size. Also, if "max width/height" restricts the size it can reduce the coverage.

### Usage tips:

- Start by disabling "pixel perfect" and adjust either the target width or height according to your scene. If needed, specify the "max width/height". Enable "pixel perfect". 

- If you set the target width, then the camera's height depends on the user's monitor/mobile device screen aspect ratio. If for example the device is quite tall, then the camera's height will be a larger number. If your world can't expand vertically forever, it's best that you set a "max height" on the script. 

- When you enable "pixel perfect" mode, the resulting size may be slightly smaller or bigger than the one you set in "target size". You may want to enable "max width/height" to restrict the size.

Note that under Unity's editor, the Game view may appear scaled if the "Scale" slider is not "1". In this case, the result will not appear pixel perfect in your screen, however the game will render internally in a pixel perfect way.

## Instructions for Canvas:

The following assume that you have placed the PixelPerfectCamera.cs script on the camera.

If you use a Canvas that has "World Space" render mode, you will get a pixel-perfect result out of the box (assuming that you have enabled "pixel perfect"). If you want the canvas size to match the camera's size, do the following:

- Place the CanvasWorldScaler.cs script on the canvas. 
- Provide a camera on the UI camera property of the script. The camera should use the PixelPerfectCamera script.

This will adjust the size and scale of the canvas so that it matches the camera's size and fills the whole screen.

If you use a Canvas that has "Screen Space - Camera" render mode:

- Set "Constant Pixel Size" on the Canvas Scaler's Ui Scale Mode.
- Place the CanvasWorldScaler.cs script on the Canvas.
- Provide a camera that uses the PixelPerfectCamera script on the "Render Camera" property of the Canvas.

The script will adjust the "Scale Factor" of the "Canvas Scaler" and as a result the canvas elements will have the correct size. Notice that Unity has a bug and it will not show in the inspector the updated value of the scale factor.

## Instructions for PixelSnap:

1) Place the script on every object that you want to snap on the pixel grid of the assets (retroSnap mode) or on the screen's pixel grid (reduce jitter mode).

2) There are 2 ways PixelSnap script can work: RetroSnap or Reduce Jitter mode and the result depends on the camera that will render the object.

RetroSnap: Enable the `RetroSnap` option in your PixelPerfectCamera. **All** the objects that use the PixelSnap script will render in RetroSnap mode, when rendered by that camera.

Reduce Jitter: If you uncomment the REDUCE_JITTER symbol on top of the script's code , jitter-reduction mode will be enabled (disabled by default). It will work under any camera regardless if it has the PixelPerfectCamera script attached or not. This can be helpful when translating pixelart objects in a non pixel-pefrect resolution. Make sure that the RetroSnap option is disabled for your PixelPerfectCamera(s), because RetroSnap takes precedence over reduce-jitter mode.

4) "Retro Snap" should be used with "Pixel Perfect" enabled, otherwise jitter artifacts will occur.

## Instructions for scripts interacting with PixelPerfectCamera

PixelPerfectCamera exposes some public variables under the "output" section in "PixelPerfectCamera.cs". You can read their values from your scripts and act accordingly. For example, if you want to know the camera's size in order to place a game object, you can read the "cameraSize" variable.

Make sure that PixelPerfectCamera executes before your script by using Unity's "Script Execution Order". Also, check if the "isInitialized" variable is true before reading the other variables. 

## Project set-up tips

If you want to enable the pixel-perfect mode, make sure that your project has the following set-up:

- Use an orthographic camera and throw the Pixel Perfect Camera script on it
- Leave the sprite's scale to 1.
- If you use UI Image elements under a Canvas element, select "set Native size".
- The textures of your sprites should use: point filtering, disable mip-mapping, compression to "none" (Truecolor format in older Unity versions)
- All your textures should have the same Pixels Per Unit
- For best results use the sprite editor to set a pivot point of [0.5, 0.5] to your sprites.
- Place your game objects in positions that are either integer or multiples of [1 / Pixels Per Unit]. For example, if Pixels Per Unit is 100, have the initial position of your game objects be at multiples of 0.01 such as 2.04, 4.32 but not 2.043 nor 0.001 etc.
- Make sure that the player settings of the platform(s) you are targeting don't reduce the texture size
- In your project's quality settings: set "Full res" in Texture quality and disable Anti-Aliasing. Make sure that the correct quality level is used for the platform you are targeting.
- DX9 samples from the edge of the texels instead of the center. This will result in all sprites rendered with 1 fragment offset. 
  If you use DX9 you can enable "pixel snap" in default sprite shader. This will correct the half fragment offset in the vertex shader. DX 11, OpenGL etc don't have this problem.




