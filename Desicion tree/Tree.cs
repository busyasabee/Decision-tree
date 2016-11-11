using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desicion_tree
{
    
    class Tree
    {
        List<string[]> tableList = new List<string[]>();
        public int tableLinesCount
        {
            get { return lineCount; }
        }
        bool[] usedAttributes;
        int colsCount;
        int lineCount;
        string[] tableHeader;
        bool firstTime = true;
        //string[,] table = new string[9, 5] {"Соперник", "Играем","Лидеры" };
        public void readFile(string path)
        {
            string line;
            Encoding enc = Encoding.GetEncoding(1251);
            StreamReader file = new StreamReader(path, enc);
            while ((line = file.ReadLine()) != null)
            {
                string[] str;
                str = line.Split(' ');
                colsCount = str.Length;
                tableList.Add(str);        

            }
            usedAttributes = new bool[colsCount - 2];
            tableHeader = tableList[0];
            tableList.RemoveAt(0);
            lineCount = tableList.Count;
            file.Close();
            /* System.Console.WriteLine(t.Item1);
             System.Console.WriteLine(" Have been read {0} lines ", counter);
             System.Console.ReadLine();*/

        }
        public void headerOut()
        {
            for (int i = 0; i < tableHeader.Length; i++)
            {
                Console.Write(tableHeader[i] + " ");
            }
            Console.Write('\n');
        }
        public void showTable()
        {
            for (int i = 0; i < tableList.Count; i++)
            {
                for (int j = 0; j < tableList[i].Length; j++)
                {
                    Console.Write(tableList[i][j] + " ");
                }
                Console.Write('\n');
            }
        }
        public void run(int[] lineNumbers)
        {
            double entropyT;
            int countWinYes = 0;
            int countWinNo = 0;
            if (firstTime == true)
            {
                int rowNumber = Array.IndexOf(tableHeader, "Победа");
                for (int i = 0; i < tableList.Count; i++)
                {
                    if (tableList[i][rowNumber] == "нет")
                    {
                        ++countWinNo;
                    }
                    else ++countWinYes;
                }
                double pNo = (double)countWinNo / lineCount;
                double pYes = (double)countWinYes / lineCount;
                entropyT = -pNo * Math.Log(pNo, 2) - pYes * Math.Log(pYes, 2);
                Console.WriteLine("Энтропия вершины" + entropyT);
                // сделать перебор атрибутов
                int d = 0;


            }
            else
            {

            }
        }
        
    }
}
