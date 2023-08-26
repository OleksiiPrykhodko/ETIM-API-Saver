using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class Feature
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public bool Deprecated { get; set; }
        public string Description { get; set; }
    }
}
