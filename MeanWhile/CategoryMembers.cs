using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MeanWhile
{
    [DataContract]
    public class CategoryMembers
    {
        [DataMember]
        public string gcmcontinue { get; set; }

        //<categorymembers gcmcontinue="page|292f2b3b35433d3f2f41010e018f0d00|4383" />
    }
}
