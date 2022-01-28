// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using osu.Framework;
using osu.Framework.Platform;
using SeeShark.FFmpeg;

namespace Mediapipe.Net.Examples.OsuFrameworkVisualTests
{
    public static class Program
    {
        public static void Main()
        {
            FFmpegManager.SetupFFmpeg("/usr/lib");
            using GameHost host = Host.GetSuitableHost("visual-tests");
            using var game = new OsuFrameworkVisualTestsTestBrowser();
            host.Run(game);
        }
    }
}
