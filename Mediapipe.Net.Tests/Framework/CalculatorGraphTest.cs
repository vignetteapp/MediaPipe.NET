// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Port;
using Mediapipe.Net.Framework.Protobuf;
using NUnit.Framework;

namespace Mediapipe.Net.Tests.Framework
{
    public class CalculatorGraphTest
    {
        // TODO: honestly this would be way better as the content of a file.
        private const string valid_config_text = @"node {
  calculator: ""PassThroughCalculator""
  input_stream: ""in""
  output_stream: ""out1""
}
node {
  calculator: ""PassThroughCalculator""
  input_stream: ""out1""
  output_stream: ""out""
}
input_stream: ""in""
output_stream: ""out""
";

        #region Constructor
        [Test]
        public void Ctor_ShouldInstantiateCalculatorGraph_When_CalledWithNoArguments()
        {
            Assert.DoesNotThrow(() =>
            {
                var graph = new CalculatorGraph();
                graph.Dispose();
            });
        }

        [Test]
        public void Ctor_ShouldInstantiateCalculatorGraph_When_CalledWithConfigText()
        {
            using var graph = new CalculatorGraph(valid_config_text);
            var config = graph.Config();
            Assert.AreEqual(config.InputStream[0], "in");
            Assert.AreEqual(config.OutputStream[0], "out");
        }
        #endregion

        #region IsDisposed
        [Test]
        public void IsDisposed_ShouldReturnFalse_When_NotDisposedYet()
        {
            using var graph = new CalculatorGraph();
            Assert.False(graph.IsDisposed);
        }

        [Test]
        public void IsDisposed_ShouldReturnTrue_When_AlreadyDisposed()
        {
            var graph = new CalculatorGraph();
            graph.Dispose();

            Assert.True(graph.IsDisposed);
        }
        #endregion

        #region Initialize
        [Test]
        public void Initialize_ShouldReturnOk_When_CalledWithConfig_And_ConfigIsNotSet()
        {
            using var graph = new CalculatorGraph();
            using (var status = graph.Initialize(CalculatorGraphConfig.Parser.ParseFromTextFormat(valid_config_text)))
            {
                Assert.True(status.Ok());
            }

            var config = graph.Config();
            Assert.AreEqual(config.InputStream[0], "in");
            Assert.AreEqual(config.OutputStream[0], "out");
        }

        [Test]
        public void Initialize_ShouldReturnInternalError_When_CalledWithConfig_And_ConfigIsSet()
        {
            using var graph = new CalculatorGraph(valid_config_text);
            using var status = graph.Initialize(CalculatorGraphConfig.Parser.ParseFromTextFormat(valid_config_text));
            Assert.AreEqual(status.Code, Status.StatusCode.Internal);
        }

        [Test]
        public void Initialize_ShouldReturnOk_When_CalledWithConfigAndSidePacket_And_ConfigIsNotSet()
        {
            using var sidePackets = new SidePackets();
            sidePackets.Emplace("flag", PacketFactory.BoolPacket(true));

            using var graph = new CalculatorGraph();
            var config = CalculatorGraphConfig.Parser.ParseFromTextFormat(valid_config_text);

            using var status = graph.Initialize(config, sidePackets);
            Assert.True(status.Ok());
        }

        [Test]
        public void Initialize_ShouldReturnInternalError_When_CalledWithConfigAndSidePacket_And_ConfigIsSet()
        {
            using var sidePackets = new SidePackets();
            sidePackets.Emplace("flag", PacketFactory.BoolPacket(true));

            using var graph = new CalculatorGraph(valid_config_text);
            var config = CalculatorGraphConfig.Parser.ParseFromTextFormat(valid_config_text);

            using var status = graph.Initialize(config, sidePackets);
            Assert.AreEqual(status.Code, Status.StatusCode.Internal);
        }
        #endregion

        #region Lifecycle
        [Test]
        public void LifecycleMethods_ShouldControlGraphLifeCycle()
        {
            using var graph = new CalculatorGraph(valid_config_text);
            Assert.True(graph.StartRun().Ok());
            Assert.False(graph.GraphInputStreamsClosed());

            Assert.True(graph.WaitUntilIdle().Ok());
            Assert.True(graph.CloseAllPacketSources().Ok());
            Assert.True(graph.GraphInputStreamsClosed());
            Assert.True(graph.WaitUntilDone().Ok());
            Assert.False(graph.HasError());
        }

        [Test]
        public void Cancel_ShouldCancelGraph()
        {
            using var graph = new CalculatorGraph(valid_config_text);
            Assert.True(graph.StartRun().Ok());
            graph.Cancel();
            Assert.AreEqual(graph.WaitUntilDone().Code, Status.StatusCode.Cancelled);
        }
        #endregion
    }
}
