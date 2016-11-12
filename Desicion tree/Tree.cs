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
        Dictionary<int, Tuple<List<int>, List<int>>> attributeLineNumbers; // храним номер столбца и к нему строки в которых встречались два значения
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
        //bool[] usedAttributes;
        int colsCount;
        int lineCount;
        string[] tableHeader;
        bool firstTime = true;
        //string[,] table = new string[9, 5] {"Соперник", "Играем","Лидеры" };

        public void readFile(string path, out bool[] usedAttributes)
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
            usedAttributes = new bool[colsCount];
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
        public void run(List<int> lineNumbers, bool[] usedAttributes) // работаем только с определёнными строками
        {
            double entropyT;
            double entropyChild;
            int countWinYes = 0;
            int countWinNo = 0;
            int lineCount = lineNumbers.Count;
            attributeLineNumbers = new Dictionary<int, Tuple<List<int>, List<int>>>(); 
            List<int> firstValueLineNumbers = new List<int>();
            List<int> secondValueLineNumbers = new List<int>();

            int colNumber = Array.IndexOf(tableHeader, "Победа");
            for (int i = 0; i < lineNumbers.Count; i++)
            {
                if (tableList[lineNumbers[i]][colNumber] == "нет")
                {
                    ++countWinNo;
                }
                else ++countWinYes;
            }
            double pNo = (double)countWinNo / lineCount;
            double pYes = (double)countWinYes / lineCount;
            if (pNo == 0)
            {
                pNo = 1;
            }
            if (pYes==0)
            {
                pYes = 1;
            }
            entropyT = -pNo * Math.Log(pNo, 2) - pYes * Math.Log(pYes, 2);
            Console.Write("Энтропия вершины ");
            for (int i = 0; i < lineNumbers.Count; i++)
            {
                Console.Write(lineNumbers[i] + " ");
            }
            Console.WriteLine("равна " + entropyT);
            if (entropyT == 0) return; //выходим
            //List<double> childEntropyList = new List<double>(); //надо ещё запоминать номера столбцов
            Dictionary<int, double> childEntropyDict = new Dictionary<int, double>();
            // сделать перебор атрибутов
            for (int i = 1; i < colsCount-1; i++)
            {
                if (usedAttributes[i] != true)
                {
                    double firstValueCount, secondValueCount;
                    firstValueCount = secondValueCount = 0d;
                    double firstValueYesCount, firstValueNoCount;
                    firstValueYesCount = firstValueNoCount = 0d;
                    double secondValueYesCount, secondValueNoCount;
                    secondValueYesCount = secondValueNoCount = 0d;
                    //List<int> firstValueLineNumbers = new List<int>();
                    //List<int> secondValueLineNumbers = new List<int>();
                    for (int j = 0; j < lineNumbers.Count; j++)
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
                    //attributeLineNumbers.Add(i, new Tuple<List<int>, List<int>>(firstValueLineNumbers, secondValueLineNumbers));
                    double p1 = firstValueCount / lineCount;
                    double p2 = secondValueCount / lineCount;
                    double p1Yes, p1No, p2Yes, p2No;
                    for (int j = 0; j < firstValueLineNumbers.Count; j++)
                    {
                        if (tableList[firstValueLineNumbers[j]][colNumber] == "да")
                        {
                            firstValueYesCount += 1;
                        }
                        else firstValueNoCount += 1;
                    }
                    p1Yes = firstValueYesCount / firstValueLineNumbers.Count;
                    // чтобы не брать логорифм от 0
                    if (p1Yes==0)
                    {
                        p1Yes = 1;
                    }
                    p1No = firstValueNoCount / firstValueLineNumbers.Count;
                    if (p1No == 0)
                    {
                        p1No = 1;
                    }

                    for (int j = 0; j < secondValueLineNumbers.Count; j++)
                    {
                        if (tableList[secondValueLineNumbers[j]][colNumber] == "да")
                        {
                            secondValueYesCount += 1;
                        }
                        else secondValueNoCount += 1;
                    }
                    p2Yes = secondValueYesCount / secondValueLineNumbers.Count;
                    if (p2Yes == 0)
                    {
                        p2Yes = 1;
                    }
                    p2No = secondValueNoCount / secondValueLineNumbers.Count;
                    if (p2No == 0)
                    {
                        p2No = 1;
                    }
                    entropyChild = p1 * (-p1Yes * Math.Log(p1Yes, 2) - p1No * Math.Log(p1No, 2)) + p2 * (-p2Yes * Math.Log(p2Yes, 2) - p2No * Math.Log(p2No, 2));
                    if(firstValueLineNumbers.Count!=0 && secondValueLineNumbers.Count != 0)
                    {
                        if (Double.IsNaN(entropyChild) == true)
                        {
                            entropyChild = 0;
                            childEntropyDict.Add(i, entropyChild);
                        }
                        else childEntropyDict.Add(i, entropyChild);
                    }
                    
                    //childEntropyList.Add(entropyChild);

                }
                firstValueLineNumbers.Clear();
                secondValueLineNumbers.Clear();
                // вроде я не здесь пишу
                
            }
            double minEntropy = childEntropyDict.Values.Min();
            //var myKey = types.FirstOrDefault(x => x.Value == "one").Key;
            int indexOfAttribute = childEntropyDict.FirstOrDefault(x => x.Value == minEntropy).Key; // можно в словаре поменять ключ и значение местами
            usedAttributes[indexOfAttribute] = true; // будет ли видно из рекурсии это?
                                                     // to do вывести в консоль по какому атрибуту делим и какие номера строк имеем после разделения
                                                     // to do искать номера строк к одним и вторым значением в значениях выбранного атрибута
            Console.WriteLine("Делим по атрибуту " + tableHeader[indexOfAttribute]);
            /*List<int> firstValueNumbers = new List<int>();//можно объявить в начале  
            List<int> secondValueNumbers = new List<int>();
            firstValueNumbers.*/
            for (int i = 0; i < lineNumbers.Count; i++)
            {
                if (tableList[lineNumbers[i]][indexOfAttribute] == attributeValuesDict[indexOfAttribute][0])
                {
                    firstValueLineNumbers.Add(lineNumbers[i]);
                }
                else secondValueLineNumbers.Add(lineNumbers[i]);
              
            }
            
            Console.Write("Строки разделились на ");
            for (int i = 0; i < firstValueLineNumbers.Count; i++)
            {
                Console.Write(firstValueLineNumbers[i].ToString() + " ");
            }
            Console.Write(" и ");
            for (int i = 0; i < secondValueLineNumbers.Count; i++)
            {
                Console.Write(secondValueLineNumbers[i].ToString() + " ");
            }
            Console.WriteLine();
            //bool[] copyUsedAttributes = (bool[])usedAttributes.Clone();
            /*bool[] copyUsedAttributes = new bool[usedAttributes.Length];
            for (int i = 0; i < usedAttributes.Length; i++)
            {
                copyUsedAttributes[i] = usedAttributes[i];
            }*/
            run(firstValueLineNumbers, usedAttributes/*copyUsedAttributes*/);
            run(secondValueLineNumbers, usedAttributes/*copyUsedAttributes*/);
            usedAttributes[indexOfAttribute] = false;
            int d = 0;

        }

    }
}
