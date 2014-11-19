using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MeanWhile
{
    [DataContract]
    public class Api
    {
        [DataMember]
        public QueryContinue queryContinue { get; set; }
        
        [DataMember]
        public Query query { get; set; }


        //<?xml version="1.0"?>
        //<api>
        //    <query-continue>
        //        <categorymembers gcmcontinue="page|292f2b3b35433d3f2f41010e018f0d00|4383" />
        //    </query-continue>
        //    <query>
        //        <pages>
        //            <page pageid="1753" ns="0" title="DjurgÃ¥rden">
        //                <coordinates>
        //                    <co lat="59.3256" lon="18.1225" primary="" globe="earth" />
        //                </coordinates>
        //            </page>
        //            <page pageid="742176" ns="0" title="DjurgÃ¥rden (olika betydelser)" /><page pageid="68397" ns="0" title="Kungliga DjurgÃ¥rden" /><page pageid="1138956" ns="0" title="Skansenberget"><coordinates><co lat="59.3242" lon="18.102" primary="" globe="earth" /></coordinates></page><page pageid="1138495" ns="10" title="Mall:DjurgÃ¥rden" /><page pageid="1112064" ns="0" title="Alberget"><coordinates><co lat="59.3228" lon="18.1031" primary="" globe="earth" /></coordinates></page><page pageid="614682" ns="0" title="Alhambra, Stockholm"><coordinates><co lat="59.3247" lon="18.1006" primary="" globe="earth" /></coordinates></page><page pageid="59430" ns="0" title="AllmÃ¤nna konst- och industriutstÃ¤llningen"><coordinates><co lat="59.3272" lon="18.0978" primary="" globe="earth" /></coordinates></page><page pageid="954166" ns="0" title="AndrÃ©egatan"><coordinates><co lat="59.3234" lon="18.1003" primary="" globe="earth" /></coordinates></page><page pageid="559586" ns="0" title="Aquaria vattenmuseum"><coordinates><co lat="59.3253" lon="18.0942" primary="" globe="earth" /></coordinates></page></pages></query></api>
    }
}
