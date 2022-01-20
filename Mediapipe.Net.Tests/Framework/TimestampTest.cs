// Copyright (c) homuler and Vignette
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework;
using NUnit.Framework;

namespace Mediapipe.Net.Tests.Framework
{
    public class TimestampTest
    {
        #region IsDisposed
        [Test]
        public void IsDisposed_ShouldReturnFalse_When_NotDisposedYet()
        {
            using var timestamp = new Timestamp(1);
            Assert.False(timestamp.IsDisposed);
        }

        [Test]
        public void IsDisposed_ShouldReturnTrue_When_AlreadyDisposed()
        {
            var timestamp = new Timestamp(1);
            timestamp.Dispose();

            Assert.True(timestamp.IsDisposed);
        }
        #endregion

        #region Value
        [Test]
        public void Value_ShouldReturnValue()
        {
            using var timestamp = new Timestamp(10);
            Assert.AreEqual(timestamp.Value(), 10);
        }
        #endregion

        #region Seconds
        [Test]
        public void Seconds_ShouldReturnValueInSeconds()
        {
            using var timestamp = new Timestamp(1_000_000);
            Assert.AreEqual(timestamp.Seconds(), 1d, 1e-9);
        }
        #endregion

        #region Microseconds
        [Test]
        public void Microseconds_ShouldReturnValueInMicroseconds()
        {
            using var timestamp = new Timestamp(1_000_000);
            Assert.AreEqual(timestamp.Microseconds(), 1_000_000);
        }
        #endregion

        #region IsSpecialValue
        [Test]
        public void IsSpecialValue_ShouldReturnFalse_When_ValueIsInRange()
        {
            using var timestamp = new Timestamp(1);
            Assert.False(timestamp.IsSpecialValue());
        }

        [Test]
        public void IsSpecialValue_ShouldReturnTrue_When_TimestampIsUnset()
        {
            using var timestamp = Timestamp.Unset();
            Assert.True(timestamp.IsSpecialValue());
        }

        [Test]
        public void IsSpecialValue_ShouldReturnTrue_When_TimestampIsUnstarted()
        {
            using var timestamp = Timestamp.Unstarted();
            Assert.True(timestamp.IsSpecialValue());
        }
        #endregion

        #region IsRangeValue
        [Test]
        public void IsRangeValue_ShouldReturnTrue_When_ValueIsInRange()
        {
            using var timestamp = new Timestamp(1);
            Assert.True(timestamp.IsRangeValue());
        }

        [Test]
        public void IsRangeValue_ShouldReturnFalse_When_TimestampIsPreStream()
        {
            using var timestamp = Timestamp.PreStream();
            Assert.False(timestamp.IsRangeValue());
        }

        [Test]
        public void IsRangeValue_ShouldReturnFalse_When_TimestampIsPostStream()
        {
            using var timestamp = Timestamp.PostStream();
            Assert.False(timestamp.IsRangeValue());
        }

        [Test]
        public void IsRangeValue_ShouldReturnTrue_When_TimestampIsMin()
        {
            using var timestamp = Timestamp.Min();
            Assert.True(timestamp.IsRangeValue());
        }

        [Test]
        public void IsRangeValue_ShouldReturnTrue_When_TimestampIsMax()
        {
            using var timestamp = Timestamp.Max();
            Assert.True(timestamp.IsRangeValue());
        }
        #endregion

        #region IsAllowedInStream
        [Test]
        public void IsAllowedInStream_ShouldReturnTrue_When_ValueIsInRange()
        {
            using var timestamp = new Timestamp(1);
            Assert.True(timestamp.IsAllowedInStream());
        }

        [Test]
        public void IsAllowedInStream_ShouldReturnFalse_When_TimestampIsOneOverPostStream()
        {
            using var timestamp = Timestamp.OneOverPostStream();
            Assert.False(timestamp.IsAllowedInStream());
        }

        [Test]
        public void IsAllowedInStream_ShouldReturnFalse_When_TimestampIsDone()
        {
            using var timestamp = Timestamp.Done();
            Assert.False(timestamp.IsAllowedInStream());
        }
        #endregion

        #region DebugString
        [Test]
        public void DebugString_ShouldReturnDebugString()
        {
            using var timestamp = new Timestamp(1);
            Assert.AreEqual(timestamp.DebugString(), "1");
        }

        [Test]
        public void DebugString_ShouldReturnDebugString_When_TimestampIsUnset()
        {
            using var timestamp = Timestamp.Unset();
            Assert.AreEqual(timestamp.DebugString(), "Timestamp::Unset()");
        }
        #endregion

        #region NextAllowedInStream
        [Test]
        public void NextAllowedInStream_ShouldReturnNextTimestamp_When_ValueIsInRange()
        {
            using var timestamp = new Timestamp(1);
            using var nextTimestamp = timestamp.NextAllowedInStream();
            Assert.AreEqual(nextTimestamp.Microseconds(), 2);
        }

        [Test]
        public void NextAllowedInStream_ShouldReturnOneOverPostStream_When_TimestampIsPostStream()
        {
            using var timestamp = Timestamp.PostStream();
            using var nextTimestamp = timestamp.NextAllowedInStream();
            Assert.AreEqual(nextTimestamp, Timestamp.OneOverPostStream());
        }
        #endregion

        #region PreviousAllowedInStream
        [Test]
        public void PreviousAllowedInStream_ShouldReturnPreviousTimestamp_When_ValueIsInRange()
        {
            using var timestamp = new Timestamp(1);
            using var nextTimestamp = timestamp.PreviousAllowedInStream();
            Assert.AreEqual(nextTimestamp.Microseconds(), 0);
        }

        [Test]
        public void PreviousAllowedInStream_ShouldReturnUnstarted_When_TimestampIsPreStream()
        {
            using var timestamp = Timestamp.PreStream();
            using var nextTimestamp = timestamp.PreviousAllowedInStream();
            Assert.AreEqual(nextTimestamp, Timestamp.Unstarted());
        }
        #endregion

        #region FromSeconds
        [Test]
        public void FromSeconds_ShouldReturnTimestamp()
        {
            using var timestamp = Timestamp.FromSeconds(1d);
            Assert.AreEqual(timestamp.Microseconds(), 1_000_000);
        }
        #endregion
    }
}