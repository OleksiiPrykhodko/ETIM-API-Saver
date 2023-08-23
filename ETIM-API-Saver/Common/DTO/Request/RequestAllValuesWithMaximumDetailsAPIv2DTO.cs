using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Request
{
    public class RequestAllValuesWithMaximumDetailsAPIv2DTO
    {
        public int From { get; set; }
        // Can't be more than 1000
        public int Size { get; set; }
        // "EN"
        public string Languagecode { get; set; }
        // false
        public bool Deprecated { get; set; }
        public Include Include { get; set; }
    }
}
