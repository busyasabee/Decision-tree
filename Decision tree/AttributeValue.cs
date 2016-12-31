using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decision_tree
{
    public class AttributeValue : IEquatable<AttributeValue>
    {
        private string value;
        private int frequency;
        private int labelsCount;
        private List<int> lineNumbers;
        private int[] leafLabelsCount;
        public double[] pLabels;
        //private List<int> leafLabelsCount;

        public AttributeValue(string value, int labelsCount)
        {
            this.value = value;
            lineNumbers = new List<int>();
            this.labelsCount = labelsCount;
            leafLabelsCount = new int[this.labelsCount];
            pLabels = new double[labelsCount];
        }

        public string getPlainValue()
        {
            return value;
        }

        public void increaseFrequency(int value)
        {
            frequency += value;
        }



        public void addLineNumber(int number)
        {
            lineNumbers.Add(number);
        }


        public void increaseLeafLabelCount(int index, int value)
        {
            leafLabelsCount[index] += value;
        }

        public int countLineNumbers()
        {
            return lineNumbers.Count;
        }

        public int getLineNumber(int index)
        {
            return lineNumbers[index];
        }

        public int getCountLeafLabel(int index)
        {
            return leafLabelsCount[index];
        }

        public int getLeafsCountSum()
        {
            return leafLabelsCount.Sum();
        }

        public bool Equals(AttributeValue other)
        {
            if (this.value == other.getPlainValue())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            AttributeValue objAsAttributeValue = obj as AttributeValue;
            if (objAsAttributeValue == null) return false;
            else return Equals(objAsAttributeValue);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public void clear()
        {
            frequency = 0;
            lineNumbers = new List<int>();
            leafLabelsCount = new int[labelsCount];
            pLabels = new double[labelsCount];
        }

    }
}
