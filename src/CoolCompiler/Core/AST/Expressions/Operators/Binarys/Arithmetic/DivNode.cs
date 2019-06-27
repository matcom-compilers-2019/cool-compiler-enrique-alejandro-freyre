using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.AST.Expressions.Atomics;
using Core.CodeGen;

namespace Core.AST.Expressions.Operators.Binarys.Arithmetic
{
    public class DivNode : ArithmeticOperatorNode
    {
        public override string Symbol => "/";

        public DivNode(int line, int column) : base(line, column)
        {
        }
    }
}
