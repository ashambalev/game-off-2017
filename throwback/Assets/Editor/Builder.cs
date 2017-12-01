using UnityEditor;
using System;

class Builder
{

    private static BuildPlayerOptions buildPlayerOptions;

    static void OSXBuild()
    {
        buildPlayerOptions.target = BuildTarget.StandaloneOSXUniversal;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    static void WinBuild()
    {
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    static void Win64Build()
    {
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    static void LinuxBuild()
    {
        buildPlayerOptions.target = BuildTarget.StandaloneLinuxUniversal;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static void iOSBuild()
    {
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static void WebGLBuild()
    {
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static void AndroidBuild()
    {
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }


    static void Build()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        string build_type = arguments[arguments.Length - 1];
        buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = arguments[arguments.Length - 2];
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/Game.unity" };
        switch (build_type)
        {
            case "Android":
                AndroidBuild();
                break;
            case "iOS":
                iOSBuild();
                break;
            case "WebGL":
                WebGLBuild();
                break;
            case "OSX":
                OSXBuild();
                break;
            case "Win":
                WinBuild();
                break;
            case "Win64":
                Win64Build();
                break;
            case "Linux":
                LinuxBuild();
                break;
            default:
                WebGLBuild();
                break;
        }
    }
}