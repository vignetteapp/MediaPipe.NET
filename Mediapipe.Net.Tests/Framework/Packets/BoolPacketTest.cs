// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Packets;
using NUnit.Framework;

namespace Mediapipe.Net.Tests.Framework.NewPacket
{
    public class BoolPacketTest
    {
        #region Constructor
        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithTrue()
        {
            using var packet = PacketFactory.BoolPacket(true);
            Assert.True(packet.ValidateAsBool().Ok());
            Assert.True(packet.GetBool());
            Assert.AreEqual(packet.Timestamp(), Timestamp.Unset());
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithFalse()
        {
            using var packet = PacketFactory.BoolPacket(false);
            Assert.True(packet.ValidateAsBool().Ok());
            Assert.False(packet.GetBool());
            Assert.AreEqual(packet.Timestamp(), Timestamp.Unset());
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithValueAndTimestamp()
        {
            using var timestamp = new Timestamp(1);
            using var packet = PacketFactory.BoolPacket(true).At(timestamp);
            Assert.True(packet.ValidateAsBool().Ok());
            Assert.True(packet.GetBool());
            Assert.AreEqual(packet.Timestamp(), timestamp);
        }
        #endregion

        #region IsDisposed
        [Test]
        public void IsDisposed_ShouldReturnFalse_When_NotDisposedYet()
        {
            using var packet = PacketFactory.BoolPacket(true);
            Assert.False(packet.IsDisposed);
        }

        [Test]
        public void IsDisposed_ShouldReturnTrue_When_AlreadyDisposed()
        {
            var packet = PacketFactory.BoolPacket(true);
            packet.Dispose();

            Assert.True(packet.IsDisposed);
        }
        #endregion

        #region DebugTypeName
        [Test]
        public void DebugTypeName_ShouldReturnBool_When_ValueIsSet()
        {
            using var packet = PacketFactory.BoolPacket(true);
            Assert.AreEqual(packet.DebugTypeName(), "bool");
        }
        #endregion
    }
}
