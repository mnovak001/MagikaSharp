#!/usr/bin/env bash
set -euo pipefail

#############################################
# CONFIGURATION
#############################################

ORT_VERSION="1.22.0"
ROOT="$(dirname "$(realpath "$0")")"

FFI_DIR="$ROOT/rust/magika_ffi"
ORT_DIR="$FFI_DIR/ort"
ORT_FOLDER="onnxruntime-linux-x64-$ORT_VERSION"

# EXACT SAME OUTPUT STRUCTURE AS GITHUB ACTIONS
ARTIFACT_ROOT="$ROOT/linux-x64-artifact"
RUST_OUT_DIR="$ARTIFACT_ROOT/release"
ORT_LIB_OUT_DIR="$ARTIFACT_ROOT/lib"

DOTNET_PROJECT="$ROOT/dotnet/MagikaSharp"

#############################################
# DOWNLOAD ORT (FULL GITHUB-ACTIONS REPLICA)
#############################################

echo "=== Preparing ONNX Runtime directory ==="
mkdir -p "$ORT_DIR"
cd "$ORT_DIR"

if [[ ! -d "$ORT_FOLDER" ]]; then
    echo "=== Downloading ONNX Runtime $ORT_VERSION ==="
    wget -q "https://github.com/microsoft/onnxruntime/releases/download/v${ORT_VERSION}/onnxruntime-linux-x64-${ORT_VERSION}.tgz"
    tar xf "onnxruntime-linux-x64-${ORT_VERSION}.tgz"
else
    echo "ORT already downloaded."
fi

export ORT_LIB_LOCATION="$ORT_DIR/$ORT_FOLDER/lib"
export ORT_INCLUDE_LOCATION="$ORT_DIR/$ORT_FOLDER/include"

echo "ORT_LIB_LOCATION=$ORT_LIB_LOCATION"
echo "ORT_INCLUDE_LOCATION=$ORT_INCLUDE_LOCATION"

#############################################
# BUILD RUST FFI
#############################################

echo "=== Adding Rust target ==="
rustup target add x86_64-unknown-linux-gnu || true

echo "=== Building Rust FFI ==="
cd "$FFI_DIR"
cargo build --release --target x86_64-unknown-linux-gnu

#############################################
# COPY ARTIFACTS (MATCH GITHUB ACTIONS EXACTLY)
#############################################

echo "=== Preparing artifact directory ==="

rm -rf "$ARTIFACT_ROOT"
mkdir -p "$RUST_OUT_DIR"
mkdir -p "$ORT_LIB_OUT_DIR"

echo "=== Copying Rust release/ directory ==="
cp -r "$FFI_DIR/target/x86_64-unknown-linux-gnu/release/"* "$RUST_OUT_DIR/"

echo "=== Copying ORT lib/ directory ==="
cp -r "$ORT_LIB_LOCATION/"* "$ORT_LIB_OUT_DIR/"

#############################################
# COPY LIBRARIES INTO .NET RUNTIMES (JUST LIKE publish-nuget)
#############################################

DOTNET_NATIVE="$DOTNET_PROJECT/runtimes/linux-x64/native"

echo "=== Syncing native libs into .NET project ==="
rm -rf "$DOTNET_NATIVE"
mkdir -p "$DOTNET_NATIVE"

# Rust FFI library
cp "$RUST_OUT_DIR/libmagika_ffi.so" "$DOTNET_NATIVE/"

# All ORT .so files
cp "$ORT_LIB_OUT_DIR/"libonnxruntime.so* "$DOTNET_NATIVE/"

echo "=== .NET Native Runtime Folder ==="
ls -l "$DOTNET_NATIVE"

#############################################
# BUILD .NET PROJECT (EXACT YAML REPLICA)
#############################################

if ! command -v dotnet >/dev/null 2>&1; then
    echo "ERROR: dotnet SDK not installed"
    exit 1
fi

echo "=== Building .NET project ==="
cd "$DOTNET_PROJECT"
dotnet build -c Release

echo
echo "=== DONE ==="
echo "Linux artifacts:"
tree "$ARTIFACT_ROOT"
echo
echo "Native .NET runtime:"
tree "$DOTNET_NATIVE"
