using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace CBB.DataManagement
{
    /// <summary>
    /// Saves and loads bindings between objects characteristics
    /// </summary>
    public static class BindingManager
    {
        private const string BIND_BRAIN_ID_FILENAME = "Brain ID - Brain File Name";
        private const string BIND_AGENT_ID_BRAIN_ID = "Agent ID - Brain ID";
        private const string FILE_FORMAT = ".data";

        private static Binding m_agentIDBrainID;
        private static Binding m_brainIDFileName;


        public static Binding AgentIDBrainID
        {
            get
            {
                m_agentIDBrainID ??= LoadBinding(new DataFileProperties(Path, BIND_AGENT_ID_BRAIN_ID, FILE_FORMAT));
                return m_agentIDBrainID;
            }
            set { m_agentIDBrainID = value; }
        }
        public static Binding BrainIDFileName
        {
            get
            {
                m_brainIDFileName ??= LoadBinding(new DataFileProperties(Path, BIND_BRAIN_ID_FILENAME, FILE_FORMAT));
                return m_brainIDFileName;
            }
            set { m_brainIDFileName = value; }
        }
        public static string Path
        {
            get
            {
#if UNITY_EDITOR
                // load data from the editor folder
                return Application.dataPath + "/_CBB/Configuration/Bindings";
#else
            // load data from the build folder
            var dataPath = Application.dataPath;
            var path = dataPath.Replace("/" + Application.productName +"_Data", "/CBB Data/Bindings");
            return path;
#endif
            }
        }

        /// <summary>
        /// Load the binding between objects from a disk file into memory
        /// </summary>
        /// <param name="path">Path where the association is stored in a file</param>
        /// <param name="fileName">The file's name where the association is stored</param>
        /// <param name="storage">The object that will hold the loaded data from the file</param>
        private static Binding LoadBinding(DataFileProperties dataFile)
        {
            if (!FileExists(dataFile))
            {
                CreateBindingFile(dataFile.GetFullPath());
            }
            var data = "";
            using (var sr = new System.IO.StreamReader(dataFile.GetFullPath()))
            {
                data = sr.ReadToEnd();
            }
            Binding binding = JsonConvert.DeserializeObject<Binding>(data);
            
            if(binding == null)
            {
                binding = new Binding();
                SaveBinding(dataFile, binding);
            }

            return binding;
        }
        private static bool FileExists(DataFileProperties dataFile)
        {
            return System.IO.File.Exists(dataFile.GetFullPath());
        }
        private static void CreateBindingFile(string path)
        {
            using System.IO.StreamWriter sw = System.IO.File.CreateText(path);
            sw.WriteLine(JsonConvert.SerializeObject(new Binding()));
        }
        private static void SaveBinding(DataFileProperties dataFile, Binding binding)
        {
            using System.IO.StreamWriter sw = System.IO.File.CreateText(dataFile.GetFullPath());
            sw.WriteLine(JsonConvert.SerializeObject(binding, Formatting.Indented));
        }
        public static void UpdateAgentIDBrainIDBinding(BehaviourLoader.Memento memento, string newAgentID, string newBrainName)
        {
            // Remove the previous binding from the memory
            if (AgentIDBrainID.data.ContainsKey(memento.SubGroupName))
            {
                AgentIDBrainID.data.Remove(memento.SubGroupName);
            }
            // Add the new binding to the memory
            var brain = BrainDataLoader.GetBrainByName(newBrainName);
            AgentIDBrainID.data.Add(newAgentID, brain.id);
            // Save the new binding to the disk
            DataFileProperties agentBrainAssociationProperties = new(Path, BIND_AGENT_ID_BRAIN_ID, FILE_FORMAT);
            SaveBinding(agentBrainAssociationProperties, AgentIDBrainID);
        }
        public static void SaveBrainIDFilenameBinding(Brain brain)
        {
            if (BrainIDFileName.data.ContainsKey(brain.id))
            {
                BrainIDFileName.data[brain.id] = brain.name;
            }
            else
            {
                BrainIDFileName.data.Add(brain.id, brain.name);
            }
            DataFileProperties bindingProperties = new(Path, BIND_BRAIN_ID_FILENAME, FILE_FORMAT);
            SaveBinding(bindingProperties, BrainIDFileName);
        }
    }
    
    public struct DataFileProperties
    {
        public string path;
        public string fileName;
        public string format;
        public DataFileProperties(string path, string fileName, string format)
        {
            this.path = path;
            this.fileName = fileName;
            this.format = format;
        }
        public readonly string GetFullPath()
        {
            return path + "/" + fileName + format;
        }
    }
    /// <summary>
    /// A link between two types of objects by one of their characteristics
    /// </summary>
    public class Binding
    {
        public Dictionary<string, string> data = new();
        public Binding()
        {
            data = new Dictionary<string, string>
            {
                { "default", "default" }
            };
        }
    }
}
