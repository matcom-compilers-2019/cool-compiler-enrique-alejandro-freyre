using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CodeGen.IC
{
    public class ICLabel : ICCode
    {
        public string LHead { get; }
        public string LTail { get; }
        public string LFull
        {
            get
            {
                if (String.IsNullOrEmpty(LTail))
                    return LHead;
                return LHead + "." + LTail;
            }
        }
        public ICLabel(string h, string t = null)
        {
            LHead = h;
            LTail = t;
        }

        public override List<string> Generate()
        {
            var lines = new List<string>();
            lines.Add(WMIPS.NEWLINE);
            lines.Add(LFull + ":");
            lines.Add(WMIPS.li("$t9", 0));
            return lines;
        }
    }
}
