using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class AutoGitVersionUpdater : IPreprocessBuildWithReport
{
    static AutoGitVersionUpdater()
    {
        //Hook Into Play Mode Change to Generate Version File When Entering Play Mode From Editor
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                GenerateGitVersionFile();
            }
        };
    }

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        GenerateGitVersionFile();
    }

    private static void GenerateGitVersionFile()
    {
        string gitDescribe = "unknown";

        try
        {
            //Get Latest Commit Message Summary (Version No.)
            ProcessStartInfo messageInfo = new ProcessStartInfo("git", "log -1 --pretty=%s")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process p = Process.Start(messageInfo))
            {
                gitDescribe = p.StandardOutput.ReadToEnd().Trim();
            }
        }
        catch
        {
            UnityEngine.Debug.LogWarning("Git not found or error during version generation.");
        }

        string version = $"Version: {gitDescribe}";
        string path = "Assets/Resources/version.txt";

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, version);

        UnityEngine.Debug.Log($"Git version file written: {gitDescribe}");
    }
}