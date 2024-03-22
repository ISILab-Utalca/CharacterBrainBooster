using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ProjectWindowUtilWrapper 
{
    
    [MenuItem("Assets/Create/CBB Script Templates/Agent Action")]
    public static void CreateActionFromTemplate()
    {
        string templatePath = Application.dataPath + "/_CBB/ScriptTemplates/80-Custom Templates__C# Action-NewActionScript.cs.txt";
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewScript.cs");
    }
}
