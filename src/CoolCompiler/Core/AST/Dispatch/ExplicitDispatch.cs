using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;

namespace Core.AST.Dispatch
{
    public class ExplicitDispatch : DispatchNode
    {
        public Expressions.ExpressionNode Base { get; set; }
        public ExplicitDispatch(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            throw new NotImplementedException();
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            Base.SecondSemanticCheck(errors, scope);
            if (!scope.IsDefinedType(Base.StaticType.Text, out SemanticCheck.Type sclass))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));
            if (!(Base.StaticType <= sclass))
                errors.Add(new SemanticError(Base.Line, Base.Column, TypeError.InconsistentType));

            foreach (var arg in Arguments)
                arg.SecondSemanticCheck(errors, scope);

            var c = sclass.ClassReference.Scope;

            if (!(c.IsDefined(IdMethod.Text, Arguments.Select(x => x.StaticType).ToArray(), out StaticType)))
                errors.Add(new SemanticError(Line, Column, TypeError.NotMethodInClass));
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            string _class = Base.StaticType.Text;
            codeManager.codeLines.Add(new ICAssignVarToVar(codeManager.variableManager.PeekCounter(), 0));
            codeManager.Dispatch(this, _class);
        }
    }
}
