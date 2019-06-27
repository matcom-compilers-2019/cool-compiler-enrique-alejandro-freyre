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
    public class IdNode : AtomNode
    {
        public string Text { get; set; }
        public IdNode(int line, int column, string t) : base(line, column)
        {
            Text = t;
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!scope.IsDefined(Text, out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.VariableNotDefineInScope));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            var (x, type) = codeManager.variableManager.Peek(Text);
            if (x != -1)
                codeManager.codeLines.Add(new ICAssignVarToVar(codeManager.variableManager.PeekCounter(), x));
            else
                codeManager.codeLines.Add(new ICAssignMemToVar(codeManager.variableManager.PeekCounter(), 0, 
                    codeManager.virtualTable.getOffset(codeManager.variableManager.ClassName, Text)));
            if (codeManager.special_object_return_type)
                codeManager.SetReturnType(type);
        }
    }
}
