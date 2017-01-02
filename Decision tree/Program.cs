using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Decision_tree
{
    class Program
    {
   
        static void Main(string[] args)
        {

            int leafLabelsCount = 2;
            Tree tree = new Tree();
            tree.LeafLabelsCount = leafLabelsCount;
            var inputFilePath = @"E:\Projects\Visual Studio\Console\Decision tree\table.txt";
            tree.readTableFromFile(inputFilePath);
            tree.showTable();
            int lineCount = tree.tableLinesCount;
            List<int> lineRange = new List<int>();

            for (int i = 0; i < lineCount; i++)
            {
                lineRange.Add(i);
            }
            
            tree.run(lineRange);
            Console.ReadKey();

        }
    }
}
