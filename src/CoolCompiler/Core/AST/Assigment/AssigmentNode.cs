using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Assigment
{
    public class AssigmentNode : Expressions.ExpressionNode
    {
        public Expressions.Atomics.IdNode Id { get; set; }

        public Expressions.ExpressionNode RightExpression { get; set; }

        public Type.TypeNode TypeReturn { get; set; }

        public AssigmentNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            RightExpression.SecondSemanticCheck(errors, scope);
            if (!scope.IsDefined(Id.Text, out SemanticCheck.Type type))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));

            if (!(RightExpression.StaticType <= type))
                errors.Add(new SemanticError(Line, Column, TypeError.InconsistentType));

            StaticType = RightExpression.StaticType;
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            RightExpression.GetIntCode(codeManager);
            var (x, type) = codeManager.variableManager.Peek(Id.Text);
            if (type is null)
                type = codeManager.virtualTable.getAttrType(codeManager.variableManager.ClassName, Id.Text);
            if ((RightExpression.StaticType.Text == "Int" ||
                RightExpression.StaticType.Text == "Bool" ||
                RightExpression.StaticType.Text == "String") &&
                type == "Object")
            {
                codeManager.codeLines.Add(new ICPushParams(codeManager.variableManager.PeekCounter()));
                codeManager.codeLines.Add(new ICCallLabel(new ICLabel("_wrapper", RightExpression.StaticType.Text),
                    codeManager.variableManager.PeekCounter()));
                codeManager.codeLines.Add(new ICPopParams(1));
            }
            if (x != -1)
                codeManager.codeLines.Add(new ICAssignVarToVar(x, codeManager.variableManager.PeekCounter()));
            else
            {
                var offset = codeManager.virtualTable.getOffset(codeManager.variableManager.ClassName, Id.Text);
                codeManager.codeLines.Add(new ICAssignVarToMem(0, codeManager.variableManager.PeekCounter(), offset));
            }
        }
    }
}
