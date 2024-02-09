using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CBB.Comunication;
using System.Net.Sockets;
using Newtonsoft.Json;
using ArtificialIntelligence.Utility;

namespace CBB.DataManagement
{
    public static class DataLoader
    {
        private static List<Brain> brains = new();
        public static System.Action<string> BrainUpdated { get; set; }

        public static string Path
        {
            get
            {
#if UNITY_EDITOR
                // load data from the editor folder
                return Application.dataPath + "/Resources/Brains";
#else
            // load data from the build folder
            var dataPath = Application.dataPath;
            var path = dataPath.Replace("/" + Application.productName +"_Data", "");
            return path;
#endif
            }
        }
        /// <summary>
        /// This function is called when loading the game, 
        /// Load Data from the resources folder and save it
        /// in the static variables of the class 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            LoadBrains(Path);
        }

        #region #BRAIN-METHODS
        public static Brain GetBrainByID(string id)
        {
            return brains.First(m => id.Equals(m.brain_ID));
        }
        public static Brain GetBrainByName(string name)
        {
            if (brains.Count == 0) LoadBrains(Path);
            return brains.First(m => name.Equals(m.brain_Name));
        }
        public static string GenerateID()
        {
            return System.Guid.NewGuid().ToString("N");
        }
        public static List<Brain> GetAllBrains()
        {
            if (brains.Count == 0) LoadBrains(Path);
            return brains;
        }

        public static void LoadBrains(string root)
        {
            System.IO.FileInfo[] files = GetAllBrainFiles(root);
            for (int i = 0; i < files.Length; i++)
            {

                if (files[i].FullName.EndsWith(".brain"))
                {
                    var brain = JSONDataManager.LoadData<Brain>(files[i].DirectoryName, files[i].Name);
                    brains.Add(brain);
                }
            }
            Debug.Log("Loaded: " + brains.Count + " brains.");
        }

        public static System.IO.FileInfo[] GetAllBrainFiles(string root)
        {
            System.IO.DirectoryInfo dir = new(root);
            Debug.Log("Loading brains from: " + dir.FullName);
            if (!dir.Exists)
            {
                dir.Create();
            }
            // Clear the brains list to avoid duplicates
            brains.Clear();
            var files = dir.GetFiles("*.brain");
            return files;
        }

        /// <summary>
        /// Save the in-memory brain data to disk.
        /// Optionally reloads the brains from disk.
        /// </summary>
        /// <param name="AgentID"></param>
        /// <param name="brain"></param>
        /// <param name="reloadBrains">If set, automatically load brains into memory</param>
        public static void SaveBrain(Brain brain)
        {
            if (string.IsNullOrEmpty(brain.brain_ID))
            {
                brain.brain_ID = GenerateID();
            }
            if (BrainFileWasRenamed(brain))
            {
                RemoveBrainFile(BindingManager.BrainIDFileName.data[brain.brain_ID]);
            }
            JSONDataManager.SaveData(Path, brain.brain_Name, "brain", brain);
            BindingManager.StoreBrainIDFilenameBinding(brain);
            Debug.Log($"Brain {brain.brain_ID} saved to: {Path}");
        }

        /// <summary>
        /// Verify if the brain file was renamed by comparing the current name 
        /// with the name stored in the binding file
        /// </summary>
        private static bool BrainFileWasRenamed(Brain brain)
        {
            if (BrainFileAlreadyExists(brain.brain_ID))
            {
                return BindingManager.BrainIDFileName.data[brain.brain_ID] != brain.brain_Name;
            }
            return false;
        }
        /// <summary>
        /// Verify if the brain file already exists by the brain's ID stored in the data file
        /// </summary>
        /// <param name="brainID"></param>
        /// <returns>true if the file exists, false otherwise</returns>
        private static bool BrainFileAlreadyExists(string brainID)
        {
            return BindingManager.BrainIDFileName.data.ContainsKey(brainID);
        }
        private static void RemoveBrainFile(string brainFileName)
        {
            System.IO.File.Delete(Path + "/" + brainFileName + ".brain");
            // Delete meta
            System.IO.File.Delete(Path + "/" + brainFileName + ".brain.meta");
        }
        /// <summary>
        /// When a new client connects, sends all the brains to it
        /// </summary>
        public static void SendBrains(TcpClient client)
        {
            var brains = GetAllBrains();
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                //Formatting = Formatting.Indented
            };

            var serializedBrains = JsonConvert.SerializeObject(brains, settings);
            Debug.Log(serializedBrains);
            Server.SendMessageToClient(client, serializedBrains);

            var considerationEvaluationMethods = ConsiderationMethods.GetAllMethodNames();
            var serializedMethods = JsonConvert.SerializeObject(considerationEvaluationMethods, settings);
            Server.SendMessageToClient(client, serializedMethods);
        }
        #endregion
    }

    [System.Serializable]
    public class PairBrainData
    {
        [System.Serializable]
        public class PairBrain
        {
            public string data_ID;
            public string brain_ID;
        }

        public List<PairBrain> pairs = new();

        internal void Add(PairBrain pairBrain)
        {
            if (pairs.Any(p => p.data_ID == pairBrain.data_ID))
            {
                Debug.LogWarning(" Data ID already exists.");
                return;
            }

            pairs.Add(pairBrain);
        }
    } 
}