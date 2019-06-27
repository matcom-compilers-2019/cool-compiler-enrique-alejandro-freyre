using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AST.Keyword
{
    public abstract class KeywordNode : Expressions.ExpressionNode
    {
        public KeywordNode(int line, int column) : base(line, column)
        {
        }
    }
}
