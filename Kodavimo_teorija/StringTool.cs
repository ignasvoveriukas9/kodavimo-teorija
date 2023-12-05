using System.Text;

namespace Kodavimo_teorija;

public static class StringTool
{
    public static List<string> DivideString(string input, int substringLength)
    {
        if (input.Length % substringLength != 0)
        {
            throw new ArgumentException("Input string length must be a multiple of the specified substring length.");
        }

        List<string> substrings = new List<string>();

        for (int i = 0; i < input.Length; i += substringLength)
        {
            substrings.Add(input.Substring(i, substringLength));
        }

        return substrings;
    }
    
    public static string CombineStrings(List<string> substrings)
    {
        StringBuilder combinedStringBuilder = new StringBuilder();

        foreach (string substring in substrings)
        {
            combinedStringBuilder.Append(substring);
        }

        return combinedStringBuilder.ToString();
    }
}