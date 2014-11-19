using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MeanWhile
{
    [DataContract]
    public class Page
    {
        [DataMember]
        public string pageId { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public IEnumerable<Coordinate> coordinates { get; set; }

        //            <page pageid="1753" ns="0" title="DjurgÃ¥rden">
        //                <coordinates>
        //                    <co lat="59.3256" lon="18.1225" primary="" globe="earth" />
        //                </coordinates>
        //            </page>
        
    }
}
