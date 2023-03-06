
using System.Collections.Generic;

namespace KissServerFramework
{

    /// <summary>
    /// Define a class for the CSV table.
    /// </summary>
    public class TestCsv
    {
        public int id;
        public int number;
        public string name;
        public List<int> testInts;
        public List<string> testStrings;
        public List<float> testFloats;
        public Dictionary<string, int> testStringIntDicts;
        public Dictionary<int, bool> testIntBooleanDicts;
    }
}
