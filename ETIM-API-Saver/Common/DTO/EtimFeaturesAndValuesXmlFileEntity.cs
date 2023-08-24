using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Common.DTO
{
    public class EtimFeaturesAndValuesXmlFileEntity
    {
        public string Name { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public string ContactInformation { get; set; } = string.Empty;

        public int NumberOfEtimFeatures { get; set; }
        public int NumberOfEtimValues { get; set; }

        public Feature[] EtimFeatures { get; set; } = new Feature[0];
        public Value[] EtimValues { get; set; } = new Value[0];
    }
}
