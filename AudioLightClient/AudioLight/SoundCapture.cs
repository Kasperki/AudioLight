using NAudio.Wave;
using System;

namespace AudioLight
{
    public class SoundCapture
    {
        private SampleAggregator sampleAggregator;
        private WasapiLoopbackCapture waveOut;
        public float[] spectrumHeight;
        public float[] spectrumWidth;

        public Color color;

        public event EventHandler<EventArgs> ColorCalculated;

        public SoundCapture()
        {
            waveOut = new WasapiLoopbackCapture();
            waveOut.StartRecording();
            waveOut.DataAvailable += waveOut_DataAvailable;

            sampleAggregator = new SampleAggregator();
            sampleAggregator.PerformFFT = true;
            sampleAggregator.MaximumCalculated += new EventHandler<MaxSampleEventArgs>(MaximumCalculated);
            sampleAggregator.FftCalculated += new EventHandler<FftEventArgs>(FftCalculated);

            spectrumHeight = new float[100];
            spectrumWidth = new float[100];

            color = new Color();
        }

        public void StopSoundCapture()
        {
            waveOut.StopRecording();
        }

        //Reads audio data, when available
        private void waveOut_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;


            int bufferIncrement = (int)(waveOut.WaveFormat.BlockAlign / waveOut.WaveFormat.Channels);

            for (int index = 0; index < e.BytesRecorded; index += bufferIncrement)
            {

                float sample32 = 0;

                if (waveOut.WaveFormat.BitsPerSample <= 16) // Presume 16-bit PCM WAV
                {
                    short sample16 = (short)((buffer[index + 1] << 8) | buffer[index + 0]);
                    sample32 = sample16 / 32768f;
                }
                else if (waveOut.WaveFormat.BitsPerSample <= 32) // Presume 32-bit IEEE Float WAV - Default
                {
                    sample32 = BitConverter.ToSingle(buffer, index);
                }

                //FFT
                sampleAggregator.Add(sample32);
            }
        }

        public void MaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            //Console.WriteLine("Maximum Calculated");
        }

        //Is called when FFT is calculated
        public void FftCalculated(object sender, FftEventArgs e)
        {
            Spectrum(44100, 2024, e.Result);
            CalculateColors();
            ColorCalculated(this, null);
        }

        //Gets the audio spectrum D:<
        public float Spectrum(float sampleRate, float inFrames, float[] fftBuffer)
        {
            float binSize = sampleRate / inFrames;
            int minBin = (int)(20 / binSize);
            int maxBin = (int)(450 / binSize);

            float maxIntensity = 0f;
            int maxBinIndex = 0;

            for (int bin = minBin; bin <= maxBin; bin++)
            {
                float real = fftBuffer[bin * 2];
                float imaginary = fftBuffer[bin * 2 + 1];
                float intensity = real * real + imaginary * imaginary;
                float magnitude = (float)Math.Sqrt(intensity);

                //Spectrum height! This is the magic
                spectrumHeight[bin] = (int)(magnitude * 1750);
                spectrumWidth[bin] = (int)(20 * Math.Log10(intensity));
                //SpecRect[bin].Height = (int)fftBuffer[bin];

                //Get the highest frequency
                if (intensity > maxIntensity)
                {
                    maxIntensity = intensity;
                    maxBinIndex = bin;
                }
            }

            //returns the highest frecuency
            return binSize * maxBinIndex;
        }

        //Based on Spectrum
        private void CalculateColors()
        {
            color.R -= 75;
            color.R += spectrumHeight[0];
            color.R += spectrumHeight[1];

            color.G -= 150;
            for (int i = 2; i < 6; i++)
            {
                color.G += spectrumHeight[i];
            }

            color.B -= 150;
            for (int i = 6; i < 20; i++)
            {
                color.B += spectrumHeight[i];
            }
        }

        public string GetColorString()
        {
            return color.GetJson();
        }
    }

    public class Color
    {
        private float r;
        public float R
        {
            get
            {
                return r;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > 255)
                    value = 255;

                r = value;
            }
        }

        private float g;
        public float G
        {
            get
            {
                return g;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > 255)
                    value = 255;

                g = value;
            }
        }

        private float b;
        public float B
        {
            get
            {
                return b;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > 255)
                    value = 255;

                b = value;
            }
        }

        private float a = 255;
        public float A
        {
            get
            {
                return a;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > 255)
                    value = 255;

                a = value;
            }
        }

        public string GetJson()
        {
            return "{\"r\":" + r + ",\"g\":" + g + ",\"b\":" + b + "}";
        }

        public System.Drawing.Color GetSystemColor()
        {
            return System.Drawing.Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }
    }
}
