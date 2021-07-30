﻿using System;
using System.IO;
using Equilibrium.IO;
using Equilibrium.Meta;
using Equilibrium.Meta.Options;
using Equilibrium.Models;
using Equilibrium.Models.Serialization;
using JetBrains.Annotations;

namespace Equilibrium.Implementations {
    [PublicAPI, UsedImplicitly, ObjectImplementation(UnityClassId.MonoBehaviour)]
    public class MonoBehaviour : Behaviour {
        public MonoBehaviour(BiEndianBinaryReader reader, UnityObjectInfo info, SerializedFile serializedFile) : base(reader, info, serializedFile) {
            Script = PPtr<MonoScript>.FromReader(reader, serializedFile);
            Name = reader.ReadString32();
            ShouldDeserialize = true;
            DataStart = reader.BaseStream.Position;
        }

        public MonoBehaviour(UnityObjectInfo info, SerializedFile serializedFile) : base(info, serializedFile) {
            Script = PPtr<MonoScript>.Null;
            Name = string.Empty;
        }

        public PPtr<MonoScript> Script { get; set; }
        public string Name { get; set; }
        public object? Data { get; set; }
        public ObjectNode? ObjectData { get; set; }
        private long DataStart { get; set; }

        public override void Deserialize(BiEndianBinaryReader reader, ObjectDeserializationOptions options) {
            base.Deserialize(reader, options);

            if (ObjectData == null &&
                SerializedFile.Assets != null) {
                var script = Script.Value;
                if (script == null) {
                    throw new NullReferenceException();
                }

                var name = script.ToString();

                var info = SerializedFile.ObjectInfos[PathId];
                if (info.TypeIndex > 0 &&
                    info.TypeIndex < SerializedFile.Types.Length &&
                    SerializedFile.Types[info.TypeIndex].TypeTree != null) {
                    ObjectData = SerializedFile.Assets.FindObjectNode(name, SerializedFile.Types[info.TypeIndex].TypeTree);
                } else {
                    var assemblyPath = options.RequestAssemblyCallback?.Invoke(script.AssemblyName);
                    if (string.IsNullOrWhiteSpace(assemblyPath)) {
                        return;
                    }

                    SerializedFile.Assets.LoadFile(assemblyPath);
                    ObjectData = SerializedFile.Assets.FindObjectNode(name, null);
                }

                if (ObjectData == null) {
                    return;
                }

                reader.BaseStream.Seek(DataStart, SeekOrigin.Begin);
                Data = ObjectFactory.CreateObject(reader, ObjectData, SerializedFile, "m_Name");
                ShouldDeserialize = false;
            }
        }

        public override void Serialize(BiEndianBinaryWriter writer, UnityVersion? targetVersion, FileSerializationOptions options) {
            if (ShouldDeserialize) {
                throw new InvalidOperationException();
            }

            base.Serialize(writer, targetVersion, options);

            Script.ToWriter(writer, SerializedFile, targetVersion);
            writer.WriteString32(Name);
            throw new NotImplementedException();
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Script, Name);

        public override string ToString() => string.IsNullOrEmpty(Name) ? base.ToString() : Name;
    }
}
