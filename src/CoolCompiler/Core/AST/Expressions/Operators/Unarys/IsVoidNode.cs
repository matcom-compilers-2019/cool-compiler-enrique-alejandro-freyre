using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Expressions.Operators.Unarys
{
    public class IsVoidNode : UnaryOperatorNode
    {
        public override string Symbol => "isvoid";

        public IsVoidNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            this.Operand.SecondSemanticCheck(errors, scope);
            if (!scope.IsDefinedType("Bool", out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            if (Operand.StaticType.Text == "Int" || Operand.StaticType.Text == "String" || Operand.StaticType.Text == "Bool")
                codeManager.codeLines.Add(new ICAssignConstToVar(codeManager.variableManager.PeekCounter(), 0));
            else
                codeManager.UnaryOperationCheck(this);
            if (codeManager.special_object_return_type)
                codeManager.SetReturnType("Bool");
        }
    }
}
