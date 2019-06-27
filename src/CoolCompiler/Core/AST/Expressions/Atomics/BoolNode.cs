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
    public class BoolNode : AtomNode
    {
        public BoolNode(int line, int column) : base(line, column)
        {
        }

        public bool Value { get; set; }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.codeLines.Add(new ICAssignConstToVar(codeManager.variableManager.PeekCounter(), Value ? 1 : 0));
            if (codeManager.special_object_return_type) codeManager.SetReturnType("Bool");
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!scope.IsDefinedType("Bool", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }
    }
}
