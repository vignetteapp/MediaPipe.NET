// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Text.RegularExpressions;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Packets;
using NUnit.Framework;

namespace Mediapipe.Net.Tests.Framework.NewPacket
{
    public class StringPacketTest
    {
        #region Constructor
        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithString()
        {
            using var packet = PacketFactory.StringPacket("test");
            Assert.True(packet.ValidateAsString().Ok());
            Assert.AreEqual(packet.GetString(), "test");
            Assert.AreEqual(packet.Timestamp(), Timestamp.Unset());
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithByteArray()
        {
            var bytes = new byte[] { (byte)'t', (byte)'e', (byte)'s', (byte)'t' };
            using var packet = PacketFactory.StringPacket(bytes);
            Assert.True(packet.ValidateAsString().Ok());
            Assert.AreEqual(packet.GetString(), "test");
            Assert.AreEqual(packet.Timestamp(), Timestamp.Unset());
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithStringAndTimestamp()
        {
            using var timestamp = new Timestamp(1);
            using var packet = PacketFactory.StringPacket("test").At(timestamp);
            Assert.True(packet.ValidateAsString().Ok());
            Assert.AreEqual(packet.GetString(), "test");
            Assert.AreEqual(packet.Timestamp(), timestamp);
        }

        [Test]
        public void Ctor_ShouldInstantiatePacket_When_CalledWithByteArrayAndTimestamp()
        {
            var bytes = new byte[] { (byte)'t', (byte)'e', (byte)'s', (byte)'t' };
            using var timestamp = new Timestamp(1);
            using var packet = PacketFactory.StringPacket(bytes).At(timestamp);
            Assert.True(packet.ValidateAsString().Ok());
            Assert.AreEqual(packet.GetString(), "test");
            Assert.AreEqual(packet.Timestamp(), timestamp);
        }
        #endregion

        #region IsDisposed
        [Test]
        public void IsDisposed_ShouldReturnFalse_When_NotDisposedYet()
        {
            using var packet = PacketFactory.StringPacket("");
            Assert.False(packet.IsDisposed);
        }

        [Test]
        public void IsDisposed_ShouldReturnTrue_When_AlreadyDisposed()
        {
            var packet = PacketFactory.StringPacket("");
            packet.Dispose();

            Assert.True(packet.IsDisposed);
        }
        #endregion

        #region GetByteArray
        [Test]
        public void GetByteArray_ShouldReturnByteArray()
        {
            var bytes = new byte[] { (byte)'a', (byte)'b', 0, (byte)'c' };
            using var packet = PacketFactory.StringPacket(bytes);
            Assert.AreEqual(packet.GetStringAsByteArray(), bytes);
            Assert.AreEqual(packet.GetString(), "ab");
        }
        #endregion

        #region DebugTypeName
        [Test]
        public void DebugTypeName_ShouldReturnString_When_ValueIsSet()
        {
            using var packet = PacketFactory.StringPacket("test");

            var regex = new Regex("string");
            Assert.True(regex.IsMatch(packet.DebugTypeName() ?? ""));
        }
        #endregion
    }
}
