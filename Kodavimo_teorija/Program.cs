// See https://aka.ms/new-console-template for more information

using System.Globalization;
using Kodavimo_teorija;
using System.Text;

int q = 2;
string bitArray;

while (true)
{
    String inputText;
    Random rnd = new Random();
    
    // user input
    
    Console.WriteLine("Klaidos tikimybe:");
    string input = Console.ReadLine();

    input = input.Replace(',', '.');
    
    byte[] headerData = null;
    string outputPath = null;

    float errorProb;
    try
    {
        errorProb = float.Parse(input,CultureInfo.InvariantCulture);
    }
    catch
    {
        Console.WriteLine("error not float");
        return 1;
    }

    Console.WriteLine("1. enter bit array\n2. enter text\n3. enter image\n0. exit");

    string inputSelect;
    inputSelect = Console.ReadLine();
    if (inputSelect == "1")
    {

        Console.WriteLine("Enter the code:");
        bitArray = Console.ReadLine();
        inputText = "";

    }
    else if (inputSelect == "2")
    {

        // read user text
        Console.WriteLine("Enter your text (press Ctrl+Z or Ctrl+D to end input):");

        StringBuilder sb = new StringBuilder();
        string line;
        while ((line = Console.ReadLine()) != null)
        {
            sb.AppendLine(line);
        }

        inputText = sb.ToString();

        // convert to bit
        byte[] bytes = Encoding.UTF8.GetBytes(inputText);

        bitArray = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }
    else if (inputSelect == "3")
    {
        inputText = "";
        Console.WriteLine("Enter the image location:");
        string imagePath = Console.ReadLine();
        Console.WriteLine("Enter the output destination location:");
        outputPath = Console.ReadLine();

        using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[54]; // BMP header is 54 bytes
            fs.Read(buffer, 0, 54);

            // Extract the header and save it
            headerData = buffer;

            // Read the rest of the image data
            int dataLength = (int)fs.Length - 54;
            byte[] imageData = new byte[dataLength];
            fs.Read(imageData, 0, dataLength);

            // Convert the image data to a binary string
            bitArray = string.Join("", imageData.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

        }
    }
    else if (inputSelect == "0")
    {
        return 0;
    }
    else
    {
        Console.WriteLine("Incorrect input");
        return 1;
    }
    

// coding, sending and decoding

    int fillAmount = 0;

    Console.Clear();
    
    string filledBitArray = Coder.FillBitArray(bitArray,ref fillAmount);
    //string filledBitArray = bitArray;

    string codedMsg = Coder.code(filledBitArray);

    Console.WriteLine("coded message: " + codedMsg + "\n");

    string sentMsg;
    try
    {
        sentMsg = Sender.send(codedMsg, errorProb, rnd);
    }
    catch
    {
        Console.WriteLine("send not bit");
        return 1;
    }

    Console.WriteLine("sent message: " + sentMsg + "\n");

    List<int> errors = new List<int>();

    for (int i = 0; i < sentMsg.Length; i++)
    {
        if (sentMsg[i] != codedMsg[i])
        {
            errors.Add(i+1);
        }
    }

    Console.WriteLine("Number of errors: " + errors.Count.ToString());
    Console.WriteLine("errors in positions: ");

    foreach (var i in errors)
    {
        Console.Write(i.ToString() + " ");
    }

    if (inputSelect == "1")
    {
        
        Console.WriteLine("\nDo you want to edit sent message? (y/n)");
        string edit = Console.ReadLine();

        if (edit == "y")
        {
            Console.WriteLine("Enter the edited message:");
            sentMsg = Console.ReadLine();
        }

    }

    string decodedMsgWithFill = Kodavimo_teorija.Decoder.Decode(sentMsg);

    string decodedMsg = decodedMsgWithFill.Substring(0, decodedMsgWithFill.Length - fillAmount);

    Console.WriteLine("\n\ndecoded message: " + decodedMsg);

// if text
    if (inputSelect == "2")
    {
        Console.WriteLine("\nOriginal text:\n" + inputText.ToString());

            // without coding
        string textMsgWithoutCode;
        try
        {
            textMsgWithoutCode = Sender.send(bitArray, errorProb, rnd);
        }
        catch
        {
            Console.WriteLine("send not bit");
            return 1;
        }
        
        StringBuilder textWithoutCode = new StringBuilder();

        for (int i = 0; i < textMsgWithoutCode.Length; i += 8)
        {
            string binaryByte = textMsgWithoutCode.Substring(i, 8);
            byte byteValue = Convert.ToByte(binaryByte, 2);
            textWithoutCode.Append((char)byteValue);
        }

        Console.WriteLine("Text sent without coding:\n" + textWithoutCode.ToString());
        
        //with coding
        StringBuilder text = new StringBuilder();

        for (int i = 0; i < decodedMsg.Length; i += 8)
        {
            string binaryByte = decodedMsg.Substring(i, 8);
            byte byteValue = Convert.ToByte(binaryByte, 2);
            text.Append((char)byteValue);
        }

        Console.WriteLine("decoded text:\n" + text.ToString());
// if image    
    }
    else if (inputSelect == "3")
    {
        int dataLength = decodedMsg.Length / 8;
        byte[] imageData = new byte[dataLength];

        for (int i = 0; i < dataLength; i++)
        {
            string binaryByte = decodedMsg.Substring(i * 8, 8);
            imageData[i] = Convert.ToByte(binaryByte, 2);
        }

        // Combine header data and image data into a single byte array
        byte[] fullImageData = new byte[headerData.Length + imageData.Length];
        Array.Copy(headerData, fullImageData, headerData.Length);
        Array.Copy(imageData, 0, fullImageData, headerData.Length, imageData.Length);

        // Save the complete image data to a BMP file
        File.WriteAllBytes(outputPath + "/codedImage", fullImageData);

        //without coding

        string imgWithoutCoding = Sender.send(bitArray, errorProb, rnd);
        
        dataLength = imgWithoutCoding.Length / 8;
        imageData = new byte[dataLength];
        
        for (int i = 0; i < dataLength; i++)
        {
            string binaryByte = imgWithoutCoding.Substring(i * 8, 8);
            imageData[i] = Convert.ToByte(binaryByte, 2);
        }

        // Combine header data and image data into a single byte array
        fullImageData = new byte[headerData.Length + imageData.Length];
        Array.Copy(headerData, fullImageData, headerData.Length);
        Array.Copy(imageData, 0, fullImageData, headerData.Length, imageData.Length);

        // Save the complete image data to a BMP file
        File.WriteAllBytes(outputPath + "/noCodedImage", fullImageData);
    }

    Console.WriteLine("press enter to continue");
    Console.ReadLine();
    Console.Clear();
}