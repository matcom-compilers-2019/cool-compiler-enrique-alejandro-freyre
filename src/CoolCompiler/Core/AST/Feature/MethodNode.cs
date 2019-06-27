using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Core.CodeGen;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.CodeGen.IC;

namespace Core.AST.Feature
{
    public class MethodNode : FeatureNode
    {
        public Expressions.Atomics.IdNode Id { get; set; }

        public Type.TypeNode TypeReturn { get; set; }

        public List<Formal.FormalNode> Arguments { get; set; }

        public Expressions.ExpressionNode Body { get; set; }

        public MethodNode(int line, int column) : base(line, column)
        {
        }

        public MethodNode(ParserRuleContext context) : base(context)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!scope.IsDefinedType(TypeReturn.Text, out SemanticCheck.Type typeReturn))
                errors.Add(new SemanticError(Line,Column,TypeError.ClassNotDefine));

            TypeReturn = new Type.TypeNode(TypeReturn.Line, TypeReturn.Column, typeReturn.Text);

            SemanticCheck.Type[] typeArgs = new SemanticCheck.Type[Arguments.Count];
            for (int i = 0; i < Arguments.Count; ++i)
                if (!scope.IsDefinedType(Arguments[i].Type.Text, out typeArgs[i]))
                    errors.Add(new SemanticError(Line, Column,TypeError.ClassNotDefine));

            scope.Define(Id.Text, typeArgs, typeReturn);
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (Id.Text == "method7")
                Console.WriteLine();

            var scopeMethod = scope.CreateChild();
            foreach (var arg in Arguments)
            {
                if (!scope.IsDefinedType(arg.Type.Text, out SemanticCheck.Type typeArg))
                    errors.Add(new SemanticError(arg.Line, arg.Column, TypeError.ClassNotDefine));
                scopeMethod.Define(arg.Id.Text, typeArg);
            }

            if (!scope.IsDefinedType(TypeReturn.Text, out SemanticCheck.Type typeReturn))
                errors.Add(new SemanticError(Line, Column, TypeError.ClassNotDefine));

            scope.Define(Id.Text, Arguments.Select(x => scope.GetType(x.Type.Text)).ToArray(), typeReturn);
            
            Body.SecondSemanticCheck(errors, scopeMethod);

            if (!(Body.StaticType <= typeReturn))
                errors.Add(new SemanticError(Body.Line, Body.Column, TypeError.InconsistentType));

            TypeReturn = new Type.TypeNode(Body.Line, Body.Column, typeReturn.Text);
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            codeManager.codeLines.Add(new ICLabel(codeManager.variableManager.ClassName, Id.Text));
            codeManager.special_object_return_type = TypeReturn.Text == "Object";
            int self = codeManager.variableManager.VarCount = 0;
            codeManager.codeLines.Add(new ICParams(self));

            if(codeManager.special_object_return_type)
                codeManager.variableManager.IncreaseCounter();
            codeManager.variableManager.IncreaseCounter();

            foreach (var f in Arguments)
            {
                codeManager.codeLines.Add(new ICParams(codeManager.variableManager.VarCount));
                codeManager.variableManager.Push(f.Id.Text, f.Type.Text);
                codeManager.variableManager.IncreaseCounter();
            }
            codeManager.variableManager.PushCounter();
            Body.GetIntCode(codeManager);
            if (codeManager.special_object_return_type)
                codeManager.retObject();
            codeManager.codeLines.Add(new ICReturn(codeManager.variableManager.PeekCounter()));
            codeManager.variableManager.PopCounter();
            foreach (var f in Arguments)
                codeManager.variableManager.Pop(f.Id.Text);
            codeManager.special_object_return_type = false;
        }
    }
}
