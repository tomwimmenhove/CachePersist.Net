using System;

namespace CachePersist.Net.Persistence
{
    public class JsonStringSavingEventArgs : EventArgs
    {
        public string Json { get; }

        public JsonStringSavingEventArgs(string json)
        {
            Json = json;
        }
    }
}
