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
        
        int tableWidth = 0;
        int consoleWidth = 0;
        List<Attribute> attributes = new List<Attribute>();
        string indent = "";
        int leafsLabelsCount;
        double entropyChild;
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
            table.RemoveAt(0);
            lineCount = table.Count;
            lastColNumber = colsCount - 1;

            for (int i = 1; i < colsCount - 1; i++)
            {

                Attribute attribute = new Attribute(tableHeader[i]);
                attribute.Index = i;

                for (int j = 0; j < lineCount; j++)
                {
                    string value = table[j][i];
                    if (attribute.containValue(value) == false)
                    {
                        attribute.addValue(value);
                    }

                }

                attributes.Add(attribute);

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
            tableWidth = colsCount * wordWidth;
            Console.SetWindowSize(tableWidth, Console.WindowHeight);
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

        void increaseConsoleWidth(string newString)
        {
            consoleWidth += newString.Length;

            if (Console.WindowWidth < consoleWidth)
            {
                if (consoleWidth <= tableWidth)
                {
                    Console.WindowWidth = tableWidth;
                }
                else
                {
                    Console.WindowWidth = consoleWidth;
                }
            }


        }

        void increaseConsoleWidth(int value)
        {
            consoleWidth += value;

            if (Console.WindowWidth < consoleWidth)
            {
                if (consoleWidth <= tableWidth)
                {
                    Console.WindowWidth = tableWidth;
                }
                else
                {
                    Console.WindowWidth = consoleWidth;
                }
            }

        }

        // lineNumbers - certain lines from table
        public void run(List<int> lineNumbers, string branchLabel = "")
        {
            string increment;
            int whitespacesCount;
            int branchLabelLength = branchLabel.Length;
            int lineCount = lineNumbers.Count;
            consoleWidth = indent.Length;
            // key - one of the attribute values
            Dictionary<string, List<int>> valueLineNumbersDict = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> valueLeafLabelsCountDict = new Dictionary<string, List<int>>();
            double nodeEntropy = 0;

            if (branchLabel.Length != 0)
            {
                increment = branchLabel + " ";
                increaseConsoleWidth(increment);
                Console.Write(indent + branchLabel + " ");
            }

            else
            {

                Console.Write(indent);
            }

            if (lineNumbers.Count == 0)
            {
                increment = "Not lines";
                increaseConsoleWidth(increment);
                Console.WriteLine("Not lines");
                return;
            }

            int value = 2 * lineNumbers.Count;

            increaseConsoleWidth(value);


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

            // Exit from recursion

            if (nodeEntropy == 0)
            {
                increment = tableHeader[lastColNumber] + " " + table[lineNumbers[0]][lastColNumber];
                increaseConsoleWidth(increment);
                Console.Write(increment);
                Console.WriteLine();
                Console.WriteLine();
                return;
            }

            Dictionary<Attribute, double> childEntropyDict = new Dictionary<Attribute, double>();
            Attribute minEntropyAttribute = new Attribute("");

            foreach (var attribute in attributes)
            {
                // Number of column
                int attributeIndex = attribute.Index;
                int attributeValuesCount = attribute.getCountValues();

                if (attribute.IsUsed != true)
                {

                    int[] valuesFrequency = new int[attributeValuesCount];
                    List<double> pValues = new List<double>();
                    Dictionary<string, List<double>> pLabelsDict = new Dictionary<string, List<double>>();

                    for (int i = 0; i < attributeValuesCount; i++)
                    {
                        string attributeValue = attribute.getValue(i);
                        valueLineNumbersDict.Add(attributeValue, new List<int>());

                    }



                    for (int j = 0; j < lineNumbers.Count; j++)
                    {  
                        for (int k = 0; k < attributeValuesCount; k++)
                        {
                            string attributeValue = attribute.getValue(k);

                            if (table[lineNumbers[j]][attributeIndex] == attributeValue)
                            {
                                valuesFrequency[k] += 1;
                                valueLineNumbersDict[attributeValue].Add(lineNumbers[j]);
                            }

                        }

                    }

                    for (int k = 0; k < attributeValuesCount; k++)
                    {
                        string attributeValue = attribute.getValue(k);
                        valueLeafLabelsCountDict.Add(attributeValue, new List<int>());
                        valueLeafLabelsCountDict[attributeValue].Add(0);
                        valueLeafLabelsCountDict[attributeValue].Add(0);
                        pLabelsDict.Add(attributeValue, new List<double>());
                        double pValue = (double)valuesFrequency[k] / lineCount;
                        pValues.Add(pValue);
                    }

                    foreach (var key in valueLineNumbersDict.Keys)
                    {
                        for (int j = 0; j < valueLineNumbersDict[key].Count; j++)
                        {
                            for (int k = 0; k < leafsLabelsCount; k++)
                            {
                                if (table[valueLineNumbersDict[key][j]][lastColNumber] == leafsLabels[k])
                                {

                                    valueLeafLabelsCountDict[key][k] += 1;
                                }
                            }

                        }

                        for (int j = 0; j < leafsLabelsCount; j++)
                        {
                            double p = (double)valueLeafLabelsCountDict[key][j] / valueLeafLabelsCountDict[key].Sum();
                            p = (p == 0) ? 1 : p;
                            pLabelsDict[key].Add(p);
                        }
                    }

                    for (int j = 0; j < pValues.Count; j++)
                    {
                        string attributeValue = attribute.getValue(j);
                        double pTmp = 0;

                        for (int k = 0; k < pLabelsDict[attributeValue].Count; k++)
                        {
                            pTmp += -pLabelsDict[attributeValue][k] * Math.Log(pLabelsDict[attributeValue][k], 2);
                        }

                        entropyChild += pValues[j] * pTmp; // entropy, if divide by the attribute

                    }

                    if (Double.IsNaN(entropyChild) == true) entropyChild = 0;

                    childEntropyDict.Add(attribute, entropyChild);

                }

                valueLineNumbersDict.Clear();

            }

            double minEntropy = childEntropyDict.Values.Min();
            Attribute attributeForSplit = childEntropyDict.FirstOrDefault(x => x.Value == minEntropy).Key; 
            int indexOfAttribute = attributeForSplit.Index;
            attributes[indexOfAttribute - 1].IsUsed = true;
            List<List<int>> linesAfterSplit = new List<List<int>>();

            for (int j = 0; j < attributeForSplit.getCountValues(); j++)
            {
                linesAfterSplit.Add(new List<int>());

            }

            for (int i = 0; i < lineNumbers.Count; i++)
            {
                for (int j = 0; j < attributeForSplit.getCountValues(); j++)
                {

                    if (table[lineNumbers[i]][indexOfAttribute] == attributeForSplit.getValue(j))
                    {
                        linesAfterSplit[j].Add(lineNumbers[i]);
                    }
                }

            }

            if (branchLabel.Length == 0)
            {
                whitespacesCount = 2 * (lineNumbers.Count) + tableHeader[indexOfAttribute].Length + 1;
            }
            else
            {
                whitespacesCount = 2 * (lineNumbers.Count) + tableHeader[indexOfAttribute].Length + 1 + (branchLabel.Length + 1);
            }

            increment = tableHeader[indexOfAttribute];
            increaseConsoleWidth(increment);
            Console.WriteLine(increment);
            Console.WriteLine();
            increaseIndent(ref indent, whitespacesCount);

            for (int i = 0; i < attributeForSplit.getCountValues(); i++)
            {
                run(linesAfterSplit[i], attributeForSplit.getValue(i));
            }

            attributes[indexOfAttribute - 1].IsUsed = false;
            indent = indent.Remove(0, whitespacesCount);

        }

    }
}
