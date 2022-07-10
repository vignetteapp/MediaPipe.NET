// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using CommandLine;
using FFmpeg.AutoGen;
using Mediapipe.Net.External;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using SeeShark;
using SeeShark.Device;
using SeeShark.FFmpeg;

namespace Mediapipe.Net.Examples.FaceMeshGpu
{
    [SupportedOSPlatform("Linux")]
    public static class Program
    {
        private static Camera? camera;
        private static FrameConverter? converter;
        private static FaceMeshGpuSolution? calculator;

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

            FFmpegManager.SetupFFmpeg("/usr/lib");
            Glog.Initialize("stuff");

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
                catch (Exception e)
                {
                    Console.Error.WriteLine($"An error occured while trying to use camera at index {parsed.CameraIndex}.");
                    Console.Error.WriteLine(e);
                    return;
                }
            }

            calculator = new FaceMeshGpuSolution();

            Console.CancelKeyPress += (sender, eventArgs) => exit();
            int frameCount = 0;
            while (true)
            {
                if (calculator == null)
                    return;

                Frame frame = camera.GetFrame();
                converter ??= new FrameConverter(frame, PixelFormat.Rgba);
                Frame cFrame = converter.Convert(frame);

                using ImageFrame imgframe = new ImageFrame(ImageFormat.Srgba,
                    cFrame.Width, cFrame.Height, cFrame.WidthStep, cFrame.RawData);

                List<NormalizedLandmarkList>? landmarks = calculator.Compute(imgframe);
                if (landmarks == null)
                    Console.WriteLine("Got null landmarks");
                else
                    Console.WriteLine($"Got a list of {landmarks[0].Landmark.Count} landmarks at frame {frameCount++}");
            }
        }

        // Dispose everything on exit
        private static void exit()
        {
            Console.WriteLine("Exiting...");
            camera?.StopCapture();
            camera?.Dispose();
            converter?.Dispose();
            calculator?.Dispose();
        }
    }
}
