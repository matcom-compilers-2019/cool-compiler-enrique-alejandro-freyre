using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Core.AST.Type;
using Core.CodeGen;
using Core.CodeGen.IC;
using Core.AST.Feature;

namespace Core.AST.Class
{
    public class ClassNode : ASTNode
    {
        public TypeNode TypeClass { get; set; }

        public TypeNode TypeClassInherit { get; set; }

        public Scope Scope { get; set; }

        public List<Feature.FeatureNode> Features { get; set; }

        public ClassNode(ParserRuleContext context) : base(context) { }

        public ClassNode(int line, int column, string className, string classInheritName) : base(line, column)
        {
            TypeClass = new TypeNode(line, column, className);
            TypeClassInherit = new TypeNode(line, column, classInheritName);
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            Scope _scope = new Scope()
            {
                Type = scope.GetType(TypeClass.Text),
                Parent = scope.GetType(TypeClassInherit.Text).ClassReference.Scope
            };

            Scope = _scope;
            foreach (var feat in Features)
            {
                feat.FirstSemanticCheck(errors, Scope);
            }
        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            foreach (var _feat in Features)
                _feat.SecondSemanticCheck(errors, scope);
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            string _class;
            _class = codeManager.variableManager.ClassName = TypeClass.Text;
            codeManager.codeLines.Add(new ICInheritance(TypeClass.Text,
                codeManager.scope.GetType(TypeClass.Text).Parent.Text));

            var self = codeManager.variableManager.VarCount = 0;
            codeManager.variableManager.IncreaseCounter();
            codeManager.variableManager.PushCounter();
            var attr = new List<AttributeNode>();
            var method = new List<MethodNode>();
            foreach (var feature in Features)
            {
                if (feature is AttributeNode)
                    attr.Add(feature as AttributeNode);
                else method.Add(feature as MethodNode);
            }
            foreach (var m in method)
                m.GetIntCode(codeManager);

            // constructor
            codeManager.codeLines.Add(new ICLabel(codeManager.variableManager.ClassName, "ctor"));
            codeManager.codeLines.Add(new ICParams(self));

            // LLamar al constructor del padre
            if (codeManager.variableManager.ClassName != "Object")
            {
                codeManager.codeLines.Add(new ICPushParams(self));
                ICLabel lb = new ICLabel(TypeClassInherit.Text, "ctor");
                codeManager.codeLines.Add(new ICCallLabel(lb));
                codeManager.codeLines.Add(new ICPopParams(1));
            }

            foreach (var m in method)
            {
                (string, string) label = codeManager.virtualTable.getLabel(TypeClass.Text, m.Id.Text);
                var offset = codeManager.virtualTable.getOffset(TypeClass.Text, m.Id.Text);
                codeManager.codeLines.Add(new ICAssignLabToMem(self, new ICLabel(label.Item1, label.Item2),
                    offset));
            }
            foreach (var a in attr)
            {
                codeManager.variableManager.PushCounter();
                a.GetIntCode(codeManager);
                codeManager.variableManager.PopCounter();
                codeManager.codeLines.Add(new ICAssignVarToMem(self, codeManager.variableManager.PeekCounter(),
                    codeManager.virtualTable.getOffset(TypeClass.Text, a.Formal.Id.Text)));
            }
            // Settear el nombre, tamaño, etiqueta
            codeManager.codeLines.Add(new ICAssignStrToMem(0, TypeClass.Text, 0));
            codeManager.codeLines.Add(new ICAssignConstToMem(0, codeManager.virtualTable.SizeOf(TypeClass.Text), 1));
            codeManager.codeLines.Add(new ICAssignLabToMem(0, new ICLabel("_class", TypeClass.Text), 2));
            codeManager.codeLines.Add(new ICReturn(-1));
            codeManager.variableManager.PopCounter();
        }
    }
}
