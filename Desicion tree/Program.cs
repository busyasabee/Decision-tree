using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desicion_tree
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<int> hs = new HashSet<int>();
            hs.Add(1);
            hs.Add(1);


            Tree tree = new Tree();
            tree.readFile(@"E:\Projects\Visual Studio\Console\Desicion tree\table.txt");
            tree.headerOut();
            tree.showTable();
            int lineCount = tree.tableLinesCount;
            List<int> lineRange = new List<int>();
            for (int i = 0; i < lineCount; i++)
            {
                lineRange.Add(i);
            }
            tree.run(lineRange);
            int d = 0;
        }
    }
}
