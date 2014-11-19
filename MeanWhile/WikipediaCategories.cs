using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeanWhile
{
    class WikipediaCategories
    {
        public List<String> Categories { get; set; }

        /// {
        /// "n":"result",
        /// "a":{
        ///         "querytime_sec":0.69513392448425,
        ///         "total_categories_searched":"25",
        ///         "query_url":"http:\/\/tools.wmflabs.org\/catscan2\/catscan2.php?language=sv&depth=1&categories=1600-talet%0D%0Aslag&comb%5Blist%5D=1&format=json"
        ///         },
        /// "*":[
        ///         {"n":"categories","*":[
        ///                                {"n":"list","*":[
        ///                                    {"n":"c","a":{"name":"1600-talet"}},
        ///                                    {"n":"c","a":{"name":"1600-talets_filosofi"}},
        ///                                    {"n":"c","a":{"name":"1600-talet_(decennium)"}},
        ///                                    {"n":"c","a":{"name":"1600-talet_efter_geografiskt_omr\u00e5de"}},
        ///                                    {"n":"c","a":{"name":"1600-talet_i_fiktion"}},
        ///                                    {"n":"c","a":{"name":"1610-talet"}},
        ///                                    {"n":"c","a":{"name":"1620-talet"}},
        ///                                    {"n":"c","a":{"name":"1630-talet"}},
        ///                                    {"n":"c","a":{"name":"1640-talet"}},
        ///                                    {"n":"c","a":{"name":"1650-talet"}},
        ///                                    {"n":"c","a":{"name":"1660-talet"}},
        ///                                    {"n":"c","a":{"name":"1670-talet"}},
        ///                                    {"n":"c","a":{"name":"1680-talet"}},
        ///                                    {"n":"c","a":{"name":"1690-talet"}},
        ///                                    {"n":"c","a":{"name":"Avlidna_1600-talet"}},
        ///                                    {"n":"c","a":{"name":"Barocken"}},
        ///                                    {"n":"c","a":{"name":"Fartyg_sj\u00f6satta_under_1600-talet"}},
        ///                                    {"n":"c","a":{"name":"F\u00f6dda_1600-talet"}},
        ///                                    {"n":"c","a":{"name":"Krig_under_1600-talet"}},
        ///                                    {"n":"c","a":{"name":"Naturkatastrofer_under_1600-talet"}},
        ///                                    {"n":"c","a":{"name":"Personer_under_1600-talet"}}
        ///                                                ],
        ///                                "a":{"root":"1600-talet","depth":1,"count":21}},
        ///                                {"n":"list","*":[{"n":"c","a":{"name":"Slag"}},
        ///                                {"n":"c","a":{"name":"Slag_efter_krig"}},
        ///                                {"n":"c","a":{"name":"Slag_efter_land"}},
        ///                                {"n":"c","a":{"name":"Slag_efter_typ"}}
        ///                                ],
        ///                             "a":{"root":"slag","depth":1,"count":4}}
        ///                             ],
        ///                             "a":{"count":2}}]}


    }
}
