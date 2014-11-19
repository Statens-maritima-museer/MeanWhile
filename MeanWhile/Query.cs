using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MeanWhile
{
    [DataContract]
    public class Query
    {
        //[DataMember]
        //public IEnumerable<Page> pages { get; set; }

        [DataMember]
        public dynamic pages { get; set; }

        // "query":{
        //          "pages":{
        //                   "1753":{"pageid":1753,"ns":0,"title":"Djurg\u00e5rden","coordinates":[{"lat":59.3256,"lon":18.1225,"primary":"","globe":"earth"}]},
        //                   "742176":{"pageid":742176,"ns":0,"title":"Djurg\u00e5rden (olika betydelser)"},
        //                   "68397":{"pageid":68397,"ns":0,"title":"Kungliga Djurg\u00e5rden"},"1138956":{"pageid":1138956,"ns":0,"title":"Skansenberget","coordinates":[{"lat":59.3242,"lon":18.102,"primary":"","globe":"earth"}]},"1138495":{"pageid":1138495,"ns":10,"title":"Mall:Djurg\u00e5rden"},"1112064":{"pageid":1112064,"ns":0,"title":"Alberget","coordinates":[{"lat":59.3228,"lon":18.1031,"primary":"","globe":"earth"}]},"614682":{"pageid":614682,"ns":0,"title":"Alhambra, Stockholm","coordinates":[{"lat":59.3247,"lon":18.1006,"primary":"","globe":"earth"}]},"59430":{"pageid":59430,"ns":0,"title":"Allm\u00e4nna konst- och industriutst\u00e4llningen","coordinates":[{"lat":59.3272,"lon":18.0978,"primary":"","globe":"earth"}]},"954166":{"pageid":954166,"ns":0,"title":"Andr\u00e9egatan","coordinates":[{"lat":59.3234,"lon":18.1003,"primary":"","globe":"earth"}]},"559586":{"pageid":559586,"ns":0,"title":"Aquaria vattenmuseum","coordinates":[{"lat":59.3253,"lon":18.0942,"primary":"","globe":"earth"}]}}}}
    }
}
