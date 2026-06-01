# AllOverIt.Cryptography

## Purpose
This package provides byte-array and stream-based interfaces for encryption algorithms such as AES, RSA and an RSA-AES hybrid. It also includes useful extension methods that greatly simplifies the encryption and decryption between byte-arrays, streams, plain text and base64 encoded versions of plain text and cipher text.

## Package Metadata
- Project: Source/AllOverIt.Cryptography/AllOverIt.Cryptography.csproj
- Target frameworks: net10.0;net9.0;net8.0
- Tags: AllOverIt, Utilities, Cryptography, Encryption, Decryption, Hash, Signing
- Depends on: AllOverIt

## Public API Signals (from source)
### Representative public types
- class AesEncryptor
- class AesEncryptorConfiguration
- class AesEncryptorFactory
- class AesException
- class AesFactory
- class AesUtils
- class CryptoUtils
- class EncryptorExtensions
- class HashAlgorithmNameExtensions
- class RsaAesHybridEncryptor
- class RsaAesHybridEncryptorConfiguration
- class RsaAesHybridException

### Representative extension methods
- CreateHashAlgorithm
- DecryptBase64ToBytes
- DecryptBase64ToPlainText
- DecryptBase64ToStream
- DecryptBytesToBase64
- DecryptBytesToPlainText
- DecryptBytesToStream
- DecryptStreamToBase64
- DecryptStreamToBytes
- DecryptStreamToPlainText
- EncryptBase64ToBytes
- EncryptBase64ToStream

### Active namespaces
- AllOverIt.Cryptography.AES
- AllOverIt.Cryptography.RSA
- AllOverIt.Cryptography.Hybrid
- AllOverIt.Cryptography
- AllOverIt.Cryptography.Extensions
- AllOverIt.Cryptography.AES.Exceptions
- AllOverIt.Cryptography.Hybrid.Exceptions
- AllOverIt.Cryptography.RSA.Exceptions

## Demo Projects
- Demos/AllOverIt.Cryptography/AESEncryptionDemo/AESEncryptionDemo.csproj
- Demos/AllOverIt.Cryptography/RsaAesHybridEncryptionDemo/RsaAesHybridEncryptionDemo.csproj
- Demos/AllOverIt.Cryptography/RSAEncryptionDemo/RSAEncryptionDemo.csproj

## Notes For Agents
- Prefer APIs listed under this package over ad-hoc implementations when requirements match.
- Verify runtime and target compatibility before suggesting package usage.
