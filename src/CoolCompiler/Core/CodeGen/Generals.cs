using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen.IC;

namespace Core.CodeGen
{
    public class Generals
    {
        public Dictionary<string, int> FVarSize;
        public Dictionary<string, (int, int)> Flimits;
        public Dictionary<string, int> FPCount;
        public Dictionary<string, int> StrCount;
        public Dictionary<string, string> Inherit;

        int _line;
        string function;
        int str_count;
        public Generals(List<ICCode> code)
        {
            FVarSize = new Dictionary<string, int>();
            Flimits = new Dictionary<string, (int, int)>();
            FPCount = new Dictionary<string, int>();
            StrCount = new Dictionary<string, int>();
            Inherit = new Dictionary<string, string>();
            str_count = 0;
            for (_line = 0; _line < code.Count; ++_line)
            {
                Review(code[_line]);
            }
        }
        void Review(ICCode line)
        {
            if (line is ICLabel)
            {
                var l = line as ICLabel;
                if (l.LHead[0] != '_')
                {
                    function = l.LFull;
                    FVarSize[function] = 0;
                    Flimits[function] = (_line, -1);
                    FPCount[function] = 0;
                }
            }
            if (line is ICAllocation)
            {
                FVarSize[function] = Math.Max(FVarSize[function],((ICAllocation)line).Var + 1);
            }
            if (line is ICAssignVarToVar)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICAssignVarToVar)line).L + 1);
            }
            if (line is ICAssignMemToVar)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICAssignMemToVar)line).L + 1);
            }
            if (line is ICAssignConstToVar)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICAssignConstToVar)line).L + 1);
            }
            if (line is ICAssignStrToVar)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICAssignStrToVar)line).L + 1);
                if (!StrCount.ContainsKey(((ICAssignStrToVar)line).R))
                    StrCount[((ICAssignStrToVar)line).R] = str_count++;
            }
            if (line is ICAssignLabToVar)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICAssignLabToVar)line).L + 1);
            }
            if (line is ICBinary)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICBinary)line).Destination + 1);
            }
            if (line is ICUnary)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICUnary)line).Destination + 1);
            }
            if (line is ICParams)
            {
                FVarSize[function] = Math.Max(FVarSize[function], ((ICParams)line).Variables + 1);
                ++FPCount[function];
            }
            if (line is ICReturn)
            {
                Flimits[function] = (Flimits[function].Item1, _line);
            }
            if (line is ICAssignStrToMem)
            {
                if (!StrCount.ContainsKey(((ICAssignStrToMem)line).R))
                    StrCount[((ICAssignStrToMem)line).R] = str_count++;
            }
            if (line is ICInheritance)
            {
                var tmp = line as ICInheritance;
                Inherit[tmp.Child] = tmp.Parent;
                if (!StrCount.ContainsKey(tmp.Child))
                    StrCount[tmp.Child] = str_count++;
                if (!StrCount.ContainsKey(tmp.Parent))
                    StrCount[tmp.Parent] = str_count++;
            }
        }
    }
}