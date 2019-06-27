using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public class ICJump : ICCode
    {
        public ICLabel Label { get; }
        public ICJump(ICLabel label)
        {
            Label = label;
        }
        public override List<string> Generate()
        {
            return new List<string>() { WMIPS.j(Label.LFull) };
        }
    }

    public class ICCondJump : ICCode
    {
        public int Condition { get; }
        public ICLabel Label { get; }
        public ICCondJump(int cond, ICLabel label)
        {
            Condition = cond;
            Label = label;
        }

        public override List<string> Generate()
        {
            return new List<string>() {
            WMIPS.lw("$a0", (-Condition * 4).ToString(), "$sp"),
            WMIPS.beqz("$a0", Label.LFull) };
        }
    }
}