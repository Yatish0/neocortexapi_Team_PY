//-------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
// Use the Code Below to Generate SDRs From Scalar values
// It takes Scalar Values as an input and generates SDR and Represent it into Bitmaps

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using GemBox.Spreadsheet;
using GemBox.Spreadsheet.Drawing;
using GemBox.Spreadsheet.Charts;
using NeoCortex;
using NeoCortexApi.Encoders;
using NeoCortexApi.Network;
using NeoCortexApi.Utility;

namespace WorkingWithSDRs
{
    public class SdrRepresentation
    {
        public static void Main(string[] args)
        {
            SdrRepresentation program = new SdrRepresentation();
            program.GenerateSDR();
        }

        public void GenerateSDR()
        {
            Console.WriteLine("Enter the first input value:");
            double firstInput = ReadDoubleInput();

            Console.WriteLine("Enter the second input value:");
            double secondInput = ReadDoubleInput();

            ScalarEncodingExperiment(firstInput);
            ScalarEncodingExperiment(secondInput);
        }

        private double ReadDoubleInput()
        {
            double input;
            while (true)
            {
                string inputStr = Console.ReadLine();
                if (double.TryParse(inputStr, out input))
                    break;
                else
                    Console.WriteLine("Invalid input. Please enter a valid double value:");
            }
            return input;
        }

        public void ScalarEncodingExperiment(double input)
        {
            string outFolder = nameof(ScalarEncodingExperiment);
            Directory.CreateDirectory(outFolder);
            DateTime now = DateTime.Now;

            ScalarEncoder encoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 21},
                { "N", 1024},
                { "Radius", -1.0},
                { "MinVal", 0.0},
                { "MaxVal", 100.0 },
                { "Periodic", false},
                { "Name", "scalar"},
                { "ClipInput", false},
            });

            int[] result = encoder.Encode(input);
            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, (int)Math.Sqrt(result.Length), (int)Math.Sqrt(result.Length));
            int[,] twoDimArray = ArrayUtils.Transpose(twoDimenArray);
            NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{outFolder}\\{input}.png", Color.Yellow, Color.Black, text: input.ToString());
        }
    }
}
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
// Use the Code Below to Generate SDRs From Date and Time
// It takes Date and Time as an input and generates SDR and Represent it into Bitmaps

/*
 using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using GemBox.Spreadsheet;
using GemBox.Spreadsheet.Drawing;
using GemBox.Spreadsheet.Charts;
using NeoCortex;
using NeoCortexApi.Encoders;
using NeoCortexApi.Network;
using NeoCortexApi.Utility;

namespace WorkingWithSDRs
{
    public class SdrRepresentation
    {
        public static void Main(string[] args)
        {
            SdrRepresentation program = new SdrRepresentation();
            program.GenerateSDR();
        }

        public void GenerateSDR()
        {
            Console.WriteLine("Enter the input value (in DateTime format):");
            string input = Console.ReadLine();
            EncodeDateTimeTest(input);
        }

        public void EncodeDateTimeTest(string input)
        {
            CortexNetworkContext ctx = new CortexNetworkContext();
            DateTimeOffset now = DateTimeOffset.Now;
            Dictionary<string, Dictionary<string, object>> encoderSettings = new Dictionary<string, Dictionary<string, object>>();
            encoderSettings.Add("DateTimeEncoder", new Dictionary<string, object>()
            {
                { "W", 21},
                { "N", 1024},
                { "MinVal", now.AddYears(-10)},
                { "MaxVal", now},
                { "Periodic", false},
                { "Name", "DateTimeEncoder"},
                { "ClipInput", false},
                { "Padding", 5},
            });
            DateTimeEncoder encoder = new DateTimeEncoder(encoderSettings, DateTimeEncoder.Precision.Days);
            int[] result = encoder.Encode(DateTimeOffset.Parse(input));
            Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(result));
            //Debug.WriteLine(NeoCortexApi.Helpers.StringifyVector(expectedOutput));
            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, 32, 32);
            int[,] twoDimArray = ArrayUtils.Transpose(twoDimenArray);
            NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"DateTime_out_{input.Replace("/", "-").Replace(":", "-")}_32x32-N-{encoderSettings["DateTimeEncoder"]["N"]}-W-{encoderSettings["DateTimeEncoder"]["W"]}.png");
            // Assert.IsTrue(result.SequenceEqual(expectedOutput));
        }
    }
}
*/