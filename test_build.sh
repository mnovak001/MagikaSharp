#!/usr/bin/env bash
set -euo pipefail

# Configuration
ORT_VERSION="1.22.0"
ROOT=$(dirname "$(realpath "$0")")
FFI_DIR="$ROOT/rust/magika_ffi"
ORT_DIR="$FFI_DIR/ort"

echo "=== Preparing ORT directory ==="
mkdir -p "$ORT_DIR"
cd "$ORT_DIR"

if [[ ! -d "onnxruntime-linux-x64-$ORT_VERSION" ]]; then
    echo "=== Downloading ONNX Runtime $ORT_VERSION ==="
    wget -q "https://github.com/microsoft/onnxruntime/releases/download/v${ORT_VERSION}/onnxruntime-linux-x64-${ORT_VERSION}.tgz"
    tar xf "onnxruntime-linux-x64-${ORT_VERSION}.tgz"
else
    echo "ORT already downloaded."
fi

export ORT_LIB_LOCATION="$ORT_DIR/onnxruntime-linux-x64-$ORT_VERSION/lib"
export ORT_INCLUDE_LOCATION="$ORT_DIR/onnxruntime-linux-x64-$ORT_VERSION/include"

echo "ORT_LIB_LOCATION=$ORT_LIB_LOCATION"
echo "ORT_INCLUDE_LOCATION=$ORT_INCLUDE_LOCATION"

cd "$FFI_DIR"

echo "=== Adding Rust MUSL / Linux targets ==="
rustup target add x86_64-unknown-linux-gnu >/dev/null || true

echo "=== Building Rust FFI library ==="
cargo build --release --target x86_64-unknown-linux-gnu

FFI_LIB="target/x86_64-unknown-linux-gnu/release/libmagika_ffi.so"
if [[ ! -f "$FFI_LIB" ]]; then
    echo "ERROR: libmagika_ffi.so missing"
    exit 1
fi

echo "Built: $FFI_LIB"

echo "=== Copying native libraries to .NET runtimes folder ==="

DEST="$ROOT/dotnet/MagikaSharp/runtimes/linux-x64/native"
mkdir -p "$DEST"

echo "Copying FFI..."
cp "$FFI_LIB" "$DEST/"

echo "Copying ONNX Runtime libraries..."
cp "$ORT_LIB_LOCATION"/libonnxruntime.so* "$DEST/"

echo "=== DONE ==="
