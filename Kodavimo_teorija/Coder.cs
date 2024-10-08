namespace Kodavimo_teorija;
using System;

public static class Coder
{
    public static string code(string input)
    {
        // Golay B matrix
        int[,] matrix = new int[,]
        {
            {1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0},
            {1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1}, 
            {0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1},
            {1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0},
            {1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1},
            {1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1},
            {0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1},
            {0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0},
            {0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0},
            {1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0},
            {0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        List<String> substrings = StringTool.DivideString(input, 12);
        List<String> codedSubstrings = new List<string>();

        foreach (var substring in substrings)
        {
            // Multiply binary vector with matrix;
            codedSubstrings.Add(substring + MultiplyBinaryVectorWithMatrix(substring, matrix));
        }

        return StringTool.CombineStrings(codedSubstrings);
    }

    public static string FillBitArray(string bitArray,ref int fill)
    {
        string output = bitArray;
        while (output.Length % 12 != 0)
        {
            output += "0";
            fill += 1;
        }

        return output;
    }


    static string MultiplyBinaryVectorWithMatrix(string binaryVector, int[,] matrix)
    {
        if (binaryVector.Length != matrix.GetLength(0))
        {
            throw new ArgumentException("Vector length must match matrix row count.");
        }

        int[] resultArray = new int[matrix.GetLength(1)];

        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                resultArray[i] += (binaryVector[j] - '0') * matrix[j, i];
            }
            resultArray[i] %= 2; // Apply modulo 2 for binary result
        }

        return string.Join("", resultArray);
    }
}