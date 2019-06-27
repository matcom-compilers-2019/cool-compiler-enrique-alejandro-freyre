using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.AST.Class;
using Core.AST.Feature;
using Core.AST.Type;
using Core.SemanticCheck;

namespace Core.SemanticCheck.Scopes
{
    public class Scope
    {
        Dictionary<string, Type> variables = new Dictionary<string, Type>();

        Dictionary<string, (Type[] Args, Type ReturnType)> methods = new Dictionary<string, (Type[], Type)>();

        static Dictionary<string, Type> declaredTypes = new Dictionary<string, Type>();

        public Scope Parent { get; set; }
        public Type Type { get; set; }

        public Scope()
        {
        }

        public Scope(Scope parent, Type type)
        {
            Parent = parent;
            Type = type;
        }

        static Scope()
        {
            declaredTypes.Add("Object", Type.OBJECT);
            declaredTypes["Object"].ClassReference = new ClassNode(-1, -1, "Object", "NULL");
            declaredTypes["Object"].ClassReference.Scope = new Scope(NULL, declaredTypes["Object"]);

            declaredTypes.Add("Bool", new Type { Text = "Bool", Parent = declaredTypes["Object"], Level = 1, ClassReference = new ClassNode(-1, -1, "Bool", "Object") });
            declaredTypes.Add("Int", new Type { Text = "Int", Parent = declaredTypes["Object"], Level = 1, ClassReference = new ClassNode(-1, -1, "Int", "Object") });
            declaredTypes.Add("String", new Type { Text = "String", Parent = declaredTypes["Object"], Level = 1, ClassReference = new ClassNode(-1, -1, "String", "Object") });
            declaredTypes.Add("IO", new Type { Text = "IO", Parent = declaredTypes["Object"], Level = 1, ClassReference = new ClassNode(-1, -1, "IO", "Object") });

            declaredTypes["Bool"].ClassReference.Scope = new Scope(declaredTypes["Object"].ClassReference.Scope, declaredTypes["Bool"]);
            declaredTypes["Int"].ClassReference.Scope = new Scope(declaredTypes["Object"].ClassReference.Scope, declaredTypes["Int"]);
            declaredTypes["String"].ClassReference.Scope = new Scope(declaredTypes["Object"].ClassReference.Scope, declaredTypes["String"]);
            declaredTypes["IO"].ClassReference.Scope = new Scope(declaredTypes["Object"].ClassReference.Scope, declaredTypes["IO"]);

            declaredTypes["Object"].ClassReference.Scope.Define("abort", new Type[0], declaredTypes["Object"]);
            declaredTypes["Object"].ClassReference.Scope.Define("type_name", new Type[0], declaredTypes["String"]);
            declaredTypes["Object"].ClassReference.Scope.Define("copy", new Type[0], declaredTypes["Object"]);

            declaredTypes["String"].ClassReference.Scope.Define("length", new Type[0], declaredTypes["Int"]);
            declaredTypes["String"].ClassReference.Scope.Define("concat", new Type[1] { declaredTypes["String"] }, declaredTypes["String"]);
            declaredTypes["String"].ClassReference.Scope.Define("substr", new Type[2] { declaredTypes["Int"], declaredTypes["Int"] }, declaredTypes["String"]);

            declaredTypes["IO"].ClassReference.Scope.Define("out_string", new Type[1] { declaredTypes["String"] }, declaredTypes["IO"]);
            declaredTypes["IO"].ClassReference.Scope.Define("out_int", new Type[1] { declaredTypes["Int"] }, declaredTypes["IO"]);
            declaredTypes["IO"].ClassReference.Scope.Define("in_string", new Type[0], declaredTypes["String"]);
            declaredTypes["IO"].ClassReference.Scope.Define("in_int", new Type[0], declaredTypes["Int"]);

        }

        public static void Clear()
        {
            var tmp = new Dictionary<string, Type>();
            HashSet<string> builtin = new HashSet<string> { "Object", "Bool", "Int", "String", "IO" };
            foreach (var item in declaredTypes)
                if (builtin.Contains(item.Key))
                    tmp.Add(item.Key, item.Value);
            declaredTypes = tmp;
        }

        public virtual bool IsDefined(string name, out Type type)
        {
            if (variables.TryGetValue(name, out type))
                return true;
            if (Parent != null && Parent.IsDefined(name, out type))
                return true;
            type = Type.OBJECT;
            return false;
        }

        public virtual bool IsDefined(string name, Type[] args, out Type type)
        {
            type = Type.OBJECT;
            if (methods.ContainsKey(name) && methods[name].Args.Length == args.Length)
            {
                bool ok = true;
                for (int i = 0; i < args.Length; ++i)
                    if (!(args[i] <= methods[name].Args[i]))
                        ok = false;
                if (ok)
                {
                    type = methods[name].ReturnType;
                    return true;
                }
            }

            if (Parent != null && Parent.IsDefined(name, args, out type))
                return true;

            type = Type.OBJECT;
            return false;
        }

        public virtual bool IsDefinedType(string name, out Type type)
        {
            if (declaredTypes.TryGetValue(name, out type))
                return true;
            type = Type.OBJECT;
            return false;
        }

        public virtual bool Define(string name, Type type)
        {
            if (variables.ContainsKey(name))
                return false;
            variables.Add(name, type);
            return true;
        }

        public virtual bool Define(string name, Type[] args, Type type)
        {
            if (methods.ContainsKey(name))
                return false;
            methods[name] = (args, type);
            return true;
        }

        public virtual bool Change(string name, Type type)
        {
            if (!variables.ContainsKey(name))
                variables.Add(name, type);
            variables[name] = type;
            return true;
        }

        public virtual Scope CreateChild()
        {
            return new Scope()
            {
                Parent = this,
                Type = this.Type
            };
        }

        public virtual bool AddType(string name, Type type)
        {
            declaredTypes.Add(name, type);
            return true;
        }

        public virtual Type GetType(string name)
        {
            if (declaredTypes.TryGetValue(name, out Type type))
                return type;
            return Type.OBJECT;
        }

        #region
        private static NullScope nullScope = new NullScope();

        public static NullScope NULL => nullScope;

        public class NullScope : Scope
        {
            public Scope Parent { get; set; }
            public Type Type { get; set; }

            public bool AddType(string name, Type type)
            {
                return false;
            }

            public bool Change(string name, Type type)
            {
                return false;
            }

            public Scope CreateChild()
            {
                return new Scope()
                {
                    Parent = NULL,
                    Type = null
                };
            }

            public bool Define(string name, Type type)
            {
                return false;
            }

            public bool Define(string name, Type[] args, Type type)
            {
                return false;
            }

            public Type GetType(string name)
            {
                return Type.OBJECT;
            }

            public bool IsDefined(string name, out Type type)
            {
                type = Type.OBJECT;
                return false;
            }

            public bool IsDefined(string name, Type[] args, out Type type)
            {
                type = Type.OBJECT;
                return false;
            }

            public bool IsDefinedType(string name, out Type type)
            {
                type = Type.OBJECT;
                return false;
            }
        }
        #endregion
    }
}
