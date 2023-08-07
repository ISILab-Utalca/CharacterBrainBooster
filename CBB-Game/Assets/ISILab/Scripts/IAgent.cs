namespace CBB.Lib
{
    public interface IAgent
    {
        public AgentData AgentData { get; set; }
        public AgentData GetInternalState();
        public void InitializeInternalState();
        public void UpdateInternalState();
    }
}