using UnityEngine;
using UnityEditor;

public class EditorHelpers
{
    [MenuItem("Assets/Copy resources route %&R")]
    public static void GetResourcesRoute()
    {
        // Get the selected file route
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //Debug.Log(path);
        // Remove the file name extension (after ".")
        string[] pathParts = path.Split('.');
        string pathWithoutExtension = pathParts[0];
        //Debug.Log(pathWithoutExtension);
        // Remove all the path before the "Resources" folder using regular expressions
        string resourcesPath = System.Text.RegularExpressions.Regex.Replace(pathWithoutExtension, ".*Resources/", "");
        //Debug.Log(resourcesPath);
        // Copy this route to the clipboard
        TextEditor te = new TextEditor();
        te.text = resourcesPath;
        te.SelectAll();
        te.Copy();
    }
}