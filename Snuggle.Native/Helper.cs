﻿using System.Reflection;
using System.Runtime.InteropServices;

namespace Snuggle.Native;

public static class Helper {
    private static bool Loaded { get; set; } 

    private static IntPtr Resolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        string rid, ext, prefix;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            rid = "linux";
            ext = "so";
            prefix = "lib";
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            rid = "osx";
            ext = "dylib";
            prefix = "lib";
        } else {
            rid = "win";
            ext = "dll";
            prefix = string.Empty;
        }

        var libName = $"{prefix}{libraryName}.{ext}";
        var local = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var libPath = Path.Combine(local, libName);
        if (File.Exists(libPath)) {
            return NativeLibrary.Load(libPath);
        }

        libPath = Path.Combine(local, "runtimes", rid, "native", libName);
        if (File.Exists(libPath)) {
            return NativeLibrary.Load(libPath);
        }

        libPath = Path.Combine(local, "runtimes", $"{rid}-{(Environment.Is64BitProcess ? "x64" : "x86")}", "native", libName);
        return File.Exists(libPath) ? NativeLibrary.Load(libPath) : IntPtr.Zero;
    }

    public static void Register() {
        if (Loaded) {
            return;
        }

        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), Resolver);
        Loaded = true;
    }
}
