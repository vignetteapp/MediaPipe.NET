// Copyright (c) homuler and Vignette
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Runtime.Versioning;
using Mediapipe.Net.Core;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    public class GpuBuffer : MpResourceHandle
    {
        public GpuBuffer(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        public GpuBuffer(GlTextureBuffer glTextureBuffer) : base()
        {
            UnsafeNativeMethods.mp_GpuBuffer__PSgtb(glTextureBuffer.SharedPtr, out var ptr).Assert();
            glTextureBuffer.Dispose(); // respect move semantics
            Ptr = ptr;
        }

        protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_GpuBuffer__delete(Ptr);

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        public GlTextureBuffer GetGlTextureBuffer()
            => new GlTextureBuffer(SafeNativeMethods.mp_GpuBuffer__GetGlTextureBufferSharedPtr(MpPtr), false);

        public GpuBufferFormat Format => SafeNativeMethods.mp_GpuBuffer__format(MpPtr);

        public int Width => SafeNativeMethods.mp_GpuBuffer__width(MpPtr);

        public int Height => SafeNativeMethods.mp_GpuBuffer__height(MpPtr);
    }
}
