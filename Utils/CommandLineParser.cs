using System;
using System.Collections.Generic;

namespace Utils
{
    public class CommandLineParser
    {
        private const string ArgPrefix = "-";
        private readonly SortedList<string, string> _params = new SortedList<string, string>();
        public CommandLineParser(string[] args)
        {
            _params.Clear();
            List<string> strs = new List<string>();
            if (args != null && args.Length > 0)
            {
                foreach (string item in args)
                {
                    string[] tokens = item.Split(' ', ',', '\t');
                    strs.AddRange(tokens);
                }
            }

            string currentKey = "NULL_KEY";
            foreach (var item in strs)
            {
                if (item.StartsWith(ArgPrefix))
                {
                    currentKey = item.Substring(1);
                }
                else
                {
                    if (HasArgument(currentKey))
                    {
                        _params[currentKey] = _params[currentKey] + " " + item;
                    }
                    else
                    {
                        _params.Add(currentKey, item);
                    }
                }
            }
        }

        public bool IsEmpty
        {
            get { return _params.Count == 0; }
        }

        public bool HasArgument(string argumentName)
        {
            bool ret = _params.ContainsKey(argumentName);
            return ret;
        }

        public string GetArgumentString(string argumentName)
        {
            string ret = _params[argumentName];
            return ret;
        }

        public byte GetArgumentByte(string argumentName)
        {
            byte ret;
            if (byte.TryParse(_params[argumentName], out ret))
                return ret;
            throw new ArgumentException();
        }

        public int GetArgumentInt(string argumentName)
        {
            int ret;
            if (int.TryParse(_params[argumentName], out ret))
                return ret;
            throw new ArgumentException();
        }

        public double GetArgumentDouble(string argumentName)
        {
            double ret;
            if (double.TryParse(_params[argumentName], out ret))
                return ret;
            throw new ArgumentException();
        }

        public long GetArgumentLong(string argumentName)
        {
            long ret;
            if (long.TryParse(_params[argumentName], out ret))
                return ret;
            throw new ArgumentException();
        }
    }
}
