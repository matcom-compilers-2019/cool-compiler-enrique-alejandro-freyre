using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Operators.Binarys.Comparison
{
    public class LessNode : ComparisonOperatorNode
    {
        public override string Symbol => "<";

        public LessNode(int line, int column) : base(line, column)
        {
        }
    }
}
