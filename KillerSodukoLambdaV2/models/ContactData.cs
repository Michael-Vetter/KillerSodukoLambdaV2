using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KillerSodukoLambdaV2.models
{
    public class ContactData
    {
        [JsonProperty("context")]
        public string Context { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
     }
}
