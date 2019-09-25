using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NotBinaryMath
{
    struct DecimalInt
    {
        private string number;
        public int Number { get => int.Parse(number); set => number = value.ToString(); }

        private static readonly string[,] sumTable =
        {
            {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"},
            {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"},
            {"2", "3", "4", "5", "6", "7", "8", "9", "10", "11"},
            {"3", "4", "5", "6", "7", "8", "9", "10", "11", "12"},
            {"4", "5", "6", "7", "8", "9", "10", "11", "12", "13"},
            {"5", "6", "7", "8", "9", "10", "11", "12", "13", "14"},
            {"6", "7", "8", "9", "10", "11", "12", "13", "14", "15"},
            {"7", "8", "9", "10", "11", "12", "13", "14", "15", "16"},
            {"8", "9", "10", "11", "12", "13", "14", "15", "16", "17"},
            {"9", "10", "11", "12", "13", "14", "15", "16", "17", "18"},
        };
        private static readonly string[,] difTable =
        {
            {"0", "-1", "-2", "-3", "-4", "-5", "-6", "-7", "-8", "-9"},
            {"1", "0", "-1", "-2", "-3", "-4", "-5", "-6", "-7", "-8"},
            {"2", "1", "0", "-1", "-2", "-3", "-4", "-5", "-6", "-7"},
            {"3", "2", "1", "0", "-1", "-2", "-3", "-4", "-5", "-6"},
            {"4", "3", "2", "1", "0", "-1", "-2", "-3", "-4", "-5"},
            {"5", "4", "3", "2", "1", "0", "-1", "-2", "-3", "-4"},
            {"6", "5", "4", "3", "2", "1", "0", "-1", "-2", "-3"},
            {"7", "6", "5", "4", "3", "2", "1", "0", "-1", "-2"},
            {"8", "7", "6", "5", "4", "3", "2", "1", "0", "-1"},
            {"9", "8", "7", "6", "5", "4", "3", "2", "1", "0"},
        };
        private static readonly string[,] mulTable =
        {
            {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
            {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"},
            {"0", "2", "4", "6", "8", "10", "12", "14", "16", "18"},
            {"0", "3", "6", "9", "12", "15", "18", "21", "24", "27"},
            {"0", "4", "8", "12", "16", "20", "24", "28", "32", "36"},
            {"0", "5", "10", "15", "20", "25", "30", "35", "40", "45"},
            {"0", "6", "12", "18", "24", "30", "36", "42", "48", "54"},
            {"0", "7", "14", "21", "28", "35", "42", "49", "56", "63"},
            {"0", "8", "16", "24", "32", "40", "48", "56", "64", "72"},
            {"0", "9", "18", "27", "36", "45", "54", "63", "72", "81"},
        };
        private static readonly int[] digitTable;

        static DecimalInt()
        {
            digitTable = new int[58];
            for (int i = 49, j = 1; i < 58; i++, j++)
            {
                digitTable[i] = j;
            }
        }
        public DecimalInt(int num) => number = num.ToString();
        public DecimalInt(string num)
        {
            if (Regex.IsMatch(num, "\\d+"))
            {
                number = num;
            }
            else
            {
                throw new ArgumentException("Number can't be cast to int");
            }
        }
        static int CharToInt(char c)
        {
            return digitTable[c];
        }
        static string DigitSum(char a, char b, bool memory = false)
        {
            if (memory)
            {
                StringBuilder str = new StringBuilder(sumTable[CharToInt(a), CharToInt(b)]);
                if (str.Length == 2)
                {
                    str.Append(sumTable[CharToInt(str[1]), 1]);
                    str.Remove(1, 1);
                    return str.ToString();
                }
                else
                {
                    return sumTable[CharToInt(str[0]), 1];
                }
            }
            else
            {
                return sumTable[CharToInt(a), CharToInt(b)];
            }
        }
        static string DigitDif(char a, char b, bool memory = false)
        {
            if (memory)
            {
                StringBuilder str = new StringBuilder(difTable[CharToInt(a), CharToInt(b)]);
                if (str.Length == 2)
                {
                    str.Append(difTable[CharToInt(str[1]), 1]);
                    str.Remove(1, 1);
                    return str.ToString();
                }
                else
                {
                    return difTable[CharToInt(str[0]), 1];
                }
            }
            else
            {
                return difTable[CharToInt(a), CharToInt(b)];
            }
        }

        public static DecimalInt operator +(DecimalInt a, DecimalInt b)
        {
            StringBuilder sum = new StringBuilder();
            string digSum = DigitSum(a.number[a.number.Length - 1], b.number[b.number.Length - 1]);
            for (int i = a.number.Length - 2, j = b.number.Length - 2;
                i >= 0 && j >= 0;
                i--, j--)
            {
                if (digSum.Length == 2)
                {
                    sum.Insert(0, digSum[1]);
                    digSum = DigitSum(a.number[i], b.number[j], true);
                }
                else 
                {
                    sum.Insert(0, digSum[0]);
                    digSum = DigitSum(a.number[i], b.number[j]);
                }
            }
            sum.Insert(0, digSum);
            return new DecimalInt(sum.ToString());
        }
        public static DecimalInt operator -(DecimalInt a, DecimalInt b)
        {
            StringBuilder dif = new StringBuilder();
            string digDif = DigitDif(a.number[a.number.Length - 1], b.number[b.number.Length - 1]);
            for (int i = a.number.Length - 2, j = b.number.Length - 2;
                i >= 0 && j >= 0;
                i--, j--)
            {
                if (digDif.Length == 2)
                {
                    dif.Insert(0, digDif[1]);
                    digDif = DigitDif(a.number[i], b.number[j], true);
                }
                else 
                {
                    dif.Insert(0, digDif[0]);
                    digDif = DigitDif(a.number[i], b.number[j]);
                }
            }
            dif.Insert(0, digDif);
            return new DecimalInt(dif.ToString());
        }

        public override string ToString()
        {
            return number;
        }
    }
}
