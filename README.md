# 📦 **MagikaSharp**

**MagikaSharp** is an **unofficial .NET wrapper** for Google’s **[magika](https://github.com/google/magika)** file type detection system.
It provides a clean and safe C# API on top of a high-performance Rust FFI integration using ONNX Runtime.

MagikaSharp brings Magika’s machine-learning-powered file type identification to the .NET ecosystem, supporting:

* ✔ Windows
* ✔ Linux
* ✔ macOS (Apple Silicon)

---

## ✨ Features

* 🔍 Fast and accurate file type detection
* 📄 Detailed metadata (label, mime, description, extensions, category, text/binary flag)
* ⚡ High-performance Rust backend with native ONNX Runtime
* 🧵 Safe managed API with automatic memory handling
* 📦 Cross-platform native runtime packing (NuGet-friendly)

---

## 📥 Installation

Install from NuGet:

```sh
dotnet add package MagikaSharp
```

---

## 🚀 Quick Example

```csharp
using MagikaSharp;

using var magika = new MagikaSession();

var info = magika.IdentifyFile("example.pdf");

Console.WriteLine($"Label: {info.Label}");
Console.WriteLine($"MIME: {info.MimeType}");
Console.WriteLine($"Group: {info.Group}");
Console.WriteLine($"Description: {info.Description}");
Console.WriteLine("Extensions: " + string.Join(", ", info.Extensions));
Console.WriteLine($"IsText: {info.IsText}");
```

---

## 📚 API Highlights

### **Identify a file**

```csharp
var info = magika.IdentifyFile("file.bin");
```

### **Identify in-memory bytes**

```csharp
var info = magika.IdentifyBytes(data);
```

### Returned info object

```csharp
public sealed class MagikaTypeInfo
{
    public string Label { get; }
    public string MimeType { get; }
    public string Group { get; }
    public string Description { get; }
    public string[] Extensions { get; }
    public bool IsText { get; }
}
```

---

## ⚙️ How It Works

MagikaSharp ships with:

* Native Rust library (`libmagika_ffi`)
* Bundled ONNX Runtime dependencies
* A thin and safe C# P/Invoke layer
* A high-level managed API class (`MagikaSession`)

At runtime, MagikaSharp dynamically loads the correct native library based on the current platform.

---

## 📝 Notes

MagikaSharp is **not affiliated with Google**.
Magika is a Google project released under the Apache 2.0 license.

This library aims to:

* provide reliable .NET bindings
* support Linux/Windows/macOS
* make Magika easy to consume in .NET apps and services


This project includes components from:

- Google Magika (Apache License 2.0)
- ONNX Runtime (MIT License)

MagikaSharp is an independent project and is not affiliated with Google.

---

## 🤝 Contributing

PRs, issue reports, and suggestions are welcome!

---

## 📄 License

- MIT
- Magika itself is Apache 2.0.

