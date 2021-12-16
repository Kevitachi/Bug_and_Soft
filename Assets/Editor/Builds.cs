using System;
using System.IO;
using UnityEditor;
using UnityEngine;

class Builds
{
    private static string PathCombine(string path1, string path2)
    {
        if (Path.IsPathRooted(path2))
        {
            path2 = path2.TrimStart(Path.DirectorySeparatorChar);
            path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
        }

        return Path.Combine(path1, path2);
    }

    static void build() {
        string[] scenes = {
            "Assets/Scenes/Main/Level1.unity",
            "Assets/Scenes/Main/Level2.unity",
            "Assets/Scenes/Main/Level2.1.unity",
            "Assets/Scenes/Main/Level3.unity",
            "Assets/Scenes/Cutscenes/CutScene0.unity",
            "Assets/Scenes/Creditos/Creditos.unity",
            "Assets/Scenes/Cutscenes/CutScene1.unity"
        };
		
		string ejecutable = Application.dataPath;
        ejecutable = ejecutable.Replace("/Assets", "");
		ejecutable = PathCombine(ejecutable, "/Build/Paco.exe");

        BuildPipeline.BuildPlayer(scenes, ejecutable, BuildTarget.StandaloneWindows64, BuildOptions.None);      
    }
}