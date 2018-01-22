using UnityEngine;
using System.Collections;


public static class StringExtension
{
    public static string DecodeSlashChar(this string str)
    {
        return str.Replace("\\n", "\n").Replace("\\t", "\t");
    }

    public static string ToLowerFast(this string value)
    {
        if (value.IsLower())
        {
            return value;
        }

        char[] output = value.ToCharArray();
        for (int i = 0; i < output.Length; i++)
        {
            if (output[i] >= 'A' &&
            output[i] <= 'Z')
            {
                output[i] = (char)(output[i] + 32);
            }
        }
        return new string(output);
    }

    public static bool IsUpper(this string value)
    {
        // Consider string to be uppercase if it has no lowercase letters.
        for (int i = 0; i < value.Length; i++)
        {
            if (char.IsLower(value[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsLower(this string value)
    {
        // Consider string to be lowercase if it has no uppercase letters.
        for (int i = 0; i < value.Length; i++)
        {
            if (char.IsUpper(value[i]))
            {
                return false;
            }
        }
        return true;
    }
}
