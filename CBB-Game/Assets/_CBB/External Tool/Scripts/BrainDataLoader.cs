using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CBB.Comunication;
using System.Net.Sockets;
using Newtonsoft.Json;
using ArtificialIntelligence.Utility;

namespace CBB.DataManagement
{
    public static class BrainDataLoader
    {
        private static List<Brain> m_brains = new();
        public static System.Action<Brain> BrainUpdated { get; set; }

        public static string Path
        {
            get
            {
#if UNITY_EDITOR
                // load data from the editor folder
                return Application.dataPath + "/_CBB/Configuration/Brains";
#else
            // load data from the build folder
            var dataPath = Application.dataPath;
            var path = dataPath.Replace("/" + Application.productName +"_Data", "/CBB Data/Brains");
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
            LoadBrains();
        }

        #region #BRAIN-METHODS
        public static Brain GetBrainByID(string id)
        {
            return m_brains.First(m => id.Equals(m.id));
        }
        public static Brain GetBrainByName(string name)
        {
            if (m_brains.Count == 0) LoadBrains();
            return m_brains.First(m => name.Equals(m.name));
        }
        public static string GenerateID()
        {
            return System.Guid.NewGuid().ToString("N");
        }
        public static List<Brain> GetAllBrains()
        {
            if (m_brains.Count == 0) LoadBrains();
            return m_brains;
        }

        public static void LoadBrains()
        {
            System.IO.FileInfo[] files = GetAllBrainFiles();
            for (int i = 0; i < files.Length; i++)
            {

                if (files[i].FullName.EndsWith(".brain"))
                {
                    var brain = JSONDataManager.LoadData<Brain>(files[i].DirectoryName, files[i].Name);
                    m_brains.Add(brain);
                }
            }
        }

        public static System.IO.FileInfo[] GetAllBrainFiles()
        {
            System.IO.DirectoryInfo dir = new(Path);
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
            if (string.IsNullOrEmpty(brain.id))
            {
                brain.id = GenerateID();
            }
            if (BrainFileWasRenamed(brain))
            {
                RemovePreviousBrainFile(BindingManager.BrainIDFileName.data[brain.id]);
            }
            JSONDataManager.SaveData(Path, brain.name + ".brain", brain);
            BindingManager.SaveBrainIDFilenameBinding(brain);
        }

        /// <summary>
        /// Verify if the brain file was renamed by comparing the current name 
        /// with the name stored in the binding file
        /// </summary>
        private static bool BrainFileWasRenamed(Brain brain)
        {
            if (BrainFileAlreadyExists(brain.id))
            {
                return BindingManager.BrainIDFileName.data[brain.id] != brain.name;
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
        private static void RemovePreviousBrainFile(string brainFileName)
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
            Server.SendMessageToClient(client, serializedBrains);

            var considerationEvaluationMethods = ConsiderationMethods.GetAllMethodNames();
            var serializedMethods = JsonConvert.SerializeObject(considerationEvaluationMethods, settings);
            Server.SendMessageToClient(client, serializedMethods);
        }
        #endregion
    }
}