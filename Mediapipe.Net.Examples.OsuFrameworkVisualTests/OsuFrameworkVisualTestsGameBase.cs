// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using CommandLine;
using FFmpeg.AutoGen;
using Mediapipe.Net.Solutions;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using SeeShark;
using SeeShark.Device;

namespace Mediapipe.Net.Examples.OsuFrameworkVisualTests
{
    public class OsuFrameworkVisualTestsGameBase : Game
    {
        private Camera? camera;
        private FrameConverter? converter;
        private FaceMeshCpuSolution? calculator;

        protected override Container<Drawable> Content { get; }

        private DependencyContainer? dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        protected OsuFrameworkVisualTestsGameBase()
        {
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1366, 768),
            });
        }

#pragma warning disable IDE0051
        [BackgroundDependencyLoader]
        private void load()
        {
            string[] args = Environment.GetCommandLineArgs();
            Options parsed = Parser.Default.ParseArguments<Options>(args).Value;

            (int, int)? videoSize = null;
            if (parsed.Width != null && parsed.Height != null)
                videoSize = ((int)parsed.Width, (int)parsed.Height);
            else if (parsed.Width != null && parsed.Height == null)
                Console.Error.WriteLine("Specifying width requires to specify height");
            else if (parsed.Width == null && parsed.Height != null)
                Console.Error.WriteLine("Specifying height requires to specify width");

            var manager = new CameraManager();
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
            dependencies?.Cache(camera);
            manager.Dispose();

            var dummyFrame = camera.GetFrame();
            converter = new FrameConverter(dummyFrame, dummyFrame.Width / 2, dummyFrame.Height / 2, PixelFormat.Rgba);
            dependencies?.Cache(converter);

            calculator = new FaceMeshCpuSolution();
            dependencies?.Cache(calculator);
        }
#pragma warning restore IDE0051

        protected override bool OnExiting()
        {
            calculator?.Dispose();
            converter?.Dispose();
            camera?.Dispose();
            return base.OnExiting();
        }
    }
}
