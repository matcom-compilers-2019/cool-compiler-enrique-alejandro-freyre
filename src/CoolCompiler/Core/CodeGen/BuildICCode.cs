using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.CodeGen.IC;
using Core.AST.Expressions.Operators.Binarys;
using Core.AST.Program;
using Core.AST.Expressions.Operators.Unarys;
using Core.SemanticCheck.Scopes;
using Core.AST.Dispatch;

namespace Core.CodeGen
{
    public class BuildICCode
    {
        ProgramNode root;
        public Scope scope;
        public List<ICCode> codeLines;
        public TrackVar variableManager;
        public TrackClass virtualTable;
        public bool special_object_return_type = false;
        public static int return_type_variable = 1;

        public BuildICCode(ProgramNode root, Scope scope)
        {
            this.root = root;
            this.scope = scope;
            variableManager = new TrackVar();
            virtualTable = new TrackClass(scope);
            codeLines = new List<ICCode>();
            variableManager.PushCounter();
            InitICLines();
            variableManager.PopCounter();
        }

        void InitICLines()
        {
            int self = variableManager.PeekCounter();
            (string, string) label;
            //var objMethods = new List<string> { "abort", "type_name", "copy" };

            codeLines.Add(new ICCallLabel(new ICLabel("start"), -1));

            codeLines.Add(new ICLabel("Object", "ctor"));
            codeLines.Add(new ICParams(self));
            foreach (var idM in TrackClass.Object)
            {
                label = virtualTable.getLabel("Object", idM);
                codeLines.Add(new ICAssignLabToMem(self, new ICLabel(label.Item1, label.Item2), virtualTable.getOffset("Object", idM)));
            }

            codeLines.Add(new ICAssignStrToMem(0, "Object", 0));
            codeLines.Add(new ICAssignConstToMem(0, virtualTable.SizeOf("Object"), 1));

            codeLines.Add(new ICReturn());

            codeLines.Add(new ICLabel("IO", "ctor"));

            codeLines.Add(new ICParams(self));
            codeLines.Add(new ICPushParams(self));
            codeLines.Add(new ICCallLabel(new ICLabel("Object", "ctor")));
            codeLines.Add(new ICPopParams(1));

            foreach (var f in TrackClass.IO)
            {
                label = virtualTable.getLabel("IO", f);
                codeLines.Add(new ICAssignLabToMem(self, new ICLabel(label.Item1, label.Item2), virtualTable.getOffset("IO", f)));
            }

            codeLines.Add(new ICAssignStrToMem(0, "IO", 0));
            codeLines.Add(new ICAssignConstToMem(0, virtualTable.SizeOf("IO"), 1));
            codeLines.Add(new ICAssignLabToMem(0, new ICLabel("_class", "IO"), 2));

            codeLines.Add(new ICReturn());

            codeLines.Add(new ICInheritance("IO", "Object"));
            codeLines.Add(new ICInheritance("Int", "Object"));
            codeLines.Add(new ICInheritance("Bool", "Object"));
            codeLines.Add(new ICInheritance("String", "Object"));

            Wrapper("Int", self);
            Wrapper("Bool", self);
            Wrapper("String", self);

            codeLines.Add(new ICLabel("Object", "abort"));
            codeLines.Add(new ICJump(new ICLabel("_abort")));

            codeLines.Add(new ICLabel("Object", "type_name"));
            codeLines.Add(new ICParams(0));
            codeLines.Add(new ICAssignMemToVar(0, 0, 0));
            codeLines.Add(new ICReturn(0));

            codeLines.Add(new ICLabel("Object", "copy"));
            codeLines.Add(new ICParams(0));
            codeLines.Add(new ICAssignMemToVar(1, 0, 1));
            codeLines.Add(new ICAssignConstToVar(2, 4));
            codeLines.Add(new ICArith(1, 1, 2, "*"));
            codeLines.Add(new ICPushParams(0));
            codeLines.Add(new ICPushParams(1));
            codeLines.Add(new ICCallLabel(new ICLabel("_copy"), 0));
            codeLines.Add(new ICPopParams(2));

            codeLines.Add(new ICReturn(0));

            WIOOut("IO", "out_string");
            WIOOut("IO", "out_int");

            WIOIn("IO", "in_string");
            WIOIn("IO", "in_int");

            //string: substr, concat, length
            codeLines.Add(new ICLabel("String", "length"));
            codeLines.Add(new ICParams(0));
            codeLines.Add(new ICPushParams(0));
            codeLines.Add(new ICCallLabel(new ICLabel("_strlen"), 0));
            codeLines.Add(new ICPopParams(1));
            codeLines.Add(new ICReturn(0));

            codeLines.Add(new ICLabel("String", "concat"));
            codeLines.Add(new ICParams(0));
            codeLines.Add(new ICParams(1));
            codeLines.Add(new ICPushParams(0));
            codeLines.Add(new ICPushParams(1));
            codeLines.Add(new ICCallLabel(new ICLabel("_strcat"), 0));
            codeLines.Add(new ICPopParams(2));
            codeLines.Add(new ICReturn(0));

            codeLines.Add(new ICLabel("String", "substr"));
            codeLines.Add(new ICParams(0));
            codeLines.Add(new ICParams(1));
            codeLines.Add(new ICParams(2));
            codeLines.Add(new ICPushParams(0));
            codeLines.Add(new ICPushParams(1));
            codeLines.Add(new ICPushParams(2));
            codeLines.Add(new ICCallLabel(new ICLabel("_substr"), 0));
            codeLines.Add(new ICPopParams(3));
            codeLines.Add(new ICReturn(0));
        }

