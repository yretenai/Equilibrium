﻿using System.IO;
using DragonLib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Snuggle.Converters;
using Snuggle.Core.Implementations;
using Snuggle.Core.Interfaces;
using Snuggle.Core.Options;

namespace Snuggle.Headless;

public static partial class ConvertCore {
    public static void ConvertSprite(SnuggleFlags flags, ILogger logger, Sprite sprite) {
        var path = PathFormatter.Format(flags.OutputFormat, "png", sprite);
        if (File.Exists(path)) {
            return;
        }

        var (data, (width, height), _) = SnuggleSpriteFile.ConvertSprite(sprite, ObjectDeserializationOptions.Default, flags.UseDirectXTex);
        var image = Image.WrapMemory<Rgba32>(data, width, height);
        var fullPath = Path.Combine(flags.OutputPath, path);
        fullPath.EnsureDirectoryExists();
        image.SaveAsPng(fullPath);
        
        logger.Info($"Saved {path}");
    }
}
