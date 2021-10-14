using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Domain
{
    public class MongoConnectionAppSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
