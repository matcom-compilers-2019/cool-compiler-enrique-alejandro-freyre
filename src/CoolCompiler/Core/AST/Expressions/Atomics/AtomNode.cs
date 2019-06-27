using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AST.Expressions.Atomics
{
    public abstract class AtomNode : ExpressionNode
    {
        public AtomNode(int line, int column) : base(line, column)
        {
        }
    }
}
