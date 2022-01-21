// Copyright (c) homuler and Vignette
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using CommandLine;

namespace Mediapipe.Net.Examples.FaceMesh
{
    public class Options
    {
        [Option('c', "camera", Default = 0, HelpText = "The index of the camera to use")]
        public int CameraIndex { get; set; }
    }
}
