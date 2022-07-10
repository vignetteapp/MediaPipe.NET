// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using CommandLine;
using FFmpeg.AutoGen;
using Mediapipe.Net.External;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Solutions;
using Mediapipe.Net.Util;
using SeeShark;
using SeeShark.Device;
using SeeShark.FFmpeg;

namespace Mediapipe.Net.Examples.PoseGpu
{
    public static class Program
    {
        private static Camera? camera;
        private static FrameConverter? converter;
        private static readonly PoseGpuSolution calculator =
            new PoseGpuSolution(modelComplexity: 2, smoothLandmarks: false);

        private static ResourceManager? resourceManager;

        public static void Main(string[] args)
        {
            // Get and parse command line arguments
            Options parsed = Parser.Default.ParseArguments<Options>(args).Value;

            (int, int)? videoSize = null;
            if (parsed.Width != null && parsed.Height != null)
                videoSize = ((int)parsed.Width, (int)parsed.Height);
            else if (parsed.Width != null && parsed.Height == null)
                Console.Error.WriteLine("Specifying width requires to specify height");
            else if (parsed.Width == null && parsed.Height != null)
                Console.Error.WriteLine("Specifying height requires to specify width");

            FFmpegManager.SetupFFmpeg(@"C:\ffmpeg\v5.0_x64\", @"/usr/lib");
            Glog.Initialize("stuff");
            if (parsed.UseResourceManager)
                resourceManager = new DummyResourceManager();

            // Get a camera device
            using (CameraManager manager = new CameraManager())
            {
                try
                {
                    camera = manager.GetDevice(parsed.CameraIndex,
                        new VideoInputOptions
                        {
                            InputFormat = parsed.InputFormat,
                            Framerate = parsed.Framerate == null ? null : new AVRational
                            {
                                num = (int)parsed.Framerate,
                                den = 1,
                            },
                            VideoSize = videoSize,
                        });
                    Console.WriteLine($"Using camera {camera.Info}");
                }
                catch (Exception)
                {
                    Console.Error.WriteLine($"No camera exists at index {parsed.CameraIndex}.");
                    return;
                }
            }

            camera.OnFrame += onFrameEventHandler;
            camera.StartCapture();

            Console.CancelKeyPress += (sender, eventArgs) => exit();
        }

        private static int frameCount = 0;
        private static void onFrameEventHandler(object? sender, FrameEventArgs e)
        {
            if (calculator == null)
                return;

            Frame frame = e.Frame;
            if (frame.Width == 0 || frame.Height == 0)
                return;

            converter ??= new FrameConverter(frame, PixelFormat.Rgba);
            Frame cFrame = converter.Convert(frame);

            ImageFrame imgframe = new ImageFrame(ImageFormat.Srgba,
                cFrame.Width, cFrame.Height, cFrame.WidthStep, cFrame.RawData);

            PoseOutput handsOutput = calculator.Compute(imgframe);

            if (handsOutput.PoseLandmarks != null)
            {
                var landmarks = handsOutput.PoseLandmarks.Landmark;
                Console.WriteLine($"Got pose output with {landmarks.Count} landmarks"
                    + $" at frame {frameCount}");
            }
            else
            {
                Console.WriteLine("No pose landmarks");
            }

            frameCount++;
        }

        // Dispose everything on exit
        private static void exit()
        {
            Console.WriteLine("Exiting...");
            camera?.Dispose();
            converter?.Dispose();
            calculator?.Dispose();
        }
    }
}
