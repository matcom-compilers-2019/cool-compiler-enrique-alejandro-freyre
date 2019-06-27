using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AST.Expressions.Operators.Unarys
{
    public abstract class UnaryOperatorNode : ExpressionNode
    {
        public ExpressionNode Operand { get; set; }

        public abstract string Symbol { get; }

        public UnaryOperatorNode(int line, int column) : base(line, column)
        {
        }
    }
}
