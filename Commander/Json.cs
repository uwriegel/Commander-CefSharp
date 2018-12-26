using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Commander
{
    static class Json
    {
        public static string Serialize(object obj)
        {
            var defaultSettings = new JsonSerializerSettings
            {
                //defaultSettings.TypeNameHandling = TypeNameHandling.All
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            //    defaultSettings.Converters = [| OptionConverter() :> JsonConverter |]
            return JsonConvert.SerializeObject(obj, defaultSettings);
        }
    }
}
