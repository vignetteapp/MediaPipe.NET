// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf.Collections;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using SeeShark;
using SeeShark.Device;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Mediapipe.Net.Examples.OsuFrameworkVisualTests
{
    public class MediapipeDrawable : CompositeDrawable
    {
        private Camera? camera;
        private FrameConverter? converter;
        private FaceMeshCpuSolution? calculator;

        private Circle[]? circles;
        private readonly Sprite sprite;
        private Texture? texture;

        private Thread? frameLoopThread;

        public MediapipeDrawable()
        {
            Masking = true;
            CornerRadius = 10;
            AddInternal(sprite = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fit,
                FillAspectRatio = 1,
            });
        }

#pragma warning disable IDE0051
        [BackgroundDependencyLoader]
        private void load(Camera camera, FrameConverter converter, FaceMeshCpuSolution calculator)
        {
            this.camera = camera;
            this.converter = converter;
            this.calculator = calculator;

            frameLoopThread = new Thread(frameLoop);
            frameLoopThread.Start();
        }
#pragma warning restore IDE0051

        private void frameLoop()
        {
            if (camera == null || calculator == null)
                return;

            while (true)
            {
                Frame frame = camera.GetFrame();
                converter ??= new FrameConverter(frame, PixelFormat.Rgba);
                Frame cFrame = converter.Convert(frame);

                using ImageFrame imgframe = new ImageFrame(ImageFormat.Srgba,
                    cFrame.Width, cFrame.Height, cFrame.WidthStep, cFrame.RawData);

                List<NormalizedLandmarkList>? landmarkList = calculator.Compute(imgframe);

                var pixelData = Image.LoadPixelData<Rgba32>(cFrame.RawData, cFrame.Width, cFrame.Height);

                texture ??= new Texture(cFrame.Width, cFrame.Height);
                texture.SetData(new TextureUpload(pixelData));
                sprite.FillAspectRatio = (float)cFrame.Width / cFrame.Height;
                sprite.Texture = texture;

                if (landmarkList != null && landmarkList[0] != null)
                    drawLandmarks(landmarkList[0]);
            }
        }

        private void drawLandmarks(NormalizedLandmarkList list)
        {
            RepeatedField<NormalizedLandmark> landmarks = list.Landmark;
            if (landmarks.Count == 0)
                return;

            if (circles == null)
            {
                circles = new Circle[landmarks.Count];
                for (int i = 0; i < circles.Length; i++)
                {
                    circles[i] = new Circle
                    {
                        X = 0,
                        Y = 0,
                        Width = 1,
                        Height = 1,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Colour4.GhostWhite,
                    };
                }
                Schedule(() => AddRangeInternal(circles));
            }

            for (int i = 0; i < landmarks.Count; i++)
            {
                NormalizedLandmark landmark = landmarks[i];
                Circle circle = circles[i];

                Schedule(() =>
                {
                    circle.X = (landmark.X - .5f) * Width;
                    circle.Y = (landmark.Y - .5f) * Height;

                    float size = sigmoid(landmark.Z) * 4;
                    circle.Width = size;
                    circle.Height = size;
                });
            }
        }

        private float sigmoid(float x) => 1 / (1 + MathF.Exp(-x));
    }
}
