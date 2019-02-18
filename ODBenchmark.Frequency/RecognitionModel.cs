using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODBenchmark.Frequency
{
    public abstract class RecognitionModelBase
    {
        public int Size { get; set; }
        protected List<IntPoint>[,] _data;
        protected int _resolutionX;
        protected int _resolutionY;
        protected float _modelTilt;

        protected List<IntPoint> _recognitionPatern;

        public RecognitionModelBase(int size, int resolutionY, int resolutionX)
        {
            Size = size;
            _data = new List<IntPoint>[Size, Size];
            _resolutionY = resolutionY;
            _resolutionX = resolutionX;
        }

        public void SetModel(List<IntPoint>[,] model, float modelTilt)
        {
            _data = model;
            _modelTilt = modelTilt;
        }

        public abstract void CalculatePatern();

        public abstract RecognisedModel Recognise(List<IntPoint>[,] model, int broadSearchItems, float accuracy, float tilt);
    }

    public class RecognitionModelHistogram : RecognitionModelBase
    {
        private float[,] _modelPattern;
        private bool[,] _modelPatternTresholded;
        private List<IntPoint> _pattern;
        private List<IntPoint> _patternVectorised;
        private List<IntPoint>[] _patternDistances;
        private int[] _patternCounts;
        private int _patternNonZero;

        public RecognitionModelHistogram(int size, int resolutionY, int resolutionX) : base(size, resolutionY, resolutionX)
        {
            _modelPattern = new float[Size, Size];
            _modelPatternTresholded = new bool[Size, Size];
            _patternVectorised = new List<IntPoint>();
            _pattern = new List<IntPoint>();
            _patternDistances = new List<IntPoint>[20];
            _patternCounts = new int[20];
            _patternNonZero = 0;
            for (int i = 0; i < 20; i++)
            {
                _patternDistances[i] = new List<IntPoint>();
            }
        }

        public override void CalculatePatern()
        {
            FillPatternAndThreashold(_modelPattern, _modelPatternTresholded, _data);
            _pattern = ClusterPattern(_modelPatternTresholded);
            VectorisePoints();
            FillPatternAngles();
        }

        private void VectorisePoints()
        {
            var centerPoint = new IntPoint(Size / 2, Size / 2);
            _patternVectorised.Clear();
            foreach (var point in _pattern)
            {
                _patternVectorised.Add(centerPoint.CalculateMoveVector(point));
            }
        }

        private void FillPatternAngles()
        {
            for (int i = 0; i < 20; i++)
            {
                if (_patternDistances[i] == null)
                    _patternDistances[i] = new List<IntPoint>();
                _patternDistances[i].Clear();
            }
            foreach (var vec in _patternVectorised)
            {
                var angleIndex = (int)(vec.VectorNormalized() * 10) % 20;

                _patternDistances[angleIndex].Add(vec);
            }
            _patternNonZero = 0;
            for (int i = 0; i < 20; i++)
            {
                _patternCounts[i] = _patternDistances[i].Count;
                if (_patternCounts[i] != 0)
                    _patternNonZero++;
            }
        }

        public override RecognisedModel Recognise(List<IntPoint>[,] model, int broadSearchItems, float accuracy, float tilt)
        {
            if (_patternNonZero == 0)
                return new RecognisedModel();
            var pattern = new float[Size, Size];
            var threashold = new bool[Size, Size];
            FillPatternAndThreashold(pattern, threashold, model);
            var pat = ClusterPattern(threashold);
            return ComparePattern(pat, tilt, accuracy);

        }

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
            for (var y = 5; y < (Size - 5); y++)
            {
                for (var x = 5; x < (Size - 5); x++)
                {
                    int[,] votingTable = new int[20, 10];
                    IntPoint centerPoint = new IntPoint(y, x);
                    List<IntPoint> sampleVectors = new List<IntPoint>();
                    foreach (var sample in samples)
                    {
                        sampleVectors.Add(centerPoint.CalculateMoveVector(sample));
                    }
                    foreach (var sampleVec in sampleVectors)
                    {
                        var angleVector = sampleVec.VectorNormalized() - (tilt - _modelTilt);
                        if (angleVector < 0)
                            angleVector += 2.0f;
                        angleVector = angleVector % 2.0f;
                        var angleIndex = (int)(angleVector * 10.0f);

                        foreach (var mod in _patternDistances[angleIndex])
                        {
                            float angleModel = mod.VectorNormalized();
                            var currentScale = mod.CalculateScaleFromMoveVectors(sampleVec);
                            var scale = (int)((currentScale - 0.5f) * 10);//We want to find scales in 0.5 to 1.5 and index them from 0 to 9
                            if ((scale > 9) || (scale < 0))
                                continue;
                            votingTable[angleIndex, scale]++;
                        }
                    }
                    for (int scaleIndex = 0; scaleIndex < 10; scaleIndex++)
                    {
                        float accurateMatches = 0.0f;
                        for (int i = 0; i < 20; i++)
                        {

                            var tmp = Math.Abs(votingTable[i, scaleIndex] - _patternCounts[i]);
                            if (tmp < 1)
                            {
                                accurateMatches++;
                            }
                            else
                                accurateMatches -= tmp / 3.0f;

                        }
                        accurateMatches = accurateMatches / 20.0f;
                        if (bestAccuracy < accurateMatches)
                        {
                            bestAccuracy = accurateMatches;
                            recognition.ModelFound = bestAccuracy > accuracy;
                            int objectR = (int)((Size / 2.0f) * (scaleIndex + 5.1f) / 10.0f);
                            recognition.RecognitionProb = bestAccuracy;
                            recognition.StartOfFoundModel = new IntPoint(y - objectR, x - objectR);
                            recognition.EndOfFoundModel = new IntPoint(y + objectR, x + objectR);
                        }
                    }
                }
            }
            return recognition;
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
