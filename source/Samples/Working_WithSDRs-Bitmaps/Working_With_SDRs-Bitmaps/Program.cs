using NeoCortex;
using NeoCortexApi.Encoders;
using NeoCortexApi.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace SdrRepresentation
{
    public class SdrEncodingLibrary
    {
        /*This code defines a program that generates Sparse Distributed Representations (SDRs) using scalar encoding.

        Input: The user is prompted to enter two double values, which serve as input for the scalar encoding experiments.

        Output:
        For each input value, a bitmap image is generated visualizing the Sparse Distributed Representation (SDR) encoded using scalar encoding. 
        The bitmap image is saved as a PNG file in a folder named "ScalarEncodingExperiment". 
        The image shows the SDR as a grid of yellow and black pixels, where yellow pixels represent active bits and black pixels represent inactive bits. 
        The input value is also displayed as text on the image.
        */
        public void RunScalarEncodingExperiment(double input)
        {
            string outFolder = "ScalarEncodingExperiment";
            Directory.CreateDirectory(outFolder);

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



        /*
        This program generates a Sparse Distributed Representation (SDR) using DateTime encoding. 

        Input: The user is prompted to enter an input value in DateTime format.

        Output:
        A bitmap image is generated visualizing the Sparse Distributed Representation (SDR) encoded using DateTime encoding.
        The image is saved as a PNG file with a filename indicating the input DateTime value and encoder settings.
        */

        public void RunDateTimeEncodingExperiment(string input)
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
            int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result, 32, 32);
            int[,] twoDimArray = ArrayUtils.Transpose(twoDimenArray);
            NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"DateTime_out_{input.Replace("/", "-").Replace(":", "-")}_32x32-N-{encoderSettings["DateTimeEncoder"]["N"]}-W-{encoderSettings["DateTimeEncoder"]["W"]}.png");
        }



        /*
        This C# program is for the testing of GeoSpatialEncoderExperimental class.
        It tests the encoding of geographical spatial data and generates bitmap images to visualize the encoded representations.

        Input: The latitude value (48.0) is used for testing the encoding of geographical spatial data.

        Output:Bitmap images are generated for each latitude value within the specified range, visualizing the encoded spatial data.
        The images are saved in a folder named after the test method (GeoSpatialEncoderTestDrawBitMap).
        Each image filename includes the corresponding latitude value.
*/
        public void RunGeoSpatialEncodingExperiment(double input)
        {
            string outFolder = "GeoSpatialEncoderTestDrawBitMap";

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
            PrintImage(encoder, outFolder);
        }

        private void PrintImage(GeoSpatialEncoderExperimental encoder, string folderName)
        {
            Directory.CreateDirectory(folderName);
            for (double j = (long)encoder.MinVal; j < (long)encoder.MaxVal; j += 1)
            {
                var result2 = encoder.Encode(j);
                int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(result2, (int)Math.Sqrt(result2.Length), (int)Math.Sqrt(result2.Length));
                var twoDimArray = ArrayUtils.Transpose(twoDimenArray);
                NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{folderName}\\{j}.png", Color.Red, Color.Green, text: j.ToString());
            }
        }



        /*
        This code is to test the spatial pooler's functionality by processing input images and generating 
        visual representations (bitmap images) of the active columns and overlap heatmaps produced by the spatial pooler.

        Input:
        The input images are located in the trainingFolder directory. 
        These images are used as input data for testing the spatial pooler's functionality.

        Output:
        Bitmap Images: Bitmap images representing the active columns and overlap heatmaps are generated for each input image. 
        These images are saved in the nameof(CreateSdrsTest) directory.

        Text Files: Text files containing information about active columns and other statistics may be generated during the test process. 
        These files are saved alongside the bitmap images in the nameof(CreateSdrsTest) directory.

        Console Output: Debug information, such as messages indicating the program's progress or any errors encountered during the execution, may be displayed in the console.
        */
        
        /*
        [TestClass]
        public class SpatialPoolerColumnActivityTest
        {
            private const int OutImgSize = 1024;

            [TestMethod]
            public void CreateSdrsTest()
            {
                var colDims = new int[] { 64, 64 };
                int numOfCols = 64 * 64;

                string trainingFolder = @"..\..\..\TestFiles\Sdr";

                int imgSize = 28;

                var trainingImages = Directory.GetFiles(trainingFolder, "*.jpeg");

                Directory.CreateDirectory($"{nameof(CreateSdrsTest)}");

                int counter = 0;

                bool isInStableState = false;

                // HTM parameters
                HtmConfig htmConfig = new HtmConfig(new int[] { imgSize, imgSize }, colDims)
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

                HomeostaticPlasticityController hpa = new HomeostaticPlasticityController(connections, trainingImages.Length * 50, (isStable, numPatterns, actColAvg, seenInputs) =>
                {
                    isInStableState = true;
                    Debug.WriteLine($"Entered STABLE state: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
                });

                SpatialPooler sp = new SpatialPoolerMT(hpa);

                sp.Init(connections);

                string outFolder = nameof(CreateSdrsTest);
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


            // <summary>
            /// Calculate all required results.
            /// 1. Overlap and Union of the Spatial Pooler SDRs of two Images as Input
            ///    It cross compares the 1st SDR with it self and all the Tranings Images.
            /// 2. Creates bitmaps of the overlaping and non-overlaping regions of the Comparing SDRs.
            /// 3. Also generate HeatMaps of the SDRs during Spatial Pooler learning Phase.
            /// </summary>
            /// <param name="sdrs"></param>
            private void CalculateResult(Dictionary<string, int[]> sdrs, Dictionary<string, int[]> inputVectors, int numOfCols, int[] activeCols, string outFolder, string trainingImage, int[] inputVector)
            {
                int[] CompareArray = new int[numOfCols];
                int[] ActiveArray = new int[numOfCols];

                ActiveArray = SdrRepresentation.GetIntArray(activeCols, 4096);

                sdrs.Add(trainingImage, activeCols);
                inputVectors.Add(trainingImage, inputVector);
                int[] FirstSDRArray = new int[81];
                if (sdrs.First().Key == null)
                {
                    FirstSDRArray = new int[sdrs.First().Value.Length];

                }

                FirstSDRArray = sdrs.First().Value;

                CompareArray = SdrRepresentation.GetIntArray(FirstSDRArray, 4096);

                var Array = SdrRepresentation.OverlapArraFun(ActiveArray, CompareArray);
                int[,] twoDimenArray2 = ArrayUtils.Make2DArray<int>(Array, (int)Math.Sqrt(Array.Length), (int)Math.Sqrt(Array.Length));
                int[,] twoDimArray1 = ArrayUtils.Transpose(twoDimenArray2);
                NeoCortexUtils.DrawBitmap(twoDimArray1, 1024, 1024, $"{outFolder}\\Overlap_{sdrs.Count}.png", Color.PaleGreen, Color.Red, text: $"Overlap.png");

                Array = ActiveArray.Union(CompareArray).ToArray();
                int[,] twoDimenArray4 = ArrayUtils.Make2DArray<int>(Array, (int)Math.Sqrt(Array.Length), (int)Math.Sqrt(Array.Length));
                int[,] twoDimArray3 = ArrayUtils.Transpose(twoDimenArray4);
                NeoCortexUtils.DrawBitmap(twoDimArray3, 1024, 1024, $"{outFolder}\\Union_{sdrs.Count}.png", Color.PaleGreen, Color.Green, text: $"Union.png");

                // Bitmap Intersection Image of two bit arrays selected for comparison
                SdrRepresentation.DrawIntersections(twoDimArray3, twoDimArray1, 10, $"{outFolder}\\Intersection_{sdrs.Count}.png", Color.Black, Color.Gray, text: $"Intersection.png");

                return;
            }

            /// <summary>
            /// Vaildate method <see cref="SdrRepresentation.OverlapArraFun(int[], int[])"/>
            /// </summary>
            [TestMethod]
            public void OverlapArraFunTest()
            {
                int[] a1 = new int[] { 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1 };
                int[] a2 = new int[] { 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0 };

                Assert.ThrowsException<IndexOutOfRangeException>(() => SdrRepresentation.OverlapArraFun(a1, a2));
                var res = SdrRepresentation.OverlapArraFun(a2, a1);

                Assert.IsNotNull(res);
            }



        }
        */


        S

    }

}
