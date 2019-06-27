using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;

namespace Core.AST.Expressions.Operators.Binarys
{
    public abstract class BinaryOperatorNode : ExpressionNode
    {
        public ExpressionNode LeftOperand { get; set; }

        public abstract string Symbol { get; }

        public ExpressionNode RightOperand { get; set; }

        public BinaryOperatorNode(int line, int column) : base(line, column)
        {
        }
    }
}
