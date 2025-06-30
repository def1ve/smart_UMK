using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace smart_UMK.ViewModels
{
    public class FileViewModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public List<string> FormattedTexts { get; set; } // 
    }
}
