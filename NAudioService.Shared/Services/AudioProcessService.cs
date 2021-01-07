using NAudio.Wave;
using NAudioService.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioService.Shared.Services
{
    public static class AudioProcessService
    {
        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="audioObjects"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public static bool Combine(this AudioObject[] audioObjects, string destinationPath, Action<string> msgAction = null)
        {
            try
            {
                var fistData = audioObjects.First(item => item != null);
                using WaveFileWriter waveFileWriter = new WaveFileWriter(destinationPath, fistData.AudioReader.WaveFormat);
                foreach (var ao in audioObjects)
                {
                    if (ao == null)
                    {
                        continue;
                    }
                    int bytesPerMillisecond = ao.AudioReader.WaveFormat.AverageBytesPerSecond / 1000;

                    int startPos = (int)ao.SelectionInterval.Item1.TotalMilliseconds * bytesPerMillisecond;
                    startPos -= startPos % ao.AudioReader.WaveFormat.BlockAlign;

                    int endPos = (int)ao.SelectionInterval.Item2.TotalMilliseconds * bytesPerMillisecond;
                    endPos -= endPos % ao.AudioReader.WaveFormat.BlockAlign;
                    byte[] audioData = new byte[endPos - startPos];
                    ao.AudioReader.Position = startPos;
                    ao.AudioReader.Read(audioData, 0, endPos - startPos);
                    //byte[] reAudioData = ResampleNaive(audioData, ao.AudioReader.WaveFormat.SampleRate, waveFileWriter.WaveFormat.SampleRate);
                    //using RawSourceWaveStream rsws = new RawSourceWaveStream(reAudioData, 0, reAudioData.Length, waveFileWriter.WaveFormat);
                    //byte[] resultData = new byte[reAudioData.Length];
                    //rsws.Read(resultData, 0, resultData.Length);
                    //waveFileWriter.Write(resultData, 0, resultData.Length);
                    waveFileWriter.Write(audioData, 0, audioData.Length);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 重采样
        /// </summary>
        /// <param name="inBuffer"></param>
        /// <param name="inputSampleRate"></param>
        /// <param name="outputSampleRate"></param>
        /// <returns></returns>
        public static byte[] ResampleNaive(byte[] inBuffer, int inputSampleRate, int outputSampleRate)
        {
            var outBuffer = new List<byte>();
            double ratio = (double)inputSampleRate / outputSampleRate;
            int outSample = 0;
            while (true)
            {
                int inBufferIndex = (int)(outSample++ * ratio);
                if (inBufferIndex < inBuffer.Length)
                    outBuffer.Add(inBuffer[inBufferIndex]);
                else
                    break;
            }
            return outBuffer.ToArray();
        }
    }
}
