// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Packets;
using NUnit.Framework;

namespace Mediapipe.Net.Tests.Framework.NewPacket
{
    public class FloatPacketTest
    {
        #region Constructor
        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithValue()
        {
            using var packet = PacketFactory.FloatPacket(0.01f);
            Assert.True(packet.ValidateAsFloat().Ok());
            Assert.AreEqual(packet.GetFloat(), 0.01f);
            Assert.AreEqual(packet.Timestamp(), Timestamp.Unset());
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithValueAndTimestamp()
        {
            using var timestamp = new Timestamp(1);
            using var packet = PacketFactory.FloatPacket(0.01f).At(timestamp);
            Assert.True(packet.ValidateAsFloat().Ok());
            Assert.AreEqual(packet.GetFloat(), 0.01f);
            Assert.AreEqual(packet.Timestamp(), timestamp);
        }
        #endregion

        #region IsDisposed
        [Test]
        public void IsDisposed_ShouldReturnFalse_When_NotDisposedYet()
        {
            using var packet = PacketFactory.FloatPacket(0f);
            Assert.False(packet.IsDisposed);
        }

        [Test]
        public void IsDisposed_ShouldReturnTrue_When_AlreadyDisposed()
        {
            var packet = PacketFactory.FloatPacket(0f);
            packet.Dispose();

            Assert.True(packet.IsDisposed);
        }
        #endregion

        #region DebugTypeName
        [Test]
        public void DebugTypeName_ShouldReturnFloat_When_ValueIsSet()
        {
            using var packet = PacketFactory.FloatPacket(0.01f);
            Assert.AreEqual(packet.DebugTypeName(), "float");
        }
        #endregion
    }
}
