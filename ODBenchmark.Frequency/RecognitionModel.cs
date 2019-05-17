using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODBenchmark.Frequency
{
    public class RecognitionModelHistogram
    {
        private float[,] _modelPattern;
        private bool[,] _modelPatternTresholded;
        private List<IntPoint> _pattern;
        private List<IntPoint> _patternVectorised;
        private bool _patternCenter;
        private List<IntPoint>[] _patternDistances;
        private int[] _patternCounts;
        private int _patternNonZero;
        private int _angleDivisions = 20;
        private float _modelGPSLatitude = 0.0f;
        private float _modelGPSLongitude = 0.0f;
        private float _modelWorldModelRotation = 0.0f;        
        private float _modelWorldModelVerticalTilt = 0.0f;
        private float _modelTilt = 0.0f;

        public int Size { get; set; }
        protected List<IntPoint>[,] _data;
        protected int _resolutionX;
        protected int _resolutionY;
        

        protected List<IntPoint> _recognitionPatern;

        public RecognitionModelHistogram(int size, int resolutionY, int resolutionX)
        {
            Size = size;
            _data = new List<IntPoint>[Size, Size];
            _resolutionY = resolutionY;
            _resolutionX = resolutionX;
            _modelPattern = new float[Size, Size];
            _modelPatternTresholded = new bool[Size, Size];
            _patternVectorised = new List<IntPoint>();
            _pattern = new List<IntPoint>();
            _patternDistances = new List<IntPoint>[_angleDivisions];
            _patternCounts = new int[_angleDivisions];
            _patternNonZero = 0;
            _patternCenter = false;
            for (int i = 0; i < _angleDivisions; i++)
            {
                _patternDistances[i] = new List<IntPoint>();
            }
        }

        public void CalculatePatern()
        {
            FillPatternAndThreashold(_modelPattern, _modelPatternTresholded, _data);
            _pattern = ClusterPattern(_modelPatternTresholded);
            VectorisePoints();
            FillPatternAngles();
        }

        private void VectorisePoints()
        {
            var centerPoint = new IntPoint((Size + 1) / 2, (Size + 1) / 2);
            _patternVectorised.Clear();
            foreach (var point in _pattern)
            {
                _patternVectorised.Add(centerPoint.CalculateMoveVector(point));
            }
        }

        private void FillPatternAngles()
        {
            for (int i = 0; i < _angleDivisions; i++)
            {
                if (_patternDistances[i] == null)
                    _patternDistances[i] = new List<IntPoint>();
                _patternDistances[i].Clear();
            }
            foreach (var vec in _patternVectorised)
            {
                if ((vec.X == 0) && (vec.Y == 0))
                {
                    _patternCenter = true;
                    continue;
                }
                var angleIndex = (int)(Math.Round(vec.VectorNormalized() * _angleDivisions / 2.0f) % _angleDivisions);

                _patternDistances[angleIndex].Add(vec);
            }
            _patternNonZero = 0;
            for (int i = 0; i < _angleDivisions; i++)
            {
                _patternCounts[i] = _patternDistances[i].Count;
                if (_patternCounts[i] != 0)
                    _patternNonZero++;
            }
        }

        public int IsViableForImageRecognition(float worldModelRotation, float worldModelVerticalTilt, float GPSLatitude, float GPSLongitude)
        {
            if((Math.Abs(GPSLatitude - _modelGPSLatitude) < 0.0005) && (Math.Abs(GPSLongitude - _modelGPSLongitude) < 0.001))
            {
                if ((Math.Abs(worldModelRotation - _modelWorldModelRotation) < 0.2) && (Math.Abs(worldModelVerticalTilt - _modelWorldModelVerticalTilt) < 0.2))
                {
                    return 0;
                }
                return 1;
            }
            return 2;
        }

        public RecognisedModel Recognise(List<IntPoint>[,] model, float accuracy, float tilt)
        {
            if (_patternNonZero == 0)
                return new RecognisedModel();
            var pattern = new float[Size, Size];
            var threashold = new bool[Size, Size];
            FillPatternAndThreashold(pattern, threashold, model);
            var pat = ClusterPattern(threashold);
            return ComparePattern(pat, tilt, accuracy);
        }

        /// <summary>
        /// Create array of pattern from data and make bool array where cell holds more than a 0.5 of maximum cell
        /// </summary>
        /// <param name="pattern">Return pattern array</param>
        /// <param name="threashold">Return threshold array</param>
        /// <param name="data">Input data</param>
        private void FillPatternAndThreashold(float[,] pattern, bool[,] threashold, List<IntPoint>[,] data)
        {
            int maxNumberOfElements = 0;
            foreach (var points in data)
            {
                if (points.Count > maxNumberOfElements)
                    maxNumberOfElements = points.Count;
            }
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    pattern[y, x] = (float)data[y, x].Count / maxNumberOfElements;
                    if (pattern[y, x] > 0.5)
                        threashold[y, x] = true;
                    else
                        threashold[y, x] = false;
                }
            }
        }

        /// <summary>
        /// Change form [Size,Size] bool array into list of IntPoints for values == true in array
        /// </summary>
        /// <param name="threashold"></param>
        /// <returns></returns>
        private List<IntPoint> ClusterPattern(bool[,] threashold)
        {
            List<IntPoint> clusters = new List<IntPoint>();

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (threashold[y, x])
                        clusters.Add(new IntPoint(y, x));
                }
            }
            return clusters;
        }

        private RecognisedModel ComparePattern(List<IntPoint> samples, float tilt, float accuracy)
        {
            var recognition = new RecognisedModel();
            float bestAccuracy = 0.0f;
            //To offset possible center from edge of picture
            for (var y = 5; y < (Size - 5); y++)
            {
                for (var x = 5; x < (Size - 5); x++)
                {
                    int[,] votingTable = new int[_angleDivisions, 5];
                    bool centerVote = false;
                    IntPoint centerPoint = new IntPoint(y, x);
                    List<IntPoint> sampleVectors = new List<IntPoint>();
                    //Calculate move vectors
                    foreach (var sample in samples)
                    {
                        sampleVectors.Add(centerPoint.CalculateMoveVector(sample));
                    }
                    foreach (var sampleVec in sampleVectors)
                    {
                        bool[] voted = new bool[5] { false, false, false, false, false };
                        //If it's a center piece
                        if ((sampleVec.X == 0) && (sampleVec.Y == 0))
                        {
                            centerVote = true;
                            continue;
                        }
                        //Calculate angle index so we can know to which vectors should we compare our vector
                        var normalized = sampleVec.VectorNormalized();
                        var angleIndex = (int)(Math.Round(((normalized + (tilt - _modelTilt)) * _angleDivisions / 2.0f)) % _angleDivisions);
                        angleIndex = (angleIndex + _angleDivisions) % _angleDivisions;

                        //Fill possible distances of our vector in comparision to other distances found in model with similar angle from center
                        foreach (var mod in _patternDistances[angleIndex])
                        {
                            float angleModel = mod.VectorNormalized();
                            var currentScale = mod.CalculateScaleFromMoveVectors(sampleVec);
                            var scale = (int)Math.Round((currentScale - 0.8f) * 10);//We want to find scales in 0.8 to 1.2 and index them from 0 to 4
                            if ((scale > 4) || (scale < 0))
                                continue;
                            if (voted[scale])
                                continue;
                            else
                                voted[scale] = true;
                            votingTable[angleIndex, scale]++;
                        }
                    }
                    //If center in model and center in calculated pattern is the same
                    if (centerVote == _patternCenter)
                        centerVote = true;
                    else
                        centerVote = false;
                    for (int scaleIndex = 0; scaleIndex < 5; scaleIndex++)
                    {
                        float accurateMatches = 0.0f;
                        for (int i = 0; i < _angleDivisions; i++)
                        {

                            var tmp = Math.Abs(votingTable[i, scaleIndex] - _patternCounts[i]);
                            if (tmp < 1)
                            {
                                accurateMatches++;
                            }
                            else
                                accurateMatches -= tmp / 3.0f;

                        }
                        accurateMatches = (float)(accurateMatches + (centerVote? 1 : 0)) / (_angleDivisions + 1.0f);
                        if (bestAccuracy < accurateMatches)
                        {
                            bestAccuracy = accurateMatches;
                            recognition.ModelFound = bestAccuracy > accuracy;
                            int objectR = (int)(((Size + 1) / 2.0f) * ((scaleIndex + 8.1f) / 10.0f));
                            recognition.RecognitionProb = bestAccuracy;
                            recognition.StartOfFoundModel = new IntPoint(y - objectR, x - objectR);
                            recognition.EndOfFoundModel = new IntPoint(y + objectR - 1, x + objectR - 1);
                        }
                    }
                }
            }
            return recognition;
        }

        public void SetModel(List<IntPoint>[,] model, float tilt, float worldModelRotation, float worldModelVerticalTilt, float GPSLatitude, float GPSLongitude)
        {
            _data = model;
            _modelTilt = tilt;
            _modelGPSLatitude = GPSLatitude;
            _modelGPSLongitude = GPSLongitude;
            _modelWorldModelRotation = worldModelRotation;
            _modelWorldModelVerticalTilt = worldModelVerticalTilt;
        }
    }
    public class RecognisedModel
    {
        public IntPoint StartOfFoundModel { get; set; }
        public IntPoint EndOfFoundModel { get; set; }
        public bool ModelFound { get; set; }
        public float RecognitionProb { get; set; }

        public RecognisedModel()
        {
            StartOfFoundModel = new IntPoint(0, 0);
            EndOfFoundModel = new IntPoint(0, 0);
            ModelFound = false;
            RecognitionProb = 0.0f;
        }

        public RecognisedModel(IntPoint start, IntPoint end, bool found, float prob)
        {
            StartOfFoundModel = start;
            EndOfFoundModel = end;
            ModelFound = found;
            RecognitionProb = prob;
        }
    }
}
