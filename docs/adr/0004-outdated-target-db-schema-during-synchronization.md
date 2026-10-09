# ADR-0004: Outdated target database schema during synchronization

- **Date:** 2026-10-09

## Context
By default target database is checked and synchronized every 3 minutes. 
Outdated target database schema during synchronization may result in synchronization errors and dead-letters messages inside used Azure Service Bus

## Decision
1. Schema update is based on calculated checksums representing Dataverse schema state and stored inside target DB to avoid unecessary ALTER table command execution. 
2. Information about last schema synchronization should be available in the management Power Apps app. There is also tool allowing for comparing checksums calculated in on-demand mode and force target database schema synchronization. 
3. There is also option in the Management App perform full recreation of target table including performing full data reload into it. 
4. All the Dataverse schema updated should be performed during administration window when updating record is not possible. Database synchronization force is recommended after such an operation. However it is not always possible due to auto-update of system or Microsoft apps. 
5. In case of synchronization error notification is visible in Dataverse environment 

## Consequences
We do not check target database schema before every INSERT/UPDATE due to performance purposes, extending execution time and possible issues with concurrent execution of Azure Functions. Possible situations when target schema is out of date are rare and it is acceptable risk which may be addressed manually. 