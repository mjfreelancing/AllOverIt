#  Version 8.0.0
## XX XXX 2024

### Applicable to all packages
* Dopped support for NET 6 and NET 7
* Enabled Nullable references


### AllOverIt
* EnrichedEnum previously threw ArgumentNullException when comparing to a null


### AllOverIt.Aws.Cdk.AppSync
* [Breaking Change] Complete rework of how resolvers and request/response handlers are configured. Datasources can
  now be shared and resolvers now support VTL and JS Code based request/response handlers.


### AllOverIt.Cryptography
* Refactored RsaAesHybridEncryptor to use CryptographicOperations.HashData() for NET 9


---
