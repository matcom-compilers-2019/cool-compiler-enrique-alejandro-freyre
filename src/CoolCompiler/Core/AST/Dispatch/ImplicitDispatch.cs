using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.AST.Feature;
using Core.CodeGen;
using Core.CodeGen.IC;

namespace Core.AST.Dispatch
{
    public class ImplicitDispatch : DispatchNode
    {
        public ImplicitDispatch(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            string _class = codeManager.variableManager.ClassName;
            codeManager.codeLines.Add(new ICAssignVarToVar(codeManager.variableManager.PeekCounter(), 0));
            codeManager.Dispatch(this,_class);
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            foreach (var arg in Arguments)
                arg.SecondSemanticCheck(errors, scope);

            if (!scope.IsDefined(IdMethod.Text, Arguments.Select(x => x.StaticType).ToArray(), out StaticType))
                errors.Add(new SemanticError(Line, Column, TypeError.NotMethodInClass));
        }
    }
}
