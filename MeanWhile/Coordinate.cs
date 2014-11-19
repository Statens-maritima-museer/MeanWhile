using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MeanWhile
{
    [DataContract]
    public class Coordinate
    {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lon { get; set; }

        [DataMember]
        public string primary { get; set; }

        [DataMember]
        public string globe { get; set; }

        //<co lat="59.3256" lon="18.1225" primary="" globe="earth" />
    }
}
