﻿using System;
using JetBrains.Annotations;
using Snuggle.Core.IO;
using Snuggle.Core.Meta;
using Snuggle.Core.Models;
using Snuggle.Core.Models.Serialization;
using Snuggle.Core.Options;

namespace Snuggle.Core.Implementations;

[PublicAPI]
[UsedImplicitly]
[ObjectImplementation(UnityClassId.Sprite)]
public class Sprite : NamedObject {
    public Sprite(BiEndianBinaryReader reader, UnityObjectInfo info, SerializedFile serializedFile) : base(reader, info, serializedFile) { }

    public Sprite(UnityObjectInfo info, SerializedFile serializedFile) : base(info, serializedFile) { }

    public override void Serialize(BiEndianBinaryWriter writer, AssetSerializationOptions options) {
        throw new NotImplementedException();
    }
}
