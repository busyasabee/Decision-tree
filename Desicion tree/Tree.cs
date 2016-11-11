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
        Dictionary<int, string[]> attributeValuesDict = new Dictionary<int, string[]>()
        {
            {1,  new string []{"выше", "ниже" } },
            {2,  new string []{"дома", "гости" } },
            {3,  new string []{"есть", "пропускают" } },
            {4,  new string []{"да", "нет" } },

        };
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
        public void run(int[] lineNumbers) // работаем только с определёнными строками
        {
            double entropyT;
            double entropyChild;
            int countWinYes = 0;
            int countWinNo = 0;
            int lineCount = lineNumbers.Length;

            int colNumber = Array.IndexOf(tableHeader, "Победа");
            for (int i = 0; i < lineNumbers.Length; i++)
            {
                if (tableList[lineNumbers[i]][colNumber] == "нет")
                {
                    ++countWinNo;
                }
                else ++countWinYes;
            }
            double pNo = (double)countWinNo / lineCount;
            double pYes = (double)countWinYes / lineCount;
            entropyT = -pNo * Math.Log(pNo, 2) - pYes * Math.Log(pYes, 2);
            Console.WriteLine("Энтропия вершины " + entropyT);
            List<double> childEntropyList = new List<double>();
            // сделать перебор атрибутов
            for (int i = 1; i < colsCount-1; i++)
            {
                double firstValueCount, secondValueCount;
                firstValueCount = secondValueCount = 0d;
                double firstValueYesCount, firstValueNoCount;
                firstValueYesCount = firstValueNoCount = 0d;
                double secondValueYesCount, secondValueNoCount;
                secondValueYesCount = secondValueNoCount = 0d;
                List<int> firstValueLineNumbers = new List<int>();
                List<int> secondValueLineNumbers = new List<int>();
                for (int j = 0; j < lineNumbers.Length; j++)
                {
                    if (tableList[lineNumbers[j]][i] == attributeValuesDict[i][0])
                    {
                        ++firstValueCount;
                        firstValueLineNumbers.Add(lineNumbers[j]);
                    }
                    else
                    {
                        ++secondValueCount;
                        secondValueLineNumbers.Add(lineNumbers[j]);
                    }
                }
                double p1 = firstValueCount / lineCount;
                double p2 = secondValueCount / lineCount;
                double p1Yes, p1No, p2Yes, p2No;
                for (int j = 0; j < firstValueLineNumbers.Count; j++)
                {
                    if (tableList[firstValueLineNumbers[j]][colNumber] == "yes")
                    {
                        firstValueYesCount += 1;
                    }
                    else firstValueNoCount += 1; 
                }
                p1Yes = firstValueYesCount / firstValueLineNumbers.Count;
                p1No = firstValueNoCount / firstValueLineNumbers.Count;

                for (int j = 0; j < secondValueLineNumbers.Count; j++)
                {
                    if (tableList[secondValueLineNumbers[j]][colNumber] == "yes")
                    {
                        secondValueYesCount += 1;
                    }
                    else secondValueNoCount += 1;
                }
                p2Yes = secondValueYesCount / secondValueLineNumbers.Count;
                p2No = secondValueNoCount / secondValueLineNumbers.Count;
                entropyChild = p1 * (-p1Yes * Math.Log(p1Yes, 2) - p1No * Math.Log(p1No, 2)) + p2 * (-p2Yes * Math.Log(p2Yes, 2) - p2No * Math.Log(p2No, 2));

                childEntropyList.Add(entropyChild);

            }

            int indexOfAttribute = childEntropyList.IndexOf(childEntropyList.Max());
            usedAttributes[indexOfAttribute] = true;
            Array.er
            int d = 0;

        }

    }
}
