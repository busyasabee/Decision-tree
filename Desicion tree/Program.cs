using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Desicion_tree
{
    class Program
    {
   
        static void Main(string[] args)
        {
         
            Tree tree = new Tree();
            bool[] usedAttributes;
            var inputFilePath = @"E:\Projects\Visual Studio\Console\Desicion tree\table.txt";
            tree.readTableFromFile(inputFilePath, out usedAttributes);
            var outputFilePath = @"E:\Projects\Visual Studio\Console\Desicion tree\output.txt";
            tree.outputFilePath = outputFilePath;
            try
            {
                using (StreamWriter sw = new StreamWriter(outputFilePath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine();
                    sw.WriteLine(" ----- " + "New program launch" + " ----- ");
                    sw.WriteLine();
                }

            }
            catch (Exception)
            {

                throw;
            }
            tree.showTable();
            int lineCount = tree.tableLinesCount;
            List<int> lineRange = new List<int>();
            for (int i = 0; i < lineCount; i++)
            {
                lineRange.Add(i);
            }
            
            tree.run(lineRange, usedAttributes/*, true*/);
            Console.ReadKey();
            int d = 0;
        }
    }
}
