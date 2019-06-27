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
    public class VoidNode : AtomNode
    {
        public string GetType { get;}

        public VoidNode(string type, int line = 0, int column = 0) : base(line, column)
        {
            GetType = type;
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            StaticType = scope.GetType(GetType);
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.codeLines.Add(new ICAssignNullToVar(codeManager.variableManager.PeekCounter()));
        }
    }
}
