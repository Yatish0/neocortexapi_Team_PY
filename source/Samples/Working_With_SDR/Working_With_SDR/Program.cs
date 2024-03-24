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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortexApi.Entities;
using NeoCortexApi;

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
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------------------------------

// Below is the code for GeoSpatial Encoder

/*
using NeoCortex;
using NeoCortexApi.Encoders;
using NeoCortexApi.Utility;
using System;
using System.Drawing;

namespace UnitTestsProject.EncoderTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of the Program class
            Program program = new Program();

            // Call the method to test the GeoSpatialEncoder and draw bitmaps
            program.GeoSpatialEncoderTestDrawBitMap(48.0); // Pass a latitude value for testing

            Console.WriteLine("GeoSpatial encoding and bitmap drawing completed. Press any key to exit.");
            Console.ReadKey();
        }

        public void GeoSpatialEncoderTestDrawBitMap(double input)
        {
            string outFolder = nameof(GeoSpatialEncoderTestDrawBitMap);

            Dictionary<string, object> encoderSettings = new Dictionary<string, object>();
            encoderSettings.Add("W", 21);
            encoderSettings.Add("N", 40);
            encoderSettings.Add("MinVal", (double)48.75); // latitude value of Italy 
            encoderSettings.Add("MaxVal", (double)51.86);// latitude value of Germany
            encoderSettings.Add("Radius", (double)1.5);
            encoderSettings.Add("Periodic", (bool)false);
            encoderSettings.Add("ClipInput", (bool)true);
            encoderSettings.Add("IsRealCortexModel", false);

            GeoSpatialEncoderExperimental encoder = new GeoSpatialEncoderExperimental(encoderSettings);

            var result = encoder.Encode(input);
            printImage(encoder, nameof(GeoSpatialEncoderTestDrawBitMap));
        }

        public void printImage(GeoSpatialEncoderExperimental encoder, string folderName)
        {
            Directory.CreateDirectory(folderName);
            for (double j = (long)encoder.MinVal; j < (long)encoder.MaxVal; j += 1)
            {
                var result2 = encoder.Encode(j);
                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result2, (int)Math.Sqrt(result2.Length), (int)Math.Sqrt(result2.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);
                //NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{folderName}\\{j}.png", Color.LightSeaGreen, Color.Black, text: j.ToString());
                NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{folderName}\\{j}.png", Color.Red, Color.Green, text: j.ToString());
            }
        }
    }
}
*/
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
/*
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortex;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace UnitTestsProject.Sdr
{
    /// <summary>
    /// Describe how to create SDRs
    /// </summary>
    [TestClass]
    public class SpatialPoolerColumnActivityTest
    {
        private const int OutImgSize = 1024;

        [TestMethod]
        public void CreateSdrsTest()
        {
            var colDims = new int[] { 64, 64 };
            int numOfCols = 64 * 64;

            // Adjust this path to point to the correct location of test files
            string trainingFolder = @"source\UnitTestsProject\TestFiles\Sdr";

            int imgSize = 28;

            var trainingImages = Directory.GetFiles(trainingFolder, "*.jpeg");

            Directory.CreateDirectory($"{nameof(CreateSdrsTest)}");

            int counter = 0;

            bool isInStableState = false;

            // HTM parameters
            HtmConfig htmConfig = new HtmConfig(new int[] { imgSize, imgSize }, new int[] { 64, 64 })
            {
                PotentialRadius = 10,
                PotentialPct = 1,
                GlobalInhibition = true,
                LocalAreaDensity = -1.0,
                NumActiveColumnsPerInhArea = 0.02 * numOfCols,
                StimulusThreshold = 0.0,
                SynPermInactiveDec = 0.008,
                SynPermActiveInc = 0.05,
                SynPermConnected = 0.10,
                MinPctOverlapDutyCycles = 1.0,
                MinPctActiveDutyCycles = 0.001,
                DutyCyclePeriod = 100,
                MaxBoost = 10.0,
                RandomGenSeed = 42,
                Random = new ThreadSafeRandom(42)

            };
            Connections connections = new Connections(htmConfig);
            HomeostaticPlasticityController hpa = new HomeostaticPlasticityController(connections, trainingImages.Length * 150, (isStable, numPatterns, actColAvg, seenInputs) =>
            {
                isInStableState = true;
                Debug.WriteLine($"Entered STABLE state: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
            });

            SpatialPooler sp = new SpatialPoolerMT(hpa);

            sp.Init(connections);

            string outFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(CreateSdrsTest));
            Directory.CreateDirectory(outFolder);

            while (true)
            {
                counter++;

                Dictionary<string, int[]> sdrs = new Dictionary<string, int[]>();

                Dictionary<string, int[]> inputVectors = new Dictionary<string, int[]>();

                foreach (var trainingImage in trainingImages)
                {
                    FileInfo fI = new FileInfo(trainingImage);

                    string outputHamDistFile = $"{outFolder}\\image-{fI.Name}_hamming.txt";
                    string outputActColFile = $"{outFolder}\\image{fI.Name}_activeCol.txt";
                    string outputActColFile1 = $"{outFolder}\\image{fI.Name}_activeCol.csv";

                    using (StreamWriter swActCol = new StreamWriter(outputActColFile))
                    {
                        using (StreamWriter swActCol1 = new StreamWriter(outputActColFile1))
                        {
                            int[] activeArray = new int[numOfCols];

                            string testName = $"{outFolder}\\{fI.Name}";

                            string inputBinaryImageFile = NeoCortexUtils.BinarizeImage($"{trainingImage}", imgSize, testName);

                            // Read input csv file into array
                            int[] inputVector = NeoCortexUtils.ReadCsvIntegers(inputBinaryImageFile).ToArray();

                            List<double[,]> overlapArrays = new List<double[,]>();
                            List<double[,]> bostArrays = new List<double[,]>();

                            sp.compute(inputVector, activeArray, true);

                            var activeCols = ArrayUtils.IndexWhere(activeArray, (el) => el == 1);

                            if (isInStableState)
                            {
                                CalculateResult(sdrs, inputVectors, numOfCols, activeCols, outFolder, trainingImage, inputVector);

                                overlapArrays.Add(ArrayUtils.Make2DArray<double>(ArrayUtils.ToDoubleArray(connections.Overlaps), colDims[0], colDims[1]));

                                bostArrays.Add(ArrayUtils.Make2DArray<double>(connections.BoostedOverlaps, colDims[0], colDims[1]));

                                var activeStr = Helpers.StringifyVector(activeArray);
                                swActCol.WriteLine("Active Array: " + activeStr);

                                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(activeArray, colDims[0], colDims[1]);
                                twoDimenArray = ArrayUtils.Transpose(twoDimenArray);
                                List<int[,]> arrays = new List<int[,]>();
                                arrays.Add(twoDimenArray);
                                arrays.Add(ArrayUtils.Transpose(ArrayUtils.Make2DArray<int>(inputVector, (int)Math.Sqrt(inputVector.Length), (int)Math.Sqrt(inputVector.Length))));

                                //Calculating the max value of the overlap in the OverlapArray
                                int max = SdrRepresentation.TraceColumnsOverlap(overlapArrays, swActCol1, fI.Name);

                                int red = Convert.ToInt32(max * 0.80);        // Value above this threshould would be red and below this will be yellow 
                                int green = Convert.ToInt32(max * 0.50);      // Value above this threshould would be yellow and below this will be green

                                string outputImage = $"{outFolder}\\cycle-{counter}-{fI.Name}";

                                NeoCortexUtils.DrawBitmaps(arrays, outputImage, Color.Yellow, Color.Gray, OutImgSize, OutImgSize);
                                NeoCortexUtils.DrawHeatmaps(overlapArrays, $"{outputImage}_overlap.png", 1024, 1024, red, red, green);

                                if (sdrs.Count == trainingImages.Length)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CalculateResult(Dictionary<string, int[]> sdrs, Dictionary<string, int[]> inputVectors, int numOfCols, int[] activeCols, string outFolder, string trainingImage, int[] inputVector)
        {
            // Implementation of CalculateResult method
            // This method is responsible for calculating required results and generating bitmaps.
            // You can fill in this method according to your requirements.
            // ...
        }

        [TestMethod]
        public void OverlapArraFunTest()
        {
            // Implementation of OverlapArraFunTest method
            // This method is responsible for testing the OverlapArraFun method.
            // You can fill in this method according to your requirements.
            // ...
        }
        public static void Main(string[] args)
        {
            // Instantiate the SpatialPoolerColumnActivityTest class
            SpatialPoolerColumnActivityTest test = new SpatialPoolerColumnActivityTest();

            // Call the CreateSdrsTest method
            test.CreateSdrsTest();

            // Log end of the program
            Console.WriteLine("Program finished. Press any key to exit...");
            Console.ReadKey();
        }

    }
}
*/