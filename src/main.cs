using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGrok.DelphiNodes;
using DGrok.Framework;
using DGrok.Visitors;

namespace crosspascal
{
    class Program
    {
        static void Main(string[] args)
        {
            RuleType _ruleType = RuleType.Goal;
            string text = "d:/code/leaf3_terra/trunk/utils";
            Parser parser = Parser.FromText(text, "input", CompilerDefines.CreateStandard(), new MemoryFileLoader());
            AstNode tree = parser.ParseRule(_ruleType);
            Console.WriteLine(tree.Inspect());
            Console.ReadLine();
        }
    }
}
