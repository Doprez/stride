﻿// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.IO;
using Silk.NET.Assimp;
using Stride.Core.Diagnostics;
using Stride.Core.Mathematics;

namespace Stride.Importer.ThreeD.Material
{
    public static unsafe class Materials
    {
        public const string MatKeyTexTypeBase = "$tex.type";
        public const string MatKeyTexColorBase = "$tex.color";
        public const string MatKeyTexAlphaBase = "$tex.alpha";

        /// <summary>
        /// Converts an Assimp's material stack operation From c++ to c#.
        /// </summary>
        public static readonly Operation[] ConvertAssimpStackOperationCppToCs = new Operation[]
        {
            Operation.Add,			//aiStackOperation_Add
            Operation.Add3ds,		//aiStackOperation_Add3ds
            Operation.AddMaya,		//aiStackOperation_AddMaya
            Operation.Average,		//aiStackOperation_Average
            Operation.Color,			//aiStackOperation_Color
            Operation.ColorBurn,		//aiStackOperation_ColorBurn
            Operation.ColorDodge,	//aiStackOperation_ColorDodge
            Operation.Darken3ds,		//aiStackOperation_Darken3ds
            Operation.DarkenMaya,	//aiStackOperation_DarkenMaya
            Operation.Desaturate,	//aiStackOperation_Desaturate
            Operation.Difference3ds,	//aiStackOperation_Difference3ds
            Operation.DifferenceMaya,//aiStackOperation_DifferenceMaya
            Operation.Divide,		//aiStackOperation_Divide
            Operation.Exclusion,		//aiStackOperation_Exclusion
            Operation.HardLight,		//aiStackOperation_HardLight
            Operation.HardMix,		//aiStackOperation_HardMix
            Operation.Hue,			//aiStackOperation_Hue
            Operation.Illuminate,	//aiStackOperation_Illuminate
            Operation.In,			//aiStackOperation_In
            Operation.Lighten3ds,	//aiStackOperation_Lighten3ds
            Operation.LightenMaya,	//aiStackOperation_LightenMaya
            Operation.LinearBurn,	//aiStackOperation_LinearBurn
            Operation.LinearDodge,	//aiStackOperation_LinearDodge
            Operation.Multiply3ds,	//aiStackOperation_Multiply3ds
            Operation.MultiplyMaya,	//aiStackOperation_MultiplyMaya
            Operation.None,			//aiStackOperation_None
            Operation.Out,			//aiStackOperation_Out
            Operation.Over3ds,		//aiStackOperation_Over3ds
            Operation.Overlay3ds,	//aiStackOperation_Overlay3ds
            Operation.OverMaya,		//aiStackOperation_OverMaya
            Operation.PinLight,		//aiStackOperation_PinLight
            Operation.Saturate,		//aiStackOperation_Saturate
            Operation.Saturation,	//aiStackOperation_Saturation
            Operation.Screen,		//aiStackOperation_Screen
            Operation.SoftLight,		//aiStackOperation_SoftLight
            Operation.Substract3ds,	//aiStackOperation_Substract3ds
            Operation.SubstractMaya,	//aiStackOperation_SubstractMaya
            Operation.Value,			//aiStackOperation_Value
            Operation.Mask           //aiStackOperation_Mask
        };

        /// <summary>
        /// Converts an Assimp's material stack node type From c++ to c#.
        /// </summary>
        public static readonly StackElementType[] ConvertAssimpStackTypeCppToCs = new StackElementType[]
        {
            StackElementType.Color,			// aiStackType_ColorType
            StackElementType.Texture,		// aiStackType_TextureType
            StackElementType.Operation      // aiStackType_BlemdOpType
        };

        public static readonly MappingMode[] ConvertAssimpMappingModeCppToCs = new MappingMode[]
        {
            MappingMode.Wrap,		// aiTextureMapMode_Wrap
            MappingMode.Clamp,		// aiTextureMapMode_Clamp
            MappingMode.Mirror,		// aiTextureMapMode_Mirror
            MappingMode.Decal        // aiTextureMapMode_Decal
        };

