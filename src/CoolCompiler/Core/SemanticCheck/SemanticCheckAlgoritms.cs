using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.AST.Class;
using Core.SemanticCheck.Errors;

namespace Core.SemanticCheck
{
    public static class SemanticCheckAlgoritms
    {
        enum Color { White, Gray, Black};
        static int GetHashClass(string className, Dictionary<string, int> map) =>
            map.ContainsKey(className) ? map[className] : -1;
        
        public static bool TopologicalSort(List<ClassNode> classes, List<SemanticError> errors)
        {
            int n = classes.Count;
            var map = new Dictionary<string, int>();
            var mk = new Color[n];
            var tp = new List<int>();
            var g = new List<int>[n];
            for (int i = 0; i < n; i++) g[i] = new List<int>();

            for (int i = 0; i < n; i++)
            {
                if (map.ContainsKey(classes[i].TypeClass.Text))
                {
                    errors.Add(new SemanticError(classes[i].Line, classes[i].Column, TypeError.RepeatClass));
                    return false;
                }
                else map[classes[i].TypeClass.Text] = i;
            }

            foreach (var c in classes)
            {
                var hashc = GetHashClass(c.TypeClass.Text, map);
                var hashi = GetHashClass(c.TypeClassInherit.Text, map);
                if (hashi != -1) g[hashc].Add(hashi);
            }

            for (int i = 0; i < n; i++)
            {
                int id1, id2;
                if (mk[i] == Color.White && !Dfs(i, tp, mk, g, out id1, out id2))
                {
                    errors.Add(new SemanticError(classes[i].TypeClass.Line, classes[i].TypeClass.Column, TypeError.InheritCycle));
                    return false;
                }
            }
            var cs = new ClassNode[n];
            for (int i = 0; i < n; i++) cs[i] = classes[tp[i]];
            for (int i = 0; i < n; i++) classes[i] = cs[i];
            return true;
        }
        static bool Dfs(int c, List<int> tp, Color[] mk, List<int>[] g, out int id1, out int id2)
        {
            id1 = 0;
            id2 = 0;
            mk[c] = Color.Gray;
            foreach (var v in g[c])
            {
                if (mk[v] == Color.Gray)
                {
                    id1 = c;
                    id2 = v;
                    return false;
                }
                if (mk[v] == Color.White && !Dfs(v, tp, mk, g, out id1, out id2))
                    return false;
            }
            tp.Add(c);
            mk[c] = Color.Black;
            return true;
        }

        public static Type LowerCommonAncestor(Type type1, Type type2)
        {
            while (type1.Level < type2.Level) type2 = type2.Parent;
            while (type2.Level < type1.Level) type1 = type1.Parent;
            while (type1 != type2) { type1 = type1.Parent; type2 = type2.Parent; }
            return type1;
        }
    }
}
