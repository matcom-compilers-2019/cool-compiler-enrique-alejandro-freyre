using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AST.Dispatch
{
    public abstract class DispatchNode : Expressions.ExpressionNode
    {
        public Expressions.Atomics.IdNode IdMethod { get; set; }

        public List<Expressions.ExpressionNode> Arguments { get; set; }

        public DispatchNode(int line, int column) : base(line, column)
        {
        }
    }
}
