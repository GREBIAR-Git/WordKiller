using System;
using System.Linq;
using System.Text;

namespace WordKiller.Scripts.File.Encryption;

internal class RLEEncryption : IEncryption
{
    public string Encrypt(string text)
    {
        StringToBinaryString(ref text);
        text = RepeatEncodingBinary(text);
        DigitsToAbc(ref text);
        return text;
    }

    public string Decrypt(string text)
    {
        AbcToDigits(ref text);
        text = RepeatDecodingBinary(text);
        text = BinaryStringToString(text);
        return text;
    }

    static void StringToBinaryString(ref string str)
    {
        str = string.Join("", Encoding.UTF8.GetBytes(str).Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
    }

    static string RepeatEncodingBinary(string binarystring)
    {
        StringBuilder output = new();
        if (binarystring.Length > 0)
        {
            char currentSybmol = binarystring[0];
            int count = 1;
            foreach (char symbol in binarystring[1..])
            {
                if (symbol == currentSybmol)
                {
                    count++;
                }
                else
                {
                    Repeat(ref output, currentSybmol, count);
                    currentSybmol = symbol;
                    count = 1;
                }
            }

            Repeat(ref output, currentSybmol, count);
        }

        return output.ToString();
    }

    static void Repeat(ref StringBuilder output, char currentSybmol, int count)
    {
        int f = count / 9;
        for (int i = 0; i < f; i++)
        {
            output.Append(9);
            output.Append(currentSybmol);
        }

        if (count > 9 * f)
        {
            int c = count - 9 * f;
            output.Append(c);
            output.Append(currentSybmol);
        }
    }

    static void DigitsToAbc(ref string digits)
    {
        const string dictionary = "abcdefghij";
        digits = new(digits.Select(x => dictionary[x - 48]).ToArray());
    }

    static void AbcToDigits(ref string abc)
    {
        const string dictionary = "abcdefghij";
        abc = new(abc.Select(x => dictionary.IndexOf(x).ToString()[0]).ToArray());
    }

    static string RepeatDecodingBinary(string repeated_digit)
    {
        StringBuilder str = new(repeated_digit);
        StringBuilder decoded = new();
        for (int index = 1; index < str.Length; index += 2)
        {
            decoded.Append(new string(str[index], str[index - 1] - 48));
        }

        return decoded.ToString();
    }

    static string BinaryStringToString(string binary)
    {
        string normal = Encoding.UTF8.GetString(Enumerable.Range(0, binary.Length / 8)
            .Select(i => Convert.ToByte(binary.Substring(i * 8, 8), 2)).ToArray());
        return normal;
    }
}