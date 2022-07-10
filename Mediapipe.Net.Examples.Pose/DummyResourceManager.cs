// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.IO;
using Mediapipe.Net.Util;

namespace Mediapipe.Net.Examples.Pose
{
    public class DummyResourceManager : ResourceManager
    {
        public override PathResolver ResolvePath => (path) =>
        {
            Console.WriteLine($"PathResolver: (not) resolving path '{path}'");
            return path;
        };

        public override ResourceProvider ProvideResource => (path) =>
        {
            Console.WriteLine($"ResourceProvider: providing resource '{path}'");
            byte[] bytes = File.ReadAllBytes(path);
            return bytes;
        };
    }
}
