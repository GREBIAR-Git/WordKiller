﻿using System;
using System.Linq;
using System.Text;
using WordKiller.Scripts.File.Encryption;

namespace WordKiller.Scripts.ImportExport.Encryption;

class Encryption : IEncryption
{
    public string Encrypt(string text)
    {
        StringToBinaryString(ref text);
        text = RepeatEncodingBinary(text);
        DigitsToAbc(ref text);
        return text;
    }

    static void StringToBinaryString(ref string str)
    {
        str = string.Join("", Encoding.UTF8.GetBytes(str).Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
    }

    static string RepeatEncodingBinary(string binarystring)
    {
        StringBuilder str = new(binarystring);
        StringBuilder encoded = new();
        for (int i = 0; i < str.Length; i++)
        {
            int f = 1;
            for (; f < 10 && i + f < str.Length; f++)
            {
                if (str[i] != str[f + i])
                {
                    break;
                }
            }
            encoded.Append(f.ToString() + str[i]);
            i += f - 1;
        }
        return encoded.ToString();
    }

    static void DigitsToAbc(ref string digits)
    {
        string dictionary = "abcdefghij";
        digits = new string(digits.Select(x => dictionary[x - 48]).ToArray());
    }

    public string Decrypt(string text)
    {
        AbcToDigits(ref text);
        text = RepeatDecodingBinary(text);
        text = BinaryStringToString(text);
        return text;
    }

    static void AbcToDigits(ref string abc)
    {
        string dictionary = "abcdefghij";
        abc = new string(abc.Select(x => dictionary.IndexOf(x).ToString()[0]).ToArray());
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
        string normal = Encoding.UTF8.GetString(Enumerable.Range(0, binary.Length / 8).Select(i => Convert.ToByte(binary.Substring(i * 8, 8), 2)).ToArray());
        normal = normal.Remove(normal.Length - 1, 1);
        normal = normal.Replace("\n", "\r\n");
        return normal;
    }
}