using System.Collections.Generic;

namespace LifeApi.Models
{
    public class LifeDatabaseSettings : ILifeDatabaseSettings
    {
        public Dictionary<string,string> CollectionNames { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface ILifeDatabaseSettings
    {
        Dictionary<string,string> CollectionNames { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}