namespace CBB.Lib
{
    public interface IAgent
    {
        public AgentData GetInternalState();
        public void InitializeInternalState();
        public void UpdateInternalState();
    }
}