// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Core;
using Stride.Games;
using Stride.Graphics;
using Buffer = Stride.Graphics.Buffer;

namespace Stride.Rendering
{
    /// <summary>
    /// Keys used for render target settings.
    /// </summary>
    public static class RenderTargetKeys
    {
        /// <summary>
        /// The depth stencil buffer key.
        /// </summary>
        public static readonly ObjectParameterKey<Texture> DepthStencil = ParameterKeys.NewObject<Texture>();

        /// <summary>
        /// The depth stencil buffer key used as an input shader resource.
        /// </summary>
        public static readonly ObjectParameterKey<Texture> DepthStencilSource = ParameterKeys.NewObject<Texture>();

        /// <summary>
        /// The render target key.
        /// </summary>
        public static readonly ObjectParameterKey<Texture> RenderTarget = ParameterKeys.NewObject<Texture>();

        /// <summary>
        /// The render target key.
        /// </summary>
        public static readonly ObjectParameterKey<Buffer> StreamTarget = ParameterKeys.NewObject<Buffer>();

        /// <summary>
        /// Used by <see cref="RenderTargetPlugin"/> to notify that the plugin requires support for depth stencil as shader resource
        /// </summary>
        public static readonly PropertyKey<bool> RequireDepthStencilShaderResource = new PropertyKey<bool>("RequireDepthStencilShaderResource", typeof(RenderTargetKeys));
    }
}
