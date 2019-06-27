using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public class ICAllocation : ICCode
    {
        public int Var { get; }
        public int Size { get; }
        public ICAllocation(int var, int sz)
        {
            Var = var;
            Size = sz;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.li("$v0", 9));
            lines.Add(WMIPS.li("$a0", 4 * Size));
            lines.Add(WMIPS.SYSCALL);
            lines.Add(WMIPS.sw("$v0", (-4 * Var).ToString(), "$sp"));
            return lines;
        }
    }
}
