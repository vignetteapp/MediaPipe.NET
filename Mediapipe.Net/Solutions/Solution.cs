// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Port;

namespace Mediapipe.Net.Solutions
{
    /// <summary>
    /// The base for any calculator.
    /// </summary>
    /// <typeparam name="TPacket">The type of packet the calculator returns the secondary output in.</typeparam>
    /// <typeparam name="T">The type of secondary output.</typeparam>
    public abstract class Solution : Disposable
    {
        protected readonly string GraphPath;
        protected readonly CalculatorGraph Graph;
        protected readonly SidePackets? SidePackets;

        protected readonly IDictionary<string, Packet> GraphOutputs;
        private readonly IDictionary<string, GCHandle> observeStreamHandles;

        protected long SimulatedTimestamp = 0;

        protected Solution(
            string graphPath,
            // SomeType calculatorParams,
            IEnumerable<string> outputs,
            SidePackets? sidePackets)
        {
            GraphPath = graphPath;
            Graph = new CalculatorGraph(File.ReadAllText(GraphPath));
            SidePackets = sidePackets;

            GraphOutputs = new Dictionary<string, Packet>();

            observeStreamHandles = new Dictionary<string, GCHandle>();
            foreach (string output in outputs)
            {
                Graph.ObserveOutputStream(output, (packet) =>
                {
                    Console.WriteLine($"Packet: {packet.DebugTypeName()})");
                    GraphOutputs.Add(output, packet);
                    return Status.Ok();
                }, out GCHandle handle).AssertOk();
                observeStreamHandles.Add(output, handle);
            }

            Graph.StartRun(SidePackets).AssertOk();
        }

        /// <summary>
        /// Feeds each input packet to the calculator graph and returns all outputs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        protected IDictionary<string, Packet> Process(IDictionary<string, Packet> inputs)
        {
            // Set the timestamp increment to 16666 us to simulate 60 fps video input (?)
            // That's what the Python API does so ¯\_(ツ)_/¯
            // Might have to find something better?
            SimulatedTimestamp += 16666;

            // Dispose of the previous packets before processing
            foreach (Packet packet in GraphOutputs.Values)
                packet.Dispose();
            GraphOutputs.Clear();

            foreach (KeyValuePair<string, Packet> input in inputs)
            {
                if (input.Value != null)
                    Graph.AddPacketToInputStream(input.Key, input.Value);
            }

            Graph.WaitUntilIdle();
            return GraphOutputs;
        }

        protected abstract IDictionary<string, Packet> ProcessFrame(ImageFrame frame);

        /// <summary>
        /// Closes all the input sources and the graph.
        /// </summary>
        public void Close() => Graph.CloseAllPacketSources().AssertOk();

        public void Reset()
        {
            Graph.CloseAllPacketSources().AssertOk();
            Graph.StartRun(SidePackets).AssertOk();
        }

        protected override void DisposeManaged()
        {
            Graph.WaitUntilDone();
            Graph.Dispose();

            SidePackets?.Dispose();

            foreach (var handle in observeStreamHandles.Values)
                handle.Free();
        }
    }
}
