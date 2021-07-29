﻿using System;
using System.IO;
using Equilibrium.Meta.Interfaces;
using JetBrains.Annotations;

namespace Equilibrium.IO {
    [PublicAPI]
    public class FileStreamHandler : IFileHandler {
        public static Lazy<FileStreamHandler> Instance { get; } = new();

        public Stream OpenFile(object tag) {
            string path = tag switch {
                FileInfo fi => fi.FullName,
                string str => str,
                _ => throw new InvalidOperationException(),
            };

            return File.OpenRead(path);
        }

        public object GetTag(object baseTag, object parent) => baseTag;

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
