using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

[InitializeOnLoad]
public class GitKeeper : AssetPostprocessor
{

    private static List<string> gitKeepedDirs = new List<string>();

    static GitKeeper () {
        ScanProjectForGitKeeps();
    }

    [MenuItem("Assets/Git Keeper/Keep")]
    private static void GitKeep () {
        string path = GetActiveDirectory() + "/.gitkeep";
        gitKeepedDirs.Add(GetActiveDirectory());
        if (!File.Exists(path)) File.Create(path).Close();
    }

    [MenuItem("Assets/Git Keeper/Unkeep")]
    private static void GitUnkeep () {
        string path = GetActiveDirectory() + "/.gitkeep";
        gitKeepedDirs.Remove(GetActiveDirectory());
        if (File.Exists(path)) File.Delete(path);
    }

    [MenuItem("Assets/Git Keeper/Keep", true)]
    private static bool GitKeepValidate () {
        string activeDir = GetActiveDirectory();
        string path = activeDir + "/.gitkeep";
        if (File.Exists(path)) return false;
        return !ContainsFiles(activeDir);
    }

    private static bool GitKeepValidate (string dir) {
        string path = dir + "/.gitkeep";
        if (File.Exists(path)) return false;
        return !ContainsFiles(dir);
    }

    [MenuItem("Assets/Git Keeper/Unkeep", true)]
    private static bool GitUnkeepValidate () {
        string activeDir = GetActiveDirectory();
        string path = activeDir + "/.gitkeep";
        return File.Exists(path);
    }

    [MenuItem("Assets/Git Keeper/Refresh")]
    private static void ScanProjectForGitKeeps () {
        gitKeepedDirs.Clear();
        string assetDir = Application.dataPath;
        GetGitKeeps(assetDir, gitKeepedDirs);
    }

    static void GetGitKeeps (string dir, List<string> keeps) {
        if (File.Exists(dir + "/.gitkeep")) {
            if (ContainsFiles(dir)) File.Delete(dir + "/.gitkeep");
            else keeps.Add(dir);
        }
        foreach (string subDir in Directory.GetDirectories(dir)) {
            GetGitKeeps(subDir, keeps);
        }
    }

    static bool ContainsFiles (string dir) {
        bool isEmpty = !Directory.EnumerateFiles(dir).Where(s => !s.Contains(".gitkeep")).Any();
        if (!isEmpty) return true;
        foreach (string subDir in Directory.GetDirectories(dir)) {
            if (ContainsFiles(subDir)) return true;
        }
        return false;
    }

    static string GetActiveDirectory () {
        string filepath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (filepath.Length > 0)
        {
            if (!Directory.Exists(filepath))
            {
                filepath = Regex.Replace(filepath, "/[^/]*$", "/");
            }
        }
        return filepath;
    }

    static string GetFileDirectory (string file) {
        string filepath = file;
        if (filepath.Length > 0)
        {
            if (!Directory.Exists(filepath))
            {
                filepath = Regex.Replace(filepath, "/[^/]*$", "/");
            }
        }
        return filepath;
    }
}
