﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decision_tree
{
    class Tree
    {

        int tableWidth;
        int consoleWidth;
        List<Attribute> attributes = new List<Attribute>();
        string indent = "";
        int leafsLabelsCount;
        int lastColNumber;
        List<List<string>> table = new List<List<string>>(); // To store the table from a file
        int colsCount;
        int lineCount;
        List<string> tableHeader = new List<string>();
        // To align the table rows in the console
        int maxWordLength;
        int wordWidth;
        List<string> leafsLabels = new List<string>();

        public int LeafLabelsCount
        {
            get { return leafsLabelsCount; }
            set { leafsLabelsCount = value; }
        }

        public int tableLinesCount
        {
            get { return lineCount; }
        }

        // Read the lines from a file into the table
        public void readTableFromFile(string path)
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
                    AttributeValue value = new AttributeValue(table[j][i], leafsLabelsCount);

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

        public Attribute findAttributeWithMinimumEntropy()
        {
            double minEntropy = 1d;
            int attributeIndex = Int32.MaxValue;

            foreach (var attribute in attributes)
            {
                if (attribute.Entropy < minEntropy && attribute.IsUsed == false)
                {
                    minEntropy = attribute.Entropy;
                    attributeIndex = attribute.Index;
                }
            }

            return attributes[attributeIndex - 1];
        }

        // lineNumbers - certain lines from table
        public void run(List<int> lineNumbers, string branchLabel = "")
        {
            string increment;
            int whitespacesCount;
            int branchLabelLength = branchLabel.Length;
            int lineCount = lineNumbers.Count;
            consoleWidth = indent.Length;
            double nodeEntropy = 0;

            if (branchLabelLength != 0)
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
                Console.WriteLine(increment);
                Console.WriteLine();
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

            foreach (var attribute in attributes)
            {
                // Number of column

                int attributeIndex = attribute.Index;
                int attributeValuesCount = attribute.getCountValues();

                if (attribute.IsUsed != true)
                {
                    for (int j = 0; j < lineNumbers.Count; j++)
                    {
                        for (int k = 0; k < attributeValuesCount; k++)
                        {
                            AttributeValue attributeValue = attribute.getValue(k);
                            string innerValue = attributeValue.getPlainValue();

                            if (table[lineNumbers[j]][attributeIndex] == innerValue)
                            {
                                attributeValue.increaseFrequency(1);
                                attributeValue.addLineNumber(lineNumbers[j]);
                            }
                        }
                    }

                    for (int k = 0; k < attributeValuesCount; k++)
                    {
                        AttributeValue attributeValue = attribute.getValue(k);
                        attributeValue.PValue = (double)attributeValue.getFrequency() / lineCount;

                    }

                    for (int i = 0; i < attribute.getCountValues(); i++)
                    {
                        AttributeValue attributeValue = attribute.getValue(i);

                        for (int j = 0; j < attributeValue.countLineNumbers(); j++)
                        {
                            for (int k = 0; k < leafsLabelsCount; k++)
                            {
                                if (table[attributeValue.getLineNumber(j)][lastColNumber] == leafsLabels[k])
                                {

                                    attributeValue.increaseLeafLabelCount(k, 1);
                                }
                            }

                        }

                        for (int j = 0; j < leafsLabelsCount; j++)
                        {
                            double p = (double)attributeValue.getCountLeafLabel(j) / attributeValue.getLeafsCountSum();
                            p = (p == 0) ? 1 : p;
                            attributeValue.PLabels[j] = p;

                        }

                    }

                    double entropyChild = 0d; // entropy, if divide by the attribute

                    for (int j = 0; j < attributeValuesCount; j++)
                    {
                        AttributeValue attributeValue = attribute.getValue(j);
                        double pTmp = 0;

                        for (int k = 0; k < leafsLabelsCount; k++)
                        {
                            pTmp += -attributeValue.PLabels[k] * Math.Log(attributeValue.PLabels[k], 2);
                        }

                        entropyChild += attributeValue.PValue * pTmp; 


                    }

                    if (Double.IsNaN(entropyChild) == true) entropyChild = 0;

                    attribute.Entropy = entropyChild;

                }

            }

            Attribute attributeForSplit = findAttributeWithMinimumEntropy();
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

                    if (table[lineNumbers[i]][indexOfAttribute] == attributeForSplit.getValue(j).getPlainValue())
                    {
                        linesAfterSplit[j].Add(lineNumbers[i]);
                    }
                }
            }

            if (branchLabelLength == 0)
            {
                whitespacesCount = 2 * (lineNumbers.Count) + tableHeader[indexOfAttribute].Length + 1;
            }
            else
            {
                whitespacesCount = 2 * (lineNumbers.Count) + tableHeader[indexOfAttribute].Length + 1 + (branchLabelLength + 1);
            }

            increment = tableHeader[indexOfAttribute];
            increaseConsoleWidth(increment);
            Console.WriteLine(increment);
            Console.WriteLine();
            increaseIndent(ref indent, whitespacesCount);

            foreach (var attribute in attributes)
            {
                for (int i = 0; i < attribute.getCountValues(); i++)
                {
                    attribute.getValue(i).clear();
                }
            }

            for (int i = 0; i < attributeForSplit.getCountValues(); i++)
            {
                run(linesAfterSplit[i], attributeForSplit.getValue(i).getPlainValue());
            }

            attributes[indexOfAttribute - 1].IsUsed = false;
            indent = indent.Remove(0, whitespacesCount);

        }
    }
}
