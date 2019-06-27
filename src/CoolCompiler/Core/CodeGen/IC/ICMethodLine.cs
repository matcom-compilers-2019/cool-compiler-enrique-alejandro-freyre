using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public class ICParams : ICCode
    {
        public int Variables { get; }
        public ICParams(int v)
        {
            Variables = v;
        }
        public override List<string> Generate()
        {
            return new List<string>();
        }
    }

    public class ICPushParams : ICCode
    {
        public int Variable { get; }
        int size;
        int pcount;
        public ICPushParams(int v)
        {
            Variable = v;
        }
        public override List<string> Generate()
        {
            return new List<string>() {
                WMIPS.lw("$a0",(-Variable*4).ToString(),"$sp"),
                WMIPS.sw("$a0", (-(size + pcount)*4).ToString(), "$sp")
            };
        }

        public void Set(int p, int s)
        {
            size = s;
            pcount = p;
        }
    }

    public class ICPopParams : ICCode
    {
        public int Amount { get; }
        public ICPopParams(int amount)
        {
            Amount = amount;
        }
        public override List<string> Generate()
        {
            return new List<string>();
        }
    }

    public class ICReturn : ICCode
    {
        public int RetVar { get; }
        public ICReturn(int v = -1)
        {
            RetVar = v;
        }
        public override List<string> Generate()
        {
            return new List<string>()
            {
                WMIPS.lw("$v0",(-RetVar*4).ToString(),"$sp"),
                WMIPS.jr("$ra")
            };
        }
    }
}
