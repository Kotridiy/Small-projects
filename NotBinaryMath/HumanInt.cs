using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NotBinaryMath
{
    struct HumanInt
    {
        private string str;
        public int Number { get => int.Parse(str); }

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
            {"0", "-9", "-8", "-7", "-6", "-5", "-4", "-3", "-2", "-1" },
            {"1", "0", "-9", "-8", "-7", "-6", "-5", "-4", "-3", "-2" },
            {"2", "1", "0", "-9", "-8", "-7", "-6", "-5", "-4", "-3" },
            {"3", "2", "1", "0", "-9", "-8", "-7", "-6", "-5", "-4" },
            {"4", "3", "2", "1", "0", "-9", "-8", "-7", "-6", "-5" },
            {"5", "4", "3", "2", "1", "0", "-9", "-8", "-7", "-6" },
            {"6", "5", "4", "3", "2", "1", "0", "-9", "-8", "-7" },
            {"7", "6", "5", "4", "3", "2", "1", "0", "-9", "-8" },
            {"8", "7", "6", "5", "4", "3", "2", "1", "0", "-9" },
            {"9", "8", "7", "6", "5", "4", "3", "2", "1", "0" },
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
        private static readonly int[] digitTable = new int[58];

        static HumanInt()
        {
            for (int i = 49, j = 1; i < 58; i++, j++)
            {
                digitTable[i] = j;
            }
        }
        public HumanInt(int num) => str = num.ToString();
        public HumanInt(string num)
        {
            if (Regex.IsMatch(num, "[-]?\\d+"))
            {
                str = num;
            }
            else
            {
                throw new ArgumentException("Number can't be cast to int");
            }
        }
        public HumanInt(HumanInt dec)
        {
            str = (string)dec.str.Clone();
        }
        private static HumanInt GetDecimal(string num)
        {
            return new HumanInt { str = num };
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

        static HumanInt NumberSum(HumanInt a, HumanInt b)
        {
            StringBuilder sum = new StringBuilder();
            var strA = ReverseString(a.str);
            var strB = ReverseString(b.str);
            if (strA.Length > strB.Length)
            {
                strB = strB.PadRight(strA.Length, '0');
            }
            else
            {
                strA = strA.PadRight(strB.Length, '0');
            }
            string digSum = DigitSum(strA[0], strB[0]);
            for (int i = 1; i < strA.Length; i++)
            {
                if (digSum.Length == 2)
                {
                    sum.Append(digSum[1]);
                    digSum = DigitSum(strA[i], strB[i], true);
                }
                else
                {
                    sum.Append(digSum[0]);
                    digSum = DigitSum(strA[i], strB[i]);
                }
            }
            return GetDecimal(digSum + ReverseString(sum.ToString()));
        }
        static HumanInt NumberDif(HumanInt a, HumanInt b)
        {
            StringBuilder dif = new StringBuilder();
            var strA = ReverseString(a.str);
            var strB = ReverseString(b.str);
            if (strA.Length > strB.Length)
            {
                strB = strB.PadRight(strA.Length, '0');
            }
            else
            {
                strA = strA.PadRight(strB.Length, '0');
            }
            string digDif = DigitDif(strA[0], strB[0]);
            for (int i = 1; i < strA.Length; i++)
            {
                if (digDif.Length == 2)
                {
                    dif.Append(digDif[1]);
                    digDif = DigitDif(strA[i], strB[i], true);
                }
                else
                {
                    dif.Append(digDif[0]);
                    digDif = DigitDif(strA[i], strB[i]);
                }
            }
            dif.Append(digDif);
            var result = ReverseString(dif.ToString()).TrimStart('0');
            if (result.Length == 0)
            {
                result = "0";
            }
            return GetDecimal(result);
        }
        static HumanInt NumberMul(HumanInt a, HumanInt b)
        {
            if (b.str == "0")
            {
                return b;
            }
            if (b.str == "1")
            {
                return a;
            }
            string strA = ReverseString(a.str), strB = ReverseString(b.str);
            HumanInt sum = new HumanInt(0);
            for (int i = 0; i < strA.Length; i++)
            {
                for (int j = 0; j < strB.Length; j++)
                {
                    var mulstr = mulTable[CharToInt(strA[i]), CharToInt(strB[j])] + new string('0', i+j);
                    sum = NumberSum(sum, GetDecimal(mulstr));
                }
            }
            return sum;
        }

        public static HumanInt operator +(HumanInt a, HumanInt b)
        {
            if (IsPositive(a) && IsPositive(b))
            {
                return NumberSum(a, b);
            }
            else if (IsNegative(a) && IsNegative(b))
            {
                return -NumberSum(-a, -b);
            }
            else if (IsPositive(a) && IsNegative(b))
            {
                if (a >= -b)
                {
                    return NumberDif(a, -b);
                }
                else
                {
                    return -NumberDif(-b, a);
                }
            }
            else
            {
                if (-a <= b)
                {
                    return NumberDif(b, -a);
                }
                else
                {
                    return -NumberDif(-a, b);
                }
            }
        }
        public static HumanInt operator -(HumanInt a, HumanInt b)
        {
            if(IsPositive(a) && IsNegative(b))
            {
                return NumberSum(a, -b);
            }
            else if (IsNegative(a) && IsPositive(b))
            {
                return -NumberSum(-a, b);
            }
            else if (IsPositive(a) && IsPositive(b))
            {
                if (a >= b)
                {
                    return NumberDif(a, b);
                }
                else
                {
                    return -NumberDif(b, a);
                }
            }
            else
            {
                if (a <= b)
                {
                    return -NumberDif(-a, -b);
                }
                else
                {
                    return NumberDif(-b, -a);
                }
            }
        }
        public static HumanInt operator *(HumanInt a, HumanInt b)
        {
            bool sign1 = false, sign2 = false;
            if (IsNegative(a))
            {
                sign1 = true;
                a = -a;
            }
            if (IsNegative(b))
            {
                sign2 = true;
                b = -b;
            }
            if (b > a)
            {
                Swap(a, b);
            }
            if (sign1 ^ sign2)
            {
                return -NumberMul(a, b);
            }
            else
            {
                return NumberMul(a, b);
            }
        }

        public static bool operator >(HumanInt a, HumanInt b)
        {
            return Compare(a, b) > 0;
        }
        public static bool operator <(HumanInt a, HumanInt b)
        {
            return Compare(a, b) < 0;
        }
        public static bool operator >=(HumanInt a, HumanInt b)
        {
            return Compare(a, b) >= 0;
        }
        public static bool operator <=(HumanInt a, HumanInt b)
        {
            return Compare(a, b) <= 0;
        }

        public static HumanInt operator -(HumanInt num)
        {
            if (num.str != "0")
            {
                if (num.str[0] == '-')
                {
                    num.str = num.str.Remove(0, 1);
                }
                else
                {
                    num.str = "-" + num.str;
                }
            }
            return num;
        }

        public override string ToString()
        {
            return str;
        }

        public override bool Equals(object obj)
        {
            return str == ((HumanInt)obj).str;
        }

        public override int GetHashCode()
        {
            return str.GetHashCode() + 10;
        }

        public static int Compare(HumanInt a, HumanInt b)
        {
            if (IsPositive(a) && IsNegative(b)) return 1;
            if (IsNegative(a) && IsPositive(b)) return -1;
            int multi = 1;
            if (IsNegative(a) && IsNegative(b)) multi = -1;
            
            if (a.str.Length != b.str.Length)
            {
                return multi * (a.str.Length > b.str.Length ? 1 : -1);
            }
            else
            {
                return multi * (String.CompareOrdinal(a.str, b.str) > 0 ? 1 : -1);
            }
        }

        static void Swap(HumanInt a, HumanInt b)
        {
            HumanInt dop = a;
            a = b;
            b = dop;
        }
        static string ReverseString(string str)
        {
            char[] chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        static bool IsNegative(HumanInt num) => num.str[0] == '-';
        static bool IsPositive(HumanInt num) => num.str[0] != '-';
    }
}