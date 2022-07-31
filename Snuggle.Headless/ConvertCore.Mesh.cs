﻿using System.IO;
using DragonLib;
using Snuggle.Converters;
using Snuggle.Core.Implementations;
using Snuggle.Core.Options;

namespace Snuggle.Headless;

public static partial class ConvertCore {
    public static void ConvertMesh(SnuggleFlags flags, Mesh mesh) {
        mesh.Deserialize(ObjectDeserializationOptions.Default);

        var path = PathFormatter.Format(mesh.HasContainerPath ? flags.OutputFormat : flags.ContainerlessOutputFormat ?? flags.OutputFormat, "gltf", mesh);
        var fullPath = Path.Combine(flags.OutputPath, path);
        if (!flags.Overwrite && File.Exists(fullPath)) {
            return;
        }

        fullPath.EnsureDirectoryExists();

        SnuggleMeshFile.Save(
            mesh,
            fullPath,
            ObjectDeserializationOptions.Default,
            SnuggleExportOptions.Default with {
                WriteNativeTextures = flags.WriteNativeTextures,
                OnlyWithCABPath = flags.OnlyCAB,
                PathTemplate = flags.OutputFormat,
                ContainerlessPathTemplate = flags.ContainerlessOutputFormat ?? flags.OutputFormat,
            },
            SnuggleMeshExportOptions.Default with {
                FindGameObjectDescendants = flags.GameObjectHierarchyDown,
                FindGameObjectParents = flags.GameObjectHierarchyUp,
                WriteMaterial = flags.Materials,
                WriteTexture = flags.Texture,
                WriteVertexColors = flags.VertexColor,
                WriteMorphs = flags.Morphs,
                MirrorXPosition = !flags.KeepXPos,
                MirrorXNormal = !flags.KeepXNorm,
                MirrorXTangent = !flags.KeepXTan,
            });
    }
}
