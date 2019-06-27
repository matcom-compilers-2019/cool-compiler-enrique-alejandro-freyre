using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace Core.AST.Feature
{
    public abstract class FeatureNode : ASTNode
    {
        public FeatureNode(ParserRuleContext context) : base(context)
        {
        }

        public FeatureNode(int line, int column) : base(line, column)
        {
        }
    }
}
