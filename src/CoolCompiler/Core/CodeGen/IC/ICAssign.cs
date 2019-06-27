using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;

namespace Core.CodeGen.IC
{
    public abstract class ICAssign<T> : ICCode
    {
        public int L { get; set; }
        public T R { get; set; }
    }

    public class ICAssignVarToMem : ICAssign<int>
    {
        public int Offset { get; }
        public ICAssignVarToMem(int l, int r, int offset = 0)
        {
            L = l;
            R = r;
            Offset = offset;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.lw("$a0", (-R * 4).ToString(), "$sp"));
            lines.Add(WMIPS.lw("$a1", (-L * 4).ToString(), "$sp"));
            lines.Add(WMIPS.sw("$a0", (Offset * 4).ToString(), "$a1"));
            return lines;
        }
    }

    public class ICAssignVarToVar : ICAssign<int>
    {
        public ICAssignVarToVar(int l, int r)
        {
            L = l;
            R = r;
        }

        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.lw("$a0", (-R * 4).ToString(), "$sp"));
            lines.Add(WMIPS.sw("$a0", (-L * 4).ToString(), "$sp"));
            return lines;
        }
    }

    public class ICAssignConstToMem : ICAssign<int>
    {
        public int Offset { get; }
        public ICAssignConstToMem(int l, int r, int o = 0)
        {
            L = l;
            R = r;
            Offset = o;
        }

        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.lw("$a0", (-L * 4).ToString(), "$sp"));
            lines.Add(WMIPS.li("$a1", R));
            lines.Add(WMIPS.sw("$a1", (Offset * 4).ToString(), "$a0"));
            return lines;
        }
    }

    public class ICAssignMemToVar : ICAssign<int>
    {
        public int Offset;
        public ICAssignMemToVar(int l, int r, int offset = 0)
        {
            L = l;
            R = r;
            Offset = offset;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.lw("$a0", (-R * 4).ToString(), "$sp"));
            lines.Add(WMIPS.lw("$a1", (Offset * 4).ToString(), "$a0"));
            lines.Add(WMIPS.sw("$a1", (-L * 4).ToString(), "$sp"));
            return lines;
        }
    }

    public class ICAssignConstToVar : ICAssign<int>
    {
        public ICAssignConstToVar(int l, int r)
        {
            L = l;
            R = r;
        }

        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.li("$a0", R));
            lines.Add(WMIPS.sw("$a0", (-L * 4).ToString(), "$sp"));
            return lines;
        }
    }

    public class ICAssignStrToVar : ICAssign<string>
    {
        public ICAssignStrToVar(int l, string r)
        {
            L = l;
            R = r;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.la("$a0", R));
            lines.Add(WMIPS.sw("$a0", (-L * 4).ToString(), "$sp"));
            return lines;
        }
    }

    public class ICAssignStrToMem : ICAssign<string>
    {
        public int Offset { get; }
        public ICAssignStrToMem(int l, string r, int o = 0)
        {
            L = l;
            R = r;
            Offset = o;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.la("$a0", R));
            lines.Add(WMIPS.lw("$a1", (-4 * L).ToString(), "$sp"));
            lines.Add(WMIPS.sw("$a0", (Offset * 4).ToString(), "$a1"));
            return lines;
        }
    }

    public class ICAssignLabToVar : ICAssign<ICLabel>
    {
        public ICAssignLabToVar(int l, ICLabel r)
        {
            L = l;
            R = r;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.la("$a0", R.LFull));
            lines.Add(WMIPS.sw("$a0", (-L * 4).ToString(), "$sp"));
            return lines;
        }
    }

    public class ICAssignLabToMem : ICAssign<ICLabel>
    {
        public int Offset { get; set; }
        public ICAssignLabToMem(int l, ICLabel r, int o = 0)
        {
            L = l;
            R = r;
            Offset = o;
        }

        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.la("$a0", R.LFull));
            lines.Add(WMIPS.lw("$a1", (-4 * L).ToString(), "$sp"));
            lines.Add(WMIPS.sw("$a0", (Offset * 4).ToString(), "$a1"));
            return lines;
        }
    }

    public class ICAssignNullToVar : ICCode
    {
        public int Var { get; }
        public ICAssignNullToVar(int var)
        {
            Var = var;
        }
        public override List<string> Generate()
        {
            var lines = new List<string>() {
            WMIPS.sw("$zero", (-Var * 4).ToString(), "$sp")};
            return lines;
        }
    }
}
