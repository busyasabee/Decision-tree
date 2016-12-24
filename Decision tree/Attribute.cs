using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desicion_tree
{

    public class Attribute
    {
        string name;
        List<string> values;
        int countValues;
        bool isUsed;
        int index;

        public Attribute(string name)
        {
            this.name = name;
            values = new List<string>();
            countValues = 0;
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

}
