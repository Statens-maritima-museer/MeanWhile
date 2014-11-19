using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeanWhile
{
    public class Category
    {

        public int Index { get; set; }

        public string ShortText { get; set; }

        public string LongText { get; set; }

        public string FileNameAppendix { get; set; }

        public List<Category> CombinedTexts { get; set; }

        public string ImageText { get; set; }
    }
}
