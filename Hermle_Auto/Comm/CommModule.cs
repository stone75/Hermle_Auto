namespace HermleCS.Comm
{
    public abstract class CommModule
    {
        public abstract bool readMessage(string deviceid, int length, out string readVal);
        public abstract bool sendMessage(string deviceid, int length, int[] val);
        public abstract string test();
    }
}

