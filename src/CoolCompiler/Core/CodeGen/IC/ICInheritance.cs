using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public class ICInheritance : ICCode
    {
        public string Child { get; }
        public string Parent { get; }

        public ICInheritance(string c, string p)
        {
            Child = c;
            Parent = p;
        }
        public override List<string> Generate()
        {
            return new List<string>();
        }
    }
}
