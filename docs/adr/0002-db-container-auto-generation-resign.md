# ADR-0002: Database container auto generation resign

- **Date:** 2026-03-30

## Context
SQL Server database in docker container used for Pg.DataverseSync.Api in the development environment

## Decision
We do not auto-initialize docker container by Aspire runtime because of some missing configuration options (container name, etc.), complicated configuration and problems with docker runtime management by Apire framework. 

## Consequences
The SQL Server container and database inside must be created on the development machine manually and the valid connection string needs to be set up in the appsettings.development.json file. 
