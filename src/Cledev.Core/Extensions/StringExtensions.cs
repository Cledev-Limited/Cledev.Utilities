using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Cledev.Core.Extensions;

public static class StringExtensions
{
    public static string ToSafeLengthText(this string text, int maxLength, string suffix = "...")
    {
        if (maxLength == 0)
        {
            return text;
        }

        return text.Length > maxLength ? $"{text.Substring(0, maxLength)}{suffix}" : text;
    }

    public static string InsertSpaceBeforeUpperCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        var stringBuilder = new StringBuilder();

        var previousChar = char.MinValue;

        foreach (var c in text)
        {
            if (char.IsUpper(c))
            {
                if (stringBuilder.Length != 0 && previousChar != ' ')
                {
                    stringBuilder.Append(' ');
                }
            }

            stringBuilder.Append(c);

            previousChar = c;
        }

        return stringBuilder.ToString();
    }

    public static string ToSlug(this string text, int maxLength = 50, char divider = '-', bool removeMultipleSpaces = true)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (removeMultipleSpaces)
        {
            text = Regex.Replace(text, @"\s+", " ");
        }

        var stringBuilder = new StringBuilder();

        foreach (var c in text.ToArray())
        {
            if (char.IsLetterOrDigit(c))
            {
                stringBuilder.Append(c);
            }
            else if (c == ' ')
            {
                stringBuilder.Append(divider);
            }
        }

        var result = stringBuilder.ToString().ToLower();

        return result.ToSafeLengthText(maxLength, string.Empty);
    }

    /// <summary>
    /// https://www.danesparza.net/2010/10/using-gravatar-images-with-c-asp-net/
    /// </summary>
    public static string ToGravatarEmailHash(this string email)
    {
        // Create a new instance of the MD5CryptoServiceProvider object.  
        var md5Hasher = MD5.Create();

        // Convert the input string to a byte array and compute the hash.  
        var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));

        // Create a new string builder to collect the bytes  
        // and create a string.  
        StringBuilder sBuilder = new();

        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string.  
        for (var i = 0; i < data.Length; i++)
        {
            sBuilder.Append(i.ToString("x2"));
        }

        return sBuilder.ToString(); // Return the hexadecimal string. 
    }
}