namespace DCL
{
    public class DataStore_WSCommunication
    {
        [System.NonSerialized]
#if UNITY_ANDROID && !UNITY_EDITOR
        public string url = "ws://127.0.0.1:5000/";
#else
        public string url = "ws://localhost:5000/"; 
#endif
        

        public readonly BaseVariable<bool> communicationEstablished = new BaseVariable<bool>();
        public readonly BaseVariable<bool> communicationReady = new BaseVariable<bool>();
    }
}