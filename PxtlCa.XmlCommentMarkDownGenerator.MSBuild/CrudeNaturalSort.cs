using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxtlCa.XmlCommentMarkDownGenerator.MSBuild
{
    public class CrudeNaturalSort : IComparer<string>
    {
        public static CrudeNaturalSort Instance {get;} = new CrudeNaturalSort();

        public int Compare(string x, string y)
        {            
            return int.TryParse(x, out int ix) && int.TryParse(y, out int iy)
                ? ix.CompareTo(iy)
                : string.Compare(x, y);  
        }
    }
}
