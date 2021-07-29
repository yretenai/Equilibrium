﻿using System;
using Equilibrium.IO;
using Equilibrium.Meta.Options;
using JetBrains.Annotations;

namespace Equilibrium.Models.Serialization {
    [PublicAPI]
    public record UnityScriptInfo(
        int Index,
        long PathId) {
        public static UnityScriptInfo FromReader(BiEndianBinaryReader reader, UnitySerializedFile header, EquilibriumOptions options) {
            if (header.Version >= UnitySerializedFileVersion.BigIdAlwaysEnabled) {
                reader.Align();
            }

            var index = reader.ReadInt32();
            var identifier = header.BigIdEnabled ? reader.ReadInt64() : reader.ReadInt32();
            return new UnityScriptInfo(index, identifier);
        }

        public static UnityScriptInfo[] ArrayFromReader(BiEndianBinaryReader reader, UnitySerializedFile header, EquilibriumOptions options) {
            if (header.Version <= UnitySerializedFileVersion.ScriptTypeIndex) {
                return Array.Empty<UnityScriptInfo>();
            }

            var count = reader.ReadInt32();
            var array = new UnityScriptInfo[count];
            for (var i = 0; i < count; ++i) {
                array[i] = FromReader(reader, header, options);
            }

            return array;
        }
    }
}
