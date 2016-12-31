using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decision_tree
{

    public class Attribute
    {
        string name;
        List<AttributeValue> values;
        int countValues;
        bool isUsed;
        int index;

        public Attribute(string name)
        {
            this.name = name;
            values = new List<AttributeValue>();
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

        public void addValue(AttributeValue value)
        {
            values.Add(value);
            countValues += 1;
        }

        public int getCountValues()
        {
            return countValues;
        }

        public AttributeValue getValue(int index)
        {
            return values[index];
        }

        public bool containValue(AttributeValue value)
        {
            return values.Contains(value);
        }
    }

}
