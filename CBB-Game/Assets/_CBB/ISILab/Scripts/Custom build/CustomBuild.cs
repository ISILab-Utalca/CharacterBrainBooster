#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using CBB.DataManagement;

namespace ISILab
{
    class CustomBuildScripts
    {
        [PostProcessBuild(0)]
        public static void BuildWindows(BuildTarget target, string pathToBuild)
        {
            Debug.Log("Path to build: " + pathToBuild);

            // Copy all .brain files from the project folder to the build folder, alongside the built game.
            string root = DataLoader.Path;
            string[] files = System.IO.Directory.GetFiles(root, "*.brain");

            // Remove the last part of pathToBuild, which is the name of the game
            var destinationFolder = pathToBuild[..pathToBuild.LastIndexOf('/')] + "/CBB Data";
            if (!System.IO.Directory.Exists(destinationFolder))
                System.IO.Directory.CreateDirectory(destinationFolder);

            var brainsDestinationFolder = destinationFolder + "/Brains";
            if (!System.IO.Directory.Exists(brainsDestinationFolder))
                System.IO.Directory.CreateDirectory(brainsDestinationFolder);

            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);
                string dest = System.IO.Path.Combine(brainsDestinationFolder, name);
                System.IO.File.Copy(file, dest, true);
            }
            Debug.Log(files.Length + " brains copied to build folder.");

            // Copy the binding files to the build folder
            root = BindingManager.Path;
            string[] bindingFiles = System.IO.Directory.GetFiles(root, "*.data");
            var bindingsDestinationFolder = destinationFolder + "/Bindings";
            if (!System.IO.Directory.Exists(bindingsDestinationFolder))
                System.IO.Directory.CreateDirectory(bindingsDestinationFolder);
            foreach (string file in bindingFiles)
            {
                string name = System.IO.Path.GetFileName(file);
                string dest = System.IO.Path.Combine(bindingsDestinationFolder, name);
                System.IO.File.Copy(file, dest, true);
            }
            Debug.Log(bindingFiles.Length + " bindings copied to build folder.");
        }
    }
} 
#endif