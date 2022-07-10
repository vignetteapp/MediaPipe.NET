// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Mediapipe.Net.Examples.OsuFrameworkVisualTests.Visual
{
    public class TestSceneFaceMeshCpu : OsuFrameworkVisualTestsTestScene
    {
#pragma warning disable IDE0051
        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4Extensions.FromHex(@"272727"),
                    },
                    new MediapipeDrawable
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(800, 450),
                        FillMode = FillMode.Fit,
                    },
                },
            });
        }
#pragma warning restore IDE0051
    }
}
