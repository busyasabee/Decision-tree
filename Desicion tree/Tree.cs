using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desicion_tree
{
    public class Attribute
    {
        string name;
        //HashSet<string> values; // with unique elements, but not order
        List<string> values;
        int countValues;
        double entropy;
        bool isUsed;
        int index;

        public Attribute(string name)
        {
            this.name = name;
            values = new List<string>();
            countValues = 0;
            entropy = 1d;
            isUsed = false;
        }

        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public void addValue(string value)
        {
            values.Add(value);
            countValues += 1;
        }

        public int getCountValues()
        {
            return countValues;
        }

        public string getValue(int index)
        {
            return values[index];
        }

        public bool containValue(string value)
        {
            return values.Contains(value);
        }
    }

    class Tree
    {
        List<int> testList = new List<int>();
        List<Attribute> attributes = new List<Attribute>();
        string indent = "";
        int leafsLabelsCount;
        //double nodeEntropy;
        double entropyChild;
        // column labels of lists 
        int lastColNumber;
        // To store the table from a file
        List<List<string>> table = new List<List<string>>();
        int colsCount;
        int lineCount;
        List<string> tableHeader = new List<string>();
        // To align the table rows in the console
        int maxWordLength;
        int wordWidth;
        string outputPath;
        Dictionary<string, List<string>> attributeValuesDict = new Dictionary<string, List<string>>();
        List<string> leafsLabels = new List<string>();
        // The number of distinct values in the column
        List<int> attributeValuesCount = new List<int>();

        public int LeafLabelsCount
        {
            get { return leafsLabelsCount; }
            set { leafsLabelsCount = value; }
        } 

        public int tableLinesCount
        {
            get { return lineCount; }
        }
        public string outputFilePath
        {
            set { outputPath = value; }

        }

        // Read the lines from a file into the table
        public void readTableFromFile(string path, out bool[] usedAttributes)
        {
            string line;
            Encoding enc = Encoding.GetEncoding(1251);
            StreamReader file = new StreamReader(path, enc);
            string[] splitStr;

            while ((line = file.ReadLine()) != null)
            {

                splitStr = line.Split(' ');

                foreach (var word in splitStr)
                {
                    if (word.Length >= maxWordLength)
                    {
                        maxWordLength = word.Length;
                    }
                }

                table.Add(splitStr.ToList<string>());

            }

            colsCount = table[0].Count;
            wordWidth = maxWordLength + 5;
            usedAttributes = new bool[colsCount];
            tableHeader = table[0].ToList<string>();
            //tableHeader.RemoveAt(0);
            table.RemoveAt(0);
            lineCount = table.Count;
            lastColNumber = colsCount - 1;

            for (int i = 1; i < colsCount - 1; i++)
            {
    
                Attribute attribute = new Attribute(tableHeader[i]);
                attribute.Index = i;
                // string attribute = tableHeader[i];
                // attributeValuesDict.Add(attribute, new List<string>());

                for (int j = 0; j < lineCount; j++)
                {
                    string value = table[j][i];
                    if (attribute.containValue(value) == false)
                    {
                        attribute.addValue(value);
                    }

                    // attributeValuesDict[attribute].Add(value);

                }

                attributes.Add(attribute);
                //attributeValuesDict[attribute] = attributeValuesDict[attribute].Distinct<string>().ToList<string>();
                //// Можно сделать класс атрибута, там будет имя, его значения, число различных значений
                //attributeValuesCount.Add(attributeValuesDict[attribute].Count);
                
            }

            for (int i = 0; i < lineCount; i++)
            {
                string value = table[i][lastColNumber];

                if (leafsLabels.Contains(value) != true)
                {
                    leafsLabels.Add(value);
                }


                if (leafsLabels.Count == leafsLabelsCount)
                {
                    break;
                }

            }

            leafsLabels = leafsLabels.Distinct<string>().ToList<string>();
            file.Close();

        }

        public void showTable()
        {
            int consoleWidth = colsCount * wordWidth;
            Console.SetWindowSize(consoleWidth, Console.WindowHeight);
            for (int i = 0; i < tableHeader.Count; i++)
            {
                Console.Write("{0, -" + wordWidth + "}", tableHeader[i]);
            }
            Console.Write('\n');

            for (int i = 0; i < table.Count; i++)
            {
                for (int j = 0; j < table[i].Count; j++)
                {
                    Console.Write("{0, -" + wordWidth + "}", table[i][j]);
                }
                Console.Write('\n');
            }
        }

        // Add to the indent n whitespaces
        void increaseIndent(ref string indent, int n)
        {
            for (int i = 0; i < n; i++)
            {
                indent += " ";
            }
            int t = 0;
        }

        // lineNumbers - certain lines from table
        public void run(List<int> lineNumbers, bool[] usedAttributes, string branchLabel = "")
        {
            int whitespacesCount;
            //int countWinYes = 0;
            //int countWinNo = 0;
            int lineCount = lineNumbers.Count;

            /*// Devide the lines by the value of the attribute 
            List<int> firstValueLineNumbers = new List<int>();
            List<int> secondValueLineNumbers = new List<int>();*/
            Dictionary<string, List<int>> valueLineNumbersDict = new Dictionary<string, List<int>>();
            // key - attribute value, key - list with the leafs probability values 
            Dictionary<string, List<int>> valueLeafLabelsCountDict = new Dictionary<string, List<int>>();
            double nodeEntropy = 0;

            if (branchLabel.Length != 0)
            {
                Console.Write(indent + branchLabel + " ");
            }

            else
            {
                Console.Write(indent);
            }

            if (lineNumbers.Count == 0)
            {
                Console.WriteLine("Not lines");
                return;
            }

            for (int i = 0; i < lineNumbers.Count; i++)
            {
                Console.Write((lineNumbers[i] + 1) + " ");
            }

            int[] labelCount = new int[leafsLabelsCount];

            for (int i = 0; i < lineNumbers.Count; i++)
            {
                for (int j = 0; j < leafsLabelsCount; j++)
                {
                    if (table[lineNumbers[i]][lastColNumber] == leafsLabels[j])
                    {
                        labelCount[j] += 1;
                    }
                }
                /*if (tableList[lineNumbers[i]][lastColNumber] == "нет")
                {
                    ++countWinNo;
                }
                else ++countWinYes;*/
            }

            double[] pLabel = new double[leafsLabelsCount];

            for (int i = 0; i < leafsLabelsCount; i++)
            {
                pLabel[i] = (double)labelCount[i] / lineCount;
                pLabel[i] = (pLabel[i] == 0) ? 1 : pLabel[i];
             }

            for (int i = 0; i < leafsLabelsCount; i++)
            {
                nodeEntropy += -pLabel[i] * Math.Log(pLabel[i], 2);
            }

            /*double pNo = (double)countWinNo / lineCount;
            double pYes = (double)countWinYes / lineCount;
            pNo = (pNo == 0) ? 1 : pNo;
            pYes = (pYes == 0) ? 1 : pYes;
            nodeEntropy = -pNo * Math.Log(pNo, 2) - pYes * Math.Log(pYes, 2);*/

            // Exit from recursion

            if (nodeEntropy == 0)
            {
                int consoleWidth = indent.Length;

                //if (Console.WindowWidth <= )
                //{

                //}
                Console.Write(tableHeader[lastColNumber] + " " + table[lineNumbers[0]][lastColNumber]);
                Console.WriteLine();
                Console.WriteLine();
                return;
            }
            // key - index of attribute, value - entropy
            //Dictionary<int, double> childEntropyDict = new Dictionary<int, double>();
            Dictionary<Attribute, double> childEntropyDict = new Dictionary<Attribute, double>();
            Attribute minEntropyAttribute = new Attribute("");


            // Iterate on attributes


            //for (int i = 1; i < colsCount - 1; i++)
            foreach (var attribute in attributes)
            {
                // Number of column
                int attributeIndex = attribute.Index;
                int attributeValuesCount = attribute.getCountValues();
                //if (usedAttributes[i] != true)
                if (attribute.IsUsed != true)
                {
                    // Может посчитать число вариантов значений для атрибутов при чтении файла, 

                    // int[] valuesFrequency = new int[attributeValuesCount[i-1]];
                    int[] valuesFrequency = new int[attribute.getCountValues()];

                    // To store the count of the class labels for each attribute value 

                    List<double> pValues = new List<double>();
                    // key - attribute name, value - p labels
                    Dictionary<string, List<double>> pLabelsDict = new Dictionary<string, List<double>>();

                    for (int i = 0; i < attributeValuesCount; i++)
                    {
                        string attributeValue = attribute.getValue(i);
                        valueLineNumbersDict.Add(attributeValue, new List<int>());

                        // итак всего 2 элемента
                        //if (valueLineNumbersDict.ContainsKey(attributeValue) != true)
                        //{
                        //    valueLineNumbersDict.Add(attributeValue, new List<int>());
                        //}

                        //if (valueLineNumbersDict.Keys.Count == attributeValuesCount)
                        //{
                        //    break;
                        //}

                    }



                    for (int j = 0; j < lineNumbers.Count; j++)
                    {
                        //for (int k = 0; k < attributeValuesCount[i-1]; k++)
                        for (int k = 0; k < attributeValuesCount; k++)
                        {
                            string attributeValue = attribute.getValue(k);
                            //valueLineNumbersDict.Add(attributeValuesDict[i][k], new List<int>()); 
                            //if (table[lineNumbers[j]][i] == attributeValuesDict[i][k])
                            if (table[lineNumbers[j]][attributeIndex] == attributeValue)
                            {
                                valuesFrequency[k] += 1;
                                //valueLineNumbersDict[attributeValuesDict[i][k]].Add(lineNumbers[j]);
                                valueLineNumbersDict[attributeValue].Add(lineNumbers[j]);
                            }

                        }
                        
                    }


                    //double firstValueCount, secondValueCount, firstValueYesCount, firstValueNoCount, secondValueYesCount, secondValueNoCount;
                    //firstValueCount = secondValueCount = firstValueYesCount = firstValueNoCount = secondValueYesCount = secondValueNoCount = 0d;

                    //for (int j = 0; j < lineNumbers.Count; j++)
                    //{
                    //    if (tableList[lineNumbers[j]][i] == attributeValuesDict[i][0])
                    //    {
                    //        ++firstValueCount;
                    //        firstValueLineNumbers.Add(lineNumbers[j]);
                    //    }
                    //    else
                    //    {
                    //        ++secondValueCount;
                    //        secondValueLineNumbers.Add(lineNumbers[j]);
                    //    }
                    //}

                    //for (int k = 0; k < attributeValuesCount[i - 1]; k++)
                    for (int k = 0; k < attributeValuesCount; k++)
                    {
                        string attributeValue = attribute.getValue(k);
                        //valueLeafLabelsCountDict.Add(attributeValuesDict[i][k], new List<int>());
                        valueLeafLabelsCountDict.Add(attributeValue, new List<int>());
                        valueLeafLabelsCountDict[attributeValue].Add(0);
                        valueLeafLabelsCountDict[attributeValue].Add(0);
                        //pLabelsDict.Add(attributeValuesDict[i][k], new List<double>());
                        pLabelsDict.Add(attributeValue, new List<double>());
                        double pValue = (double) valuesFrequency[k] / lineCount;
                        pValues.Add(pValue);
                    }

                    /*double p1 = firstValueCount / lineCount;
                    double p2 = secondValueCount / lineCount;*/
                    foreach (var key in valueLineNumbersDict.Keys)
                    {
                        for (int j = 0; j < valueLineNumbersDict[key].Count; j++)
                        {
                            for (int k = 0; k < leafsLabelsCount; k++)
                            {
                                if (table[valueLineNumbersDict[key][j]][lastColNumber] == leafsLabels[k])
                                {
                                    // добавил добавление нулей при создании списка
                                    //if (valueLeafLabelsCountDict[key].Count <= 2)
                                    //{
                                    //    valueLeafLabelsCountDict[key].Add(0);
                                    //}

                                    valueLeafLabelsCountDict[key][k] += 1; 
                                }
                            }
                            
                        }

                        for (int j = 0; j < leafsLabelsCount; j++)
                        {
                            double p = (double) valueLeafLabelsCountDict[key][j] / valueLeafLabelsCountDict[key].Sum();
                            p = (p == 0) ? 1 : p;
                            pLabelsDict[key].Add(p);
                        }
                    }

                    /*double p1Yes, p1No, p2Yes, p2No;
                    for (int j = 0; j < firstValueLineNumbers.Count; j++)
                    {
                        if (tableList[firstValueLineNumbers[j]][lastColNumber] == "да")
                        {
                            firstValueYesCount += 1;
                        }
                        else firstValueNoCount += 1;
                    }
                    p1Yes = firstValueYesCount / firstValueLineNumbers.Count;
                    // чтобы не брать логорифм от 0
                    if (p1Yes == 0)
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
                        if (tableList[secondValueLineNumbers[j]][lastColNumber] == "да")
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
                    }*/

                    for (int j = 0; j < pValues.Count; j++)
                    {
                        string attributeValue = attribute.getValue(j);
                        double pTmp = 0;

                        //for (int k = 0; k < pLabelsDict[j].Count; k++)
                        for (int k = 0; k < pLabelsDict[attributeValue].Count; k++)
                        {
                            pTmp += -pLabelsDict[attributeValue][k] * Math.Log(pLabelsDict[attributeValue][k], 2);
                        }

                        entropyChild += pValues[j] * pTmp;

                    }
                    //entropyChild = p1 * (-p1Yes * Math.Log(p1Yes, 2) - p1No * Math.Log(p1No, 2)) + p2 * (-p2Yes * Math.Log(p2Yes, 2) - p2No * Math.Log(p2No, 2));

                    // решил в начале это обработать и выйти
                    //if (firstValueLineNumbers.Count != 0 && secondValueLineNumbers.Count != 0)
                    //{
                        if (Double.IsNaN(entropyChild) == true)
                        {
                            entropyChild = 0;
                            //childEntropyDict.Add(i, entropyChild);
                            childEntropyDict.Add(attribute, entropyChild);
                        }
                        else //childEntropyDict.Add(i, entropyChild);
                            childEntropyDict.Add(attribute, entropyChild);
                    //}



                }

                valueLineNumbersDict.Clear();
                //firstValueLineNumbers.Clear();
                //secondValueLineNumbers.Clear();

            }

            double minEntropy = childEntropyDict.Values.Min();
            //var myKey = types.FirstOrDefault(x => x.Value == "one").Key;
            Attribute attributeForSplit = childEntropyDict.FirstOrDefault(x => x.Value == minEntropy).Key; // можно в словаре поменять ключ и значение местами
            int indexOfAttribute = attributeForSplit.Index;
            //usedAttributes[indexOfAttribute] = true; // будет ли видно из рекурсии это?
                                                     // to do вывести в консоль по какому атрибуту делим и какие номера строк имеем после разделения
                                                     // to do искать номера строк к одним и вторым значением в значениях выбранного атрибута

            attributes[indexOfAttribute - 1].IsUsed = true;
            testList.Add(1);
            List<List<int>> linesAfterSplit = new List<List<int>>();

            for (int i = 0; i < lineNumbers.Count; i++)
            {
                for (int j = 0; j < attributeForSplit.getCountValues(); j++)
                {
                    linesAfterSplit.Add(new List<int>());
                    if (table[lineNumbers[i]][indexOfAttribute] == attributeForSplit.getValue(j))
                    {
                        linesAfterSplit[j].Add(lineNumbers[i]);
                    }
                }
                //if (table[lineNumbers[i]][indexOfAttribute] == attributeValuesDict[indexOfAttribute][0])
                //{
                //    firstValueLineNumbers.Add(lineNumbers[i]);
                //}
                //else secondValueLineNumbers.Add(lineNumbers[i]);

            }

            if (branchLabel.Length == 0)
            {
                whitespacesCount = 2 * (lineNumbers.Count) + tableHeader[indexOfAttribute].Length + 1;
            }
            else
            {
                whitespacesCount = 2 * (lineNumbers.Count) + tableHeader[indexOfAttribute].Length + 1 + (branchLabel.Length + 1);
            }

            Console.WriteLine(tableHeader[indexOfAttribute]);
            Console.WriteLine();
            increaseIndent(ref indent, whitespacesCount);

            if (Console.WindowWidth - 20 <= indent.Length)
            {
                Console.WindowWidth += 20;
            }
            for (int i = 0; i < attributeForSplit.getCountValues(); i++)
            {
                run(linesAfterSplit[i], usedAttributes, attributeForSplit.getValue(i));
            }
            //run(firstValueLineNumbers, usedAttributes, attributeValuesDict[indexOfAttribute][0]);
            ////indent = indent.Remove(0, whitespacesCount);
            //run(secondValueLineNumbers, usedAttributes, attributeValuesDict[indexOfAttribute][1]);

            //usedAttributes[indexOfAttribute] = false;
            attributes[indexOfAttribute - 1].IsUsed = false;
            indent = indent.Remove(0, whitespacesCount);
            int d = 0;

        }

    }
}
