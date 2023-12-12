using System.Security.AccessControl;
using System.Text;

namespace Kodavimo_teorija;

public static class Sender
{
    public static string send(string input, float errorProb, Random rnd)
    {
        var builder = new StringBuilder();
        string output;

        foreach (var bit in input)
        {
            if (rnd.NextDouble() < errorProb)
            {
                if (bit == '1')
                {
                    builder.Append('0');
                }else if (bit == '0')
                {
                    builder.Append('1');
                }
                else
                {
                    throw new Exception("not bit");
                }
            }
            else
            {
                builder.Append(bit);
            }
        }


        output = builder.ToString();

        return output;
    }
}