using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Commander
{
    static class Json
    {
        public static string Serialize(object obj) => JsonConvert.SerializeObject(obj, defaultSettings);

        static JsonSerializerSettings defaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //defaultSettings.TypeNameHandling = TypeNameHandling.All
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

    }
}
