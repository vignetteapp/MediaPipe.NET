// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Packets;
using NUnit.Framework;

namespace Mediapipe.Net.Tests.Framework.NewPacket
{
    public class FloatArrayPacketTest
    {
        #region Constructor
        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithEmptyArray()
        {
            float[] array = Array.Empty<float>();
            using var packet = PacketFactory.FloatArrayPacket(array);
            Assert.True(packet.ValidateAsFloatArray().Ok());
            Assert.AreEqual(packet.GetFloatArray(), array);
            Assert.AreEqual(packet.Timestamp(), Timestamp.Unset());
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithArray()
        {
            float[] array = { 0.01f };
            using var packet = PacketFactory.FloatArrayPacket(array);
            Assert.True(packet.ValidateAsFloatArray().Ok());
            Assert.AreEqual(packet.GetFloatArray(), array);
            Assert.AreEqual(packet.Timestamp(), Timestamp.Unset());
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithValueAndTimestamp()
        {
            float[] array = { 0.01f, 0.02f };
            using var timestamp = new Timestamp(1);

            // Using PacketFactory.FloatArrayPacket(array).At(timestamp) messes up the float values in the array.
            // We get [0.00999999978f, 0.0199999996f] instead of [0.01f, 0.02f]. No clue why that happens.
            using var packet = PacketFactory.FloatArrayPacket(array, timestamp);
            Assert.True(packet.ValidateAsFloatArray().Ok());
            Assert.AreEqual(packet.GetFloatArray(), array);
            Assert.AreEqual(packet.Timestamp(), timestamp);
        }
        #endregion

        #region IsDisposed
        [Test]
        public void IsDisposed_ShouldReturnFalse_When_NotDisposedYet()
        {
            using var packet = PacketFactory.FloatArrayPacket(Array.Empty<float>());
            Assert.False(packet.IsDisposed);
        }

        [Test]
        public void IsDisposed_ShouldReturnTrue_When_AlreadyDisposed()
        {
            var packet = PacketFactory.FloatArrayPacket(Array.Empty<float>());
            packet.Dispose();

            Assert.True(packet.IsDisposed);
        }
        #endregion

        #region DebugTypeName
        [Test]
        public void DebugTypeName_ShouldReturnFloat_When_ValueIsSet()
        {
            float[] array = { 0.01f };
            using var packet = PacketFactory.FloatArrayPacket(array);

            Assert.AreEqual(packet.DebugTypeName(),
                OperatingSystem.IsWindows()
                    ? "float [0]"
                    : "float []");
        }
        #endregion
    }
}
