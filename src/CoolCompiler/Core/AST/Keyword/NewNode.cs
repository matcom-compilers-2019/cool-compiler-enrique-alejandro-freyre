using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Keyword
{
    public class NewNode : KeywordNode
    {
        public Type.TypeNode Type { get; set; }
        public NewNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!scope.IsDefinedType(this.Type.Text, out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            if (Type.Text == "Int" || Type.Text == "Bool")
                codeManager.codeLines.Add(new ICAssignConstToVar(codeManager.variableManager.PeekCounter(), 0));
            else if (Type.Text == "String")
                codeManager.codeLines.Add(new ICAssignStrToVar(codeManager.variableManager.PeekCounter(), ""));
            else
            {
                int s = codeManager.virtualTable.SizeOf(Type.Text);
                codeManager.codeLines.Add(new ICAllocation(codeManager.variableManager.PeekCounter(), s));
                codeManager.codeLines.Add(new ICPushParams(codeManager.variableManager.PeekCounter()));
                codeManager.codeLines.Add(new ICCallLabel(new ICLabel(Type.Text, "ctor")));
                codeManager.codeLines.Add(new ICPopParams(1));
            }

            if (codeManager.special_object_return_type)
                codeManager.SetReturnType(Type.Text);
        }
    }
}
