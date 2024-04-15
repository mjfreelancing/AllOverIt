#  Version 8.0.0
## XX XXX 2024

### PENDING / IN-PROGRESS
* Enable nullable references
* Change properties and arguments from IReadOnlyCollection<T> to T[], where applicable
* Add 'required' to properties, where applicable - including NetStandard 2.1 support
* Consider removing extraneous null guard checks in extension methods


### Applicable to all packages
* Dopped support for NET 6 and NET 7


### AllOverIt
* EnrichedEnum previously threw ArgumentNullException when comparing to a null


### AllOverIt.Aws.Cdk.AppSync
* [Breaking Change] Complete rework of how resolvers and request/response handlers are configured. Datasources can
  now be shared and resolvers now support VTL and JS Code based request/response handlers.


### AllOverIt.Cryptography
* Refactored RsaAesHybridEncryptor to use CryptographicOperations.HashData() for NET 9


---
