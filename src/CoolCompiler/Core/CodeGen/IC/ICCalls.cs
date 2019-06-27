using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public class ICCallLabel : ICCode
    {
        public ICLabel Name { get; }
        public int OutResult { get; }

        int size;

        public ICCallLabel(ICLabel lb, int r=-1)
        {
            Name = lb;
            OutResult = r;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.sw("$ra", (-4 * size).ToString(), "$sp"));
            lines.Add(WMIPS.addiu("$sp", "$sp", -4 * (size + 1)));
            lines.Add(WMIPS.jal(Name.LFull));
            lines.Add(WMIPS.addiu("$sp", "$sp", 4 * (size + 1)));
            lines.Add(WMIPS.lw("$ra", (-4 * size).ToString(), "$sp"));            
            if (OutResult >= 0)
                lines.Add(WMIPS.sw("$v0", (-4 * OutResult).ToString(), "$sp"));
            return lines;
        }

        public void Set(int s) => size = s;
    }

    public class ICCallAddress : ICCode
    {
        public int Address { get; }
        public int OutResult { get; }

        int size;

        public ICCallAddress(int a, int r=-1)
        {
            Address = a;
            OutResult = r;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.sw("$ra", (-4 * size).ToString(), "$sp"));
            lines.Add(WMIPS.lw("$a0", (-4 * Address).ToString(), "$sp"));
            lines.Add(WMIPS.addiu("$sp", "$sp", -4 * (size + 1)));
            lines.Add(WMIPS.jalr("$ra", "$a0"));
            lines.Add(WMIPS.addiu("$sp", "$sp", 4 * (size + 1)));
            lines.Add(WMIPS.lw("$ra", (-4 * size).ToString(), "$sp"));
            if (OutResult >= 0)
                lines.Add(WMIPS.sw("$v0", (-4 * OutResult).ToString(), "$sp"));
            return lines;
        }
        public void Set(int s) => size = s;
    }
}