        void Wrapper(string className, int self)
        {
            codeLines.Add(new ICLabel("_wrapper", className));
            codeLines.Add(new ICParams(self));
            codeLines.Add(new ICAllocation(self + 1, virtualTable.SizeOf(className) + 1));
            codeLines.Add(new ICPushParams(self + 1));
            codeLines.Add(new ICCallLabel(new ICLabel("Object", "ctor"), self));
            codeLines.Add(new ICPopParams(1));
            codeLines.Add(new ICAssignStrToMem(self + 1, className));
            codeLines.Add(new ICAssignVarToMem(self + 1, self, virtualTable.SizeOf(className)));
            codeLines.Add(new ICAssignLabToMem(self + 1, new ICLabel("_class", className), 2));
            codeLines.Add(new ICReturn(self + 1));
        }

        void WIOOut(string className, string func)
        {
            codeLines.Add(new ICLabel(className, func));
            codeLines.Add(new ICParams(0));
            codeLines.Add(new ICParams(1));
            codeLines.Add(new ICPushParams(1));
            codeLines.Add(new ICCallLabel(new ICLabel("_" + func), 0));
            codeLines.Add(new ICPopParams(1));
            codeLines.Add(new ICReturn(0));
        }

        void WIOIn(string className, string func)
        {
            codeLines.Add(new ICLabel(className, func));
            codeLines.Add(new ICParams(0));
            codeLines.Add(new ICCallLabel(new ICLabel("_" + func), 0));
            codeLines.Add(new ICReturn(0));
        }

        public List<ICCode> GetIntCode()
        {
            root.GetIntCode(this);
            variableManager.PushCounter();

            codeLines.Add(new ICLabel("start"));
            int s = virtualTable.SizeOf("Main");
            codeLines.Add(new ICAllocation(variableManager.PeekCounter(), s));
            codeLines.Add(new ICPushParams(variableManager.PeekCounter()));
            codeLines.Add(new ICCallLabel(new ICLabel("Main", "ctor")));
            codeLines.Add(new ICPopParams(1));
            codeLines.Add(new ICPushParams(variableManager.PeekCounter()));
            codeLines.Add(new ICCallLabel(new ICLabel("Main", "main")));
            codeLines.Add(new ICPopParams(1));

            variableManager.PopCounter();
            return codeLines;
        }

