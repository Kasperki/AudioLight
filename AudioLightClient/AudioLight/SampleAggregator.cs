using NAudio.Dsp;
using System;

namespace AudioLight
{
    class SampleAggregator
    {
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        private float maxValue;
        private float minValue;
        int count;

        // FFT
        public event EventHandler<FftEventArgs> FftCalculated;
        public bool PerformFFT { get; set; }
        private static int fftLength = 1024;
        private int fftPos;
        private Complex[] fftBuffer = new Complex[fftLength];
        private float[] fftOutputBuffer = new float[fftLength];

        public void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }

        public void Add(float value)
        {
            if (PerformFFT && FftCalculated != null)
            {
                fftBuffer[fftPos].X = value * (float)FastFourierTransform.HammingWindow(fftPos, fftLength);
                fftBuffer[fftPos].Y = 0;

                fftPos++;
                if (fftPos >= fftLength)
                {
                    fftPos = 0;
                    // 1024 = 2^10
                    FastFourierTransform.FFT(true, 10, fftBuffer);

                    //intesities
                    for (int i = 0; i < fftLength / 2; i += 2)
                    {
                        // Calculate actual intensities for the FFT results.
                        fftOutputBuffer[i] = (float)Math.Sqrt(fftBuffer[i].X * fftBuffer[i].X + fftBuffer[i].Y * fftBuffer[i].Y);
                        //fftOutputBuffer[i] = fftBuffer[i].X;    //Real
                        fftOutputBuffer[i + 1] = fftBuffer[i].Y; //Complex
                    }

                    FftCalculated(this, new FftEventArgs(fftOutputBuffer));
                }
            }

            maxValue = Math.Max(maxValue, value);
            minValue = Math.Min(minValue, value);

            count++;
            if (count >= fftLength)
            {
                MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(minValue, maxValue));
                Reset();
            }
        }
    }

    public class MaxSampleEventArgs : EventArgs
    {
        public MaxSampleEventArgs(float minValue, float maxValue)
        {
            this.MaxSample = maxValue;
            this.MinSample = minValue;
        }
        public float MaxSample { get; private set; }
        public float MinSample { get; private set; }
    }

    public class FftEventArgs : EventArgs
    {
        public FftEventArgs(float[] result)
        {
            this.Result = result;
        }
        public float[] Result { get; private set; }
    }
}
