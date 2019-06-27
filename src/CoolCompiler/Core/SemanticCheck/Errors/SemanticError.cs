using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SemanticCheck.Errors
{
    public enum TypeError {
        RepeatClass,
        InheritCycle,
        NotClassMainDefine,
        NotMethodMainInClassMain,
        ClassNotDefine,
        InvalidInherits,
        RepeatMethod,
        RepeatVariableName,
        VariableNotDefineInScope,
        InconsistentType,
        NotMethodInClass,
        InvalidNumberOfParams,
        InvalidSintax,
        InvalidCase
    };
    public class SemanticError
    {
        public int Line;
        public int Column;
        public TypeError Type;
        public SemanticError(int line, int column, TypeError type)
        {
            Line = line;
            Column = column;
            Type = type;
        }
    }
}
