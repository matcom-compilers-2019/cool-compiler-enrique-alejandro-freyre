using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public class ICComent : ICCode
    {
        public string Comment { get; }
        public ICComent(string c)
        {
            Comment = c;
        }
        public override List<string> Generate()
        {
            return new List<string>();
        }
    }
}
