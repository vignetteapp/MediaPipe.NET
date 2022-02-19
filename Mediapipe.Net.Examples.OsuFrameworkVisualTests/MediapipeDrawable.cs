// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Calculators;
using Mediapipe.Net.Framework.Format;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using SeeShark;
using SeeShark.Decode;
using SeeShark.Device;
using SeeShark.FFmpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace Mediapipe.Net.Examples.OsuFrameworkVisualTests
{
    public class MediapipeDrawable : CompositeDrawable
    {
        public readonly Camera Camera;
        private FrameConverter? converter;
        private readonly FaceMeshCpuCalculator calculator;

        private readonly Sprite sprite;
        private Texture? texture;

        public MediapipeDrawable(int cameraIndex = 0)
        {
            var manager = new CameraManager();
            Camera = manager.GetDevice(cameraIndex, new VideoInputOptions
            {
                InputFormat = "mjpeg",
                VideoSize = (800, 600),
            });
            manager.Dispose();

            calculator = new FaceMeshCpuCalculator();

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

        public void Start()
        {
            calculator.Run();
        }

        protected override unsafe void Update()
        {
            if (Camera.TryGetFrame(out Frame frame) != DecodeStatus.NewFrame)
                return;

            if (converter == null)
                converter = new FrameConverter(frame, PixelFormat.Rgba);

            Frame cFrame = converter.Convert(frame);

            ImageFrame imgFrame;
            fixed (byte* rawDataPtr = cFrame.RawData)
            {
                imgFrame = new ImageFrame(ImageFormat.Srgba, cFrame.Width, cFrame.Height, cFrame.WidthStep,
                    rawDataPtr);
            }

            using ImageFrame outImgFrame = calculator.Send(imgFrame);
            imgFrame.Dispose();

            var span = new ReadOnlySpan<byte>(outImgFrame.MutablePixelData, outImgFrame.Height * outImgFrame.WidthStep);
            var pixelData = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(span, cFrame.Width, cFrame.Height);

            texture ??= new Texture(cFrame.Width, cFrame.Height);
            texture.SetData(new TextureUpload(pixelData));
            sprite.FillAspectRatio = (float)cFrame.Width / cFrame.Height;
            sprite.Texture = texture;
            base.Update();
        }

        public new void Dispose()
        {
            if (IsDisposed)
                return;

            Camera.StopCapture();
            Camera.Dispose();
            converter?.Dispose();
            calculator.Dispose();
            base.Dispose();
        }
    }
}