        public void retObject()
        {
            int t;
            var ret = variableManager.PeekCounter();
            var tag = codeLines.Count.ToString();

            variableManager.PushCounter();
            variableManager.IncreaseCounter();
            t = variableManager.VarCount;
            codeLines.Add(new ICAssignStrToVar(t, "Int"));
            codeLines.Add(new ICArith(t, return_type_variable, t, "="));
            codeLines.Add(new ICCondJump(t, new ICLabel("_attempt_bool", tag)));
            codeLines.Add(new ICPushParams(ret));
            codeLines.Add(new ICCallLabel(new ICLabel("_wrapper", "Int"), ret));
            codeLines.Add(new ICPopParams(1));
            codeLines.Add(new ICJump(new ICLabel("_not_more_attempt", tag)));
            variableManager.PopCounter();

            codeLines.Add(new ICLabel("_attempt_bool", tag));
            variableManager.PushCounter();
            variableManager.IncreaseCounter();
            t = variableManager.VarCount;
            codeLines.Add(new ICAssignStrToVar(t, "Bool"));
            codeLines.Add(new ICArith(t, return_type_variable, t, "="));
            codeLines.Add(new ICCondJump(t, new ICLabel("_attempt_string", tag)));
            codeLines.Add(new ICPushParams(ret));
            codeLines.Add(new ICCallLabel(new ICLabel("_wrapper", "Bool"), ret));
            codeLines.Add(new ICPopParams(1));
            codeLines.Add(new ICJump(new ICLabel("_not_more_attempt", tag)));
            variableManager.PopCounter();

            codeLines.Add(new ICLabel("_attempt_string", tag));
            variableManager.PushCounter();
            variableManager.IncreaseCounter();
            t = variableManager.VarCount;
            codeLines.Add(new ICAssignStrToVar(t, "String"));
            codeLines.Add(new ICArith(t, return_type_variable, t, "="));
            codeLines.Add(new ICCondJump(t, new ICLabel("_not_more_attempt", tag)));
            codeLines.Add(new ICPushParams(ret));
            codeLines.Add(new ICCallLabel(new ICLabel("_wrapper", "String"), ret));
            codeLines.Add(new ICPopParams(1));
            variableManager.PopCounter();

            codeLines.Add(new ICLabel("_not_more_attempt", tag));
        }

        public void SetReturnType(string type)
        {
            if (type == "Int" || type == "Bool" || type == "String")
                codeLines.Add(new ICAssignStrToVar(return_type_variable, type));
            else
                codeLines.Add(new ICAssignStrToVar(return_type_variable, "Object"));
        }

        public void BinaryOperationCheck(BinaryOperatorNode n)
        {
            variableManager.PushCounter();

            var count1 = variableManager.IncreaseCounter();
            variableManager.PushCounter();
            n.LeftOperand.GetIntCode(this);
            variableManager.PopCounter();

            var count2 = variableManager.IncreaseCounter();
            variableManager.PushCounter();
            n.RightOperand.GetIntCode(this);
            variableManager.PopCounter();
            variableManager.PopCounter();

            if (n.LeftOperand.StaticType.Text == "String" && n.Symbol == "=")
            {
                codeLines.Add(new ICArith(variableManager.PeekCounter(), count1, count2, "=:="));
                return;
            }
            codeLines.Add(new ICArith(variableManager.PeekCounter(), count1, count2, n.Symbol));
        }

        public void UnaryOperationCheck(UnaryOperatorNode node)
        {
            variableManager.PushCounter();
            variableManager.IncreaseCounter();
            var count1 = variableManager.VarCount;
            variableManager.PushCounter();
            node.Operand.GetIntCode(this);
            variableManager.PopCounter();
            switch (node.Symbol)
            {
                case "not":
                    codeLines.Add(new ICNot(variableManager.PeekCounter(), count1));
                    break;
                case "~":
                    codeLines.Add(new ICNegate(variableManager.PeekCounter(), count1));
                    break;
                case "isvoid":
                    codeLines.Add(new ICIsVoid(variableManager.PeekCounter(), count1));
                    break;
                default:
                    break;
            }
        }

