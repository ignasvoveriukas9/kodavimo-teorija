namespace Kodavimo_teorija;
using System;

public static class Decoder
{
    public static string Decode (String input)
    {
        List<String> substrings = StringTool.DivideString(input, 23);
        List<String> decodedSubstrings = new List<string>();

        foreach (var substring in substrings)
        {
            string errorPattern = FindErrorPatternGolay(substring);
            string correctedSubstring =
                ArrayToString(AddArrays(StringToArray(substring), StringToArray(errorPattern)), 23);
            decodedSubstrings.Add(correctedSubstring.Substring(0,12));
            
        }

        return StringTool.CombineStrings(decodedSubstrings);
    }
    static string FindErrorPatternGolay(string w)
    {
        if (w.Length != 23)
        {
            throw new ArgumentException("Input vector length must be 23.");
        }

        // Matrix B
        int[,] B = new int[,]
        {
            {1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1},
            {1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1}, 
            {0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1},
            {1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1},
            {1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1},
            {0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1},
            {0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1},
            {0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1},
            {1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1},
            {0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}
        };

        // Matrix H
        int[,] H = new int[,]
        {
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1},
            {1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1}, 
            {0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1},
            {1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1},
            {1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1},
            {1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1},
            {0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1},
            {0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1},
            {0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1},
            {1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1},
            {0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}
        };

        // Step 1: Add 0 or 1 to w to make the weight odd
        w = MakeWeightOdd(w);

        // Step 2: Compute syndrome s = wH
        int[] s = ComputeSyndrome(w, H);

        // Step 3: If wt(s) <= 3 then u = [s, 0]
        if (Weight(s) <= 3)
        {
            return ArrayToString(s,s.Length) + ArrayToString(new int[12], 12);
        }

        // Step 4: If wt(s + bi) <= 2 for some row bi of B then u = [s + bi, ei]
        for (int i = 0; i < B.GetLength(0); i++)
        {
            int[] sBi = AddArrays(s, GetRow(B, i));
            if (Weight(sBi) <= 2)
            {
                return ArrayToString(sBi, 12) + ArrayToString(GetRow(IdentityMatrix(12), i), 12);
            }
        }

        // Step 5: Compute the second syndrome sB
        int[] sB = ComputeSyndrome(ArrayToString(s, s.Length), B);

        // Step 6: If wt(sB) <= 3 then u = [0, sB]
        if (Weight(sB) <= 3)
        {
            return ArrayToString(new int[12], 12) + ArrayToString(sB, 12);
        }

        // Step 7: If wt(sB + bi) <= 2 for some row bi of B then u = [ei, sB + bi]
        for (int i = 0; i < B.GetLength(0); i++)
        {
            int[] sBbi = AddArrays(sB, GetRow(B, i));
            if (Weight(sBbi) <= 2)
            {
                return ArrayToString(GetRow(IdentityMatrix(12), i), 12) + ArrayToString(sBbi, 12);
            }
        }

        // Step 8: If u is not yet determined, throw an exception
        throw new InvalidOperationException("Decoding failed. Unable to determine u.");
    }

    static string MakeWeightOdd(string vector)
    {
        // If the weight is already odd, add 0; otherwise, add 1
        return vector + (Weight(vector) % 2 == 0 ? "1" : "0");
    }
    
    static int Weight(string vector)
    {
        int weight = 0;

        foreach (char bit in vector)
        {
            weight += (bit - '0');
        }

        return weight;
    }

    static int[] ComputeSyndrome(string vector, int[,] matrix)
    {
        int[] syndrome = new int[matrix.GetLength(1)];

        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                syndrome[i] += (vector[j] - '0') * matrix[j, i];
            }
            syndrome[i] %= 2; // Apply modulo 2 for binary result
        }

        return syndrome;
    }

    static int Weight(int[] array)
    {
        int weight = 0;

        foreach (int element in array)
        {
            weight += element;
        }

        return weight;
    }

    static int[] AddArrays(int[] array1, int[] array2)
    {
        int[] result = new int[array1.Length];

        for (int i = 0; i < array1.Length; i++)
        {
            result[i] = (array1[i] + array2[i]) % 2;
        }

        return result;
    }

    static int[] GetRow(int[,] matrix, int rowIndex)
    {
        int rowLength = matrix.GetLength(1);
        int[] row = new int[rowLength];

        for (int i = 0; i < rowLength; i++)
        {
            row[i] = matrix[rowIndex, i];
        }

        return row;
    }

    static int[,] IdentityMatrix(int size)
    {
        int[,] identityMatrix = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            identityMatrix[i, i] = 1;
        }

        return identityMatrix;
    }

    static string ArrayToString(int[] array, int length)
    {
        return string.Join("", array).Substring(0, length);
    }
    
    static int[] StringToArray(string s)
    {
        int[] array = new int[s.Length];

        for (int i = 0; i < s.Length; i++)
        {
            array[i] = (s[i] - '0');
        }

        return array;
    }
}