        public static unsafe MaterialStack ConvertAssimpStackCppToCs(Silk.NET.Assimp.Assimp assimp, Scene* scene, Silk.NET.Assimp.Material* material, Silk.NET.Assimp.TextureType type, Logger logger)
        {
            var ret = new MaterialStack();
            var count = (int)assimp.GetMaterialTextureCount(material, type);

            // Process the material stack
            for (int iEl = count - 1; iEl >= 0; --iEl)
            {
                StackElement el;
                // Common properties
                int elType = 0, elFlags = 0;
                float elAlpha = 0.0f, elBlend = 0.0f;
                // Operation-specific properties
                int elOp = 0;
                // Color-specific properties
                var elColor = new System.Numerics.Vector4();
                // Texture-specific properties
                var elTexPath = new AssimpString();
                int elTexChannel = 0, elMappingModeU = 0, elMappingModeV = 0;
                uint pMax = 0;

                if (assimp.GetMaterialFloatArray(material, MatKeyTexAlphaBase, (uint)type, (uint)iEl, ref elAlpha, ref pMax) != Return.Success)
                    elAlpha = 1.0f; // default alpha
                if (assimp.GetMaterialFloatArray(material, Silk.NET.Assimp.Assimp.MaterialTexblendBase, (uint)type, (uint)iEl, ref elBlend, ref pMax) != Return.Success)
                    elBlend = 1.0f; // default blend
                if (assimp.GetMaterialIntegerArray(material, Silk.NET.Assimp.Assimp.MaterialTexflagsBase, (uint)type, (uint)iEl, ref elFlags, ref pMax) != Return.Success)
                    elFlags = 0; // default flags (no flags)
                if (assimp.GetMaterialIntegerArray(material, MatKeyTexTypeBase, (uint)type, (uint)iEl, ref elType, ref pMax) != Return.Success)
                    elType = (int)StackElementType.Texture;//continue; // error !

                switch ((StackElementType)elType)
                {
                    case StackElementType.Operation:
                        if (assimp.GetMaterialIntegerArray(material, Silk.NET.Assimp.Assimp.MaterialTexopBase, (uint)type, (uint)iEl, ref elOp, ref pMax) != Return.Success)
                        {
                            logger?.Error("Material not found");
                            continue; // error !
                        }
                        el = new StackOperation(ConvertAssimpStackOperationCppToCs[elOp], elAlpha, elBlend, elFlags);
                        break;
                    case StackElementType.Color:
                        if (assimp.GetMaterialColor(material, MatKeyTexColorBase, (uint)type, (uint)iEl, ref elColor) != Return.Success)
                        {
                            logger?.Error("Material with index not found");
                            continue; // error !
                        }
                        el = new StackColor(new Color3(elColor.X, elColor.Y, elColor.Z), elAlpha, elBlend, elFlags);
                        break;
                    case StackElementType.Texture:
                        if (assimp.GetMaterialString(material, Silk.NET.Assimp.Assimp.MaterialTextureBase, (uint)type, (uint)iEl, ref elTexPath) != Return.Success)
                        {
                            logger?.Error("Material texture item not found");
                            continue; // error !
                        }
                        if (assimp.GetMaterialIntegerArray(material, Silk.NET.Assimp.Assimp.MaterialUvwsrcBase, (uint)type, (uint)iEl, ref elTexChannel, ref pMax) != Return.Success)
                        {
                            logger?.Error("Material integer item not found");
                            continue; // error !
                        }
                        if (assimp.GetMaterialIntegerArray(material, Silk.NET.Assimp.Assimp.MaterialMappingmodeUBase, (uint)type, (uint)iEl, ref elMappingModeU, ref pMax) != Return.Success)
                            elMappingModeU = (int)TextureMapMode.Wrap; // default mapping mode
                        if (assimp.GetMaterialIntegerArray(material, Silk.NET.Assimp.Assimp.MaterialMappingmodeVBase, (uint)type, (uint)iEl, ref elMappingModeV, ref pMax) != Return.Success)
                            elMappingModeV = (int)TextureMapMode.Wrap; // default mapping mode

                        // Determine the physical texture file name
                        if (!TryGetTextureFileName(elTexPath.AsString, scene, out var texFileName, out var errorMessage))
                        {
                            logger?.Error(errorMessage);
                            continue; // error !
                        }
                        el = new StackTexture(
                            texFileName,
                            elTexChannel,
                            ConvertAssimpMappingModeCppToCs[elMappingModeU],
                            ConvertAssimpMappingModeCppToCs[elMappingModeV],
                            elAlpha,
                            elBlend,
                            elFlags);
                        break;
                    default:
                        // error !
                        continue;
                }

                ret.Push(el);
            }

            return ret;
        }

        internal static unsafe bool TryGetTextureFileName(string materialTextureFileName, Scene* scene, out string fileName, out string errorMessage)
        {
            var texFileName = Path.GetFileName(materialTextureFileName);
            if (!texFileName.StartsWith(Assimp.EmbeddedTexnamePrefix))
            {
                fileName = texFileName;
                errorMessage = null;
                return true;
            }
            // Embedded texture is denoted by '*' + index value, where the texture is accessed by scene->MTextures[index]
            var texIndexStringSpan = texFileName.AsSpan().Slice(Assimp.EmbeddedTexnamePrefix.Length);
            if (int.TryParse(texIndexStringSpan, out int texIndex))
            {
                var texture = scene->MTextures[texIndex];
                return TryGetTextureFileName(texture, out fileName, out errorMessage);
            }
            else
            {
                fileName = null;
                errorMessage = $"Invalid texture name for embedded texture: {texFileName} - Expected a number.";
                return false;
            }
        }

        internal unsafe static bool TryGetTextureFileName(Silk.NET.Assimp.Texture* texture, out string fileName, out string errorMessage)
        {
            var texFileName = Path.GetFileName(texture->MFilename.AsString);
            if (Path.HasExtension(texFileName))
            {
                fileName = texFileName;
                errorMessage = null;
                return true;
            }
            // Formats like glTF may strip the file extension from the file name, so we need to bring this back
            if (texture->MHeight != 0)
            {
                fileName = null;
                errorMessage = $"Could not determine texture file name: Texture '{fileName}' is an embedded uncompressed texture.";
                return false;
            }
            var fileExt = System.Text.Encoding.UTF8.GetString(texture->AchFormatHint, byteCount: Assimp.Hintmaxtexturelen).Trim('\0');
            if (!string.IsNullOrEmpty(fileExt))
            {
                fileName = texFileName + '.' + fileExt;
                errorMessage = null;
                return true;
            }
            else
            {
                fileName = null;
                errorMessage = $"Could not determine texture file name: Embedded Texture '{fileName}' has unknown file extension.";
                return false;
            }
        }
    }
}
