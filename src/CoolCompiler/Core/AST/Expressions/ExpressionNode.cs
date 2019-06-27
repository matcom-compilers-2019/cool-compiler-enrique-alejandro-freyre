using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SemanticCheck;

namespace Core.AST.Expressions
{
    public abstract class ExpressionNode : ASTNode
    {
        public SemanticCheck.Type StaticType = SemanticCheck.Type.OBJECT;
        public ExpressionNode(int line, int column) : base(line, column)
        {
        }
    }
}
