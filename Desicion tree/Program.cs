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
       
            Tree tree = new Tree();
            tree.readFile(@"E:\Projects\Visual Studio\Console\Desicion tree\table.txt");
            tree.headerOut();
            tree.showTable();
            int lineCount = tree.tableLinesCount;
            int[] lineRange = new int[lineCount];
            for (int i = 0; i < lineCount; i++)
            {
                lineRange[i] = i + 1;
            }
            tree.run(lineRange);
            int d = 0;
        }
    }
}
