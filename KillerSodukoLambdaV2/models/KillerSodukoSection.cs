using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KillerSodukoLambdaV2.models
{
    
    public class KillerSodukoSection
    {
        [JsonProperty("numberOfSpaces")]
        public int NumberOfSpaces { get; set; }
        [JsonProperty("sumOfDigits")]
        public int SumOfDigits { get; set; }
    }
}