        public void Dispatch(DispatchNode node, string className)
        {
            string method = node.IdMethod.Text;

            if (className == "Int" || className == "Bool" || className == "String")
            {
                if (method == "abort")
                    codeLines.Add(new ICCallLabel(new ICLabel("Object", "abort")));

                else if (method == "type_name")
                    codeLines.Add(new ICAssignStrToVar(variableManager.PeekCounter(), className));

                else if (method == "copy")
                {
                    codeLines.Add(new ICPushParams(variableManager.PeekCounter()));
                    codeLines.Add(new ICCallLabel(new ICLabel("_wrapper", className), variableManager.PeekCounter()));
                    codeLines.Add(new ICPopParams(1));
                }
            }
            else
            {
                variableManager.PushCounter();

                int fAddress = variableManager.IncreaseCounter();
                int offset = virtualTable.getOffset(className, method);

                List<int> parameters = new List<int>();
                List<string> types = virtualTable.getParamTypes(className, method);

                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    variableManager.IncreaseCounter();
                    variableManager.PushCounter();
                    parameters.Add(variableManager.VarCount);
                    node.Arguments[i].GetIntCode(this);

                    if (types[i] == "Object" && (node.Arguments[i].StaticType.Text == "Int"
                        || node.Arguments[i].StaticType.Text == "String" || node.Arguments[i].StaticType.Text == "Bool"))
                    {
                        codeLines.Add(new ICPushParams(variableManager.PeekCounter()));
                        codeLines.Add(new ICCallLabel(new ICLabel("_wrapper", node.Arguments[i].StaticType.Text), variableManager.PeekCounter()));
                        codeLines.Add(new ICPopParams(1));
                    }

                    variableManager.PopCounter();
                }

                variableManager.PopCounter();

                if (className != "String")
                    codeLines.Add(new ICAssignMemToVar(fAddress, variableManager.PeekCounter(), offset));

                codeLines.Add(new ICPushParams(variableManager.PeekCounter()));

                foreach (var param in parameters)
                    codeLines.Add(new ICPushParams(param));

                if (className != "String")
                    codeLines.Add(new ICCallAddress(fAddress, variableManager.PeekCounter()));
                else
                    codeLines.Add(new ICCallLabel(new ICLabel(className, method), variableManager.PeekCounter()));

                if (special_object_return_type)
                    SetReturnType(node.StaticType.Text);

                codeLines.Add(new ICPopParams(parameters.Count + 1));
            }
        }
    }

    public class TrackVar
    {
        List<int> counter;
        public int VarCount { get; set; }

        public string ClassName { get; set; }

        Dictionary<string, List<Tuple<int, string>>> Vars;

        public TrackVar()
        {
            VarCount = 0;
            counter = new List<int>();
            Vars = new Dictionary<string, List<Tuple<int, string>>>();
        }

        public void Push(string varname, string vartype)
        {
            if (!Vars.ContainsKey(varname))
                Vars[varname] = new List<Tuple<int, string>>();
            Vars[varname].Add(new Tuple<int, string>(VarCount, vartype));
        }

        public void Pop(string varname)
        {
            try
            {
                Vars[varname].Reverse();
                Vars[varname].RemoveAt(0);
                Vars[varname].Reverse();
            }
            catch (Exception)
            { return; }

        }

        public Tuple<int, string> Peek(string varname)
        {
            try
            {
                Vars[varname].Reverse();
                var r = Vars[varname][0];
                Vars[varname].Reverse();
                return r;
            }
            catch (Exception)
            { return new Tuple<int, string>(-1, null); }
        }

        public void PushCounter() => counter.Add(VarCount);

        public void PopCounter()
        {
            VarCount = counter[counter.Count - 1];
            counter.RemoveAt(counter.Count - 1);
        }

        public int PeekCounter() => counter[counter.Count - 1];

        public int IncreaseCounter()
        {
            ++VarCount;
            return VarCount;
        }
    }

    public class TrackClass
    {
        Scope scope;
        Dictionary<string, ClassInfo> classTracker;

        public static string[] Object = { "abort", "type_name", "copy" };
        public static string[] IO = { "out_string", "out_int", "in_string", "in_int" };
        public TrackClass(Scope s)
        {
            scope = s;
            classTracker = new Dictionary<string, ClassInfo>();
            insertClass("Object");
            foreach (var item in Object)
                insertMethod("Object", item, new List<string>());
            insertClass("IO");
            insertMethod("IO", "out_string", new List<string>() { "String" });
            insertMethod("IO", "out_int", new List<string>() { "Int" });
            insertMethod("IO", "in_string", new List<string>());
            insertMethod("IO", "in_int", new List<string>());
            insertClass("String");
            insertMethod("String", "length", new List<string>());
            insertMethod("String", "concat", new List<string>() { "String" });
            insertMethod("String", "substr", new List<string>() { "Int", "Int" });
            insertClass("Int");
            insertClass("Bool");
        }

        public void insertClass(string classname)
        {
            classTracker[classname] = new ClassInfo(classname);
            if (classname != "Object")
            {
                scope.IsDefinedType(classname, out SemanticCheck.Type info);
                var p = info.ClassReference.TypeClassInherit.Text;
                //var p = scope.IsDefinedType[classname].TypeClassInherit.Text;
                //classTracker[p] = new ClassInfo(p);
                foreach (var method in classTracker[p].GetMethods())
                {
                    classTracker[classname].SetMethod(method.Key, method.Value);
                }
            }
        }

        public void insertMethod(string classname, string methodname, List<string> paramsTypes)
        {
            if (classname != "Object")
            {
                scope.IsDefinedType(classname, out SemanticCheck.Type info);
                var p = info.ClassReference.TypeClassInherit.Text;
                //var p = scope.InfoClasses[classname].TypeClassInherit.Text;
                int index = classTracker[p].IndexOfMethod(methodname);
                if (index >= 0)
                {
                    classTracker[classname].SetMethod(methodname, classTracker[p].GetMethodParams(methodname));
                    return;
                }
            }
            classTracker[classname].SetMethod(methodname, paramsTypes);
        }

        public (string, string) getLabel(string classname, string item) => classTracker[classname].Label(item);

        public List<string> getParamTypes(string classname, string method) => classTracker[classname].GetMethodParams(method);

        public int getOffset(string classname, string item) => classTracker[classname].IndexOfItem(item) + 3;

        public void insertAttr(string classname, string attr, string type)
        {
            classTracker[classname].SetAttr(attr, type);
        }

        public string getAttrType(string classname, string attr) => classTracker[classname].AttrType(attr);

        public int SizeOf(string classname) => classTracker[classname].Size() + 3;
    }

    public class ClassInfo
    {
        public string Name { get; }

        Dictionary<string, List<string>> methodParamTypes;

        Dictionary<string, string> attrTypes;

        public ClassInfo(string name)
        {
            Name = name;
            methodParamTypes = new Dictionary<string, List<string>>();
            attrTypes = new Dictionary<string, string>();
        }

        public void SetMethod(string methodName, List<string> paramTypes)
        {
            methodParamTypes[methodName] = paramTypes;
        }

        public Dictionary<string, List<string>> GetMethods()
        {
            return methodParamTypes;
        }

        public List<string> GetMethodParams(string method)
        {
            try
            {
                return methodParamTypes[method];
            }
            catch (Exception) { return null; }
        }

        public int IndexOfMethod(string methodname)
        {
            int index = 0;
            foreach (var key in methodParamTypes.Keys)
            {
                if (key == methodname)
                    return index;
                ++index;
            }
            return -1;
        }

        public int IndexOfAttr(string attrname)
        {
            int index = 0;
            foreach (var key in attrTypes.Keys)
            {
                if (key == attrname)
                    return index;
                ++index;
            }
            return -1;
        }

        public int IndexOfItem(string item) => Math.Max(IndexOfMethod(item), IndexOfAttr(item));

        public (string, string) Label(string item)
        {
            if (IndexOfAttr(item) >= 0 || IndexOfMethod(item) >= 0)
                return (Name, item);
            return (null, null);
        }

        public void SetAttr(string attr, string type)
        {
            attrTypes[attr] = type;
        }

        public string AttrType(string attr)
        {
            try
            {
                return attrTypes[attr];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int Size()
        {
            return methodParamTypes.Count + attrTypes.Count;
        }
    }
}
