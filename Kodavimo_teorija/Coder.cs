namespace Kodavimo_teorija;

public static class Coder
{
    public static string code(string input)
    {
        string output;
        
        //TODO inplement coding part

        output = input;

        return output;
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
}