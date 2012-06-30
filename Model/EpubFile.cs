using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpubEditor.Model
{
    public class EpubFile
    {
        public string Name { get; set; }
        public string Author { get; set; }

        public string Content { get; set; }
        public string Title { get; set; }    
    }
}
