using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Operators.Binarys.Arithmetic
{
    public class MulNode : ArithmeticOperatorNode
    {
        public override string Symbol => "*";

        public MulNode(int line, int column) : base(line, column)
        {
        }
    }
}
