using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Atomics
{
    public class SelfNode : AtomNode
    {
        public SelfNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.codeLines.Add(new ICAssignVarToVar(codeManager.variableManager.PeekCounter(), 0));
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            StaticType = scope.Type;
        }
    }
}
