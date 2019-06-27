using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.SemanticCheck.Errors;
using Core.SemanticCheck.Scopes;
using Antlr4.Runtime;
using Core.SemanticCheck;
using Core.CodeGen.IC;
using Core.CodeGen;
using Core.AST.Class;
using Core.AST.Feature;

namespace Core.AST.Program
{
    public class ProgramNode : ASTNode
    {
        public List<Class.ClassNode> ClassNodes { get; set; }

        public ProgramNode(ParserRuleContext context) : base(context) { }

        public ProgramNode(int line, int column) : base(line, column)
        {
        }

        public override void FirstSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            if (!SemanticCheckAlgoritms.TopologicalSort(ClassNodes, errors))
                return;
            foreach (var _class in ClassNodes)
                scope.AddType(_class.TypeClass.Text, new SemanticCheck.Type(_class.TypeClass.Text, scope.GetType(_class.TypeClassInherit.Text), _class));
            int indexMain = -1;
            for (int i = 0; i < ClassNodes.Count; ++i)
                if (ClassNodes[i].TypeClass.Text == "Main")
                    indexMain = i;
            if (indexMain == -1)
            {
                errors.Add(new SemanticError(Line, Column, TypeError.NotClassMainDefine));
                return;
            }

            bool mainFunction = false;
            foreach (var item in ClassNodes[indexMain].Features)
            {
                if (item is Feature.MethodNode)
                {
                    var method = item as Feature.MethodNode;
                    if (method.Id.Text == "main" && method.Arguments.Count == 0)
                        mainFunction = true;
                }
            }

            if (!mainFunction)
            {
                errors.Add(new SemanticError(ClassNodes[indexMain].Line, ClassNodes[indexMain].Column, TypeError.NotMethodMainInClassMain));
                return;
            }

            foreach (var cclass in ClassNodes)
            {
                if (!scope.IsDefinedType(cclass.TypeClassInherit.Text, out SemanticCheck.Type type))
                {
                    errors.Add(new SemanticError(cclass.Line, cclass.Column, TypeError.RepeatClass));
                    return;
                }
                if (new List<string> { "Bool", "Int", "String" }.Contains(type.Text))
                {
                    errors.Add(new SemanticError(cclass.Line, cclass.Column, TypeError.RepeatClass));
                    return;
                }
                cclass.FirstSemanticCheck(errors, scope);
            }

        }

        public override void SecondSemanticCheck(List<SemanticError> errors, Scope scope)
        {
            foreach (var _class in ClassNodes)
                _class.SecondSemanticCheck(errors, _class.Scope);
        }

        public override void GetIntCode(BuildICCode codeManager)
        {
            var classes = new List<ClassNode>();
            classes.AddRange(ClassNodes);
            classes.Sort((x, y) => codeManager.scope.GetType(x.TypeClass.Text) <=
                codeManager.scope.GetType(y.TypeClass.Text) ? 1 : -1);
            foreach (var _class in classes)
            {
                codeManager.virtualTable.insertClass(_class.TypeClass.Text);
                var attr = new List<AttributeNode>();
                var method = new List<MethodNode>();
                foreach (var feature in _class.Features)
                {
                    if (feature is AttributeNode)
                        attr.Add(feature as AttributeNode);
                    else method.Add(feature as MethodNode);
                }
                foreach (var m in method)
                {
                    var p = new List<string>();
                    foreach (var arg in m.Arguments)
                        p.Add(arg.Type.Text);
                    codeManager.virtualTable.insertMethod(_class.TypeClass.Text, m.Id.Text, p);
                }
                foreach (var a in attr)
                    codeManager.virtualTable.insertAttr(_class.TypeClass.Text, a.Formal.Id.Text, a.Formal.Type.Text);
            }
            foreach (var _class in classes)
                _class.GetIntCode(codeManager);
        }
    }
}
