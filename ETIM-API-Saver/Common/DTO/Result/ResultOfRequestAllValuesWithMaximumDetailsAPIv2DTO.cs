using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Result
{
    public class ResultOfRequestAllValuesWithMaximumDetailsAPIv2DTO
    {
        public int Total { get; set; }
        public Value[] Values { get; set; }
    }
}
