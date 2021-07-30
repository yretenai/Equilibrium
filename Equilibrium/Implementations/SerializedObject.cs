﻿using System;
using System.Text.Json.Serialization;
using Equilibrium.IO;
using Equilibrium.Meta;
using Equilibrium.Meta.Interfaces;
using Equilibrium.Meta.Options;
using Equilibrium.Models;
using Equilibrium.Models.Serialization;
using JetBrains.Annotations;

namespace Equilibrium.Implementations {
    [PublicAPI, UsedImplicitly, ObjectImplementation(UnityClassId.Object)]
    public class SerializedObject : IEquatable<SerializedObject>, ISerialized {
        public SerializedObject(BiEndianBinaryReader reader, UnityObjectInfo info, SerializedFile serializedFile) : this(info, serializedFile) => IsMutated = false;

        public SerializedObject(UnityObjectInfo info, SerializedFile serializedFile) {
            SerializedFile = serializedFile;
            PathId = info.PathId;
            ClassId = info.ClassId;
            IsMutated = true;
        }

        public long PathId { get; init; }
        public object ClassId { get; init; }

        [JsonIgnore]
        public SerializedFile SerializedFile { get; init; }

        [JsonIgnore]
        public bool ShouldDeserialize { get; set; }

        [JsonIgnore]
        public bool IsMutated { get; set; }

        public bool Equals(SerializedObject? other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return PathId == other.PathId && ClassId == other.ClassId;
        }

        public virtual void Deserialize(BiEndianBinaryReader reader, ObjectDeserializationOptions options) {
            ShouldDeserialize = false;
        }

        public void Deserialize(ObjectDeserializationOptions options) {
            using var reader = new BiEndianBinaryReader(SerializedFile.OpenFile(PathId), SerializedFile.Header.IsBigEndian);
            Deserialize(reader, options);
        }

        public virtual void Serialize(BiEndianBinaryWriter writer, UnityVersion? targetVersion, FileSerializationOptions options) { }

        public virtual void Free() { }

        public override string ToString() => Enum.Format(ClassId.GetType(), ClassId, "G");

        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            return obj is SerializedObject unityObject && Equals(unityObject);
        }

        public override int GetHashCode() => HashCode.Combine(ClassId, PathId);
    }
}
