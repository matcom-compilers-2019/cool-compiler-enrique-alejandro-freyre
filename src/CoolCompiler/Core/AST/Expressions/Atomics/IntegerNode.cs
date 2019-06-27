using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.CodeGen.IC;

namespace Core.AST.Expressions.Atomics
{
    public class IntegerNode : AtomNode
    {
        public int Value { get; set; }

        public IntegerNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!scope.IsDefinedType("Int", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.codeLines.Add(new ICAssignConstToVar(codeManager.variableManager.PeekCounter(), Value));
            if (codeManager.special_object_return_type) codeManager.SetReturnType("Int");
        }
    }
}
