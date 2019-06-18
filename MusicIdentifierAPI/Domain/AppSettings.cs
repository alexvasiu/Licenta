using System;

namespace MusicIdentifierAPI.Domain
{
    [Serializable]
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ConnectionString { get; set; }
    }
}