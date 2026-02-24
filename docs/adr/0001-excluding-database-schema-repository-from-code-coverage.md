# ADR-0001: Excluding database schema repository from code coverage

- **Date:** 2026-02-24

## Context
Unit testing of DatabasesettSchemaRepository class requires ing up LocalDb database localy and dynamically on GitHub Actions agent

## Decision
We do not write unit tests for this class

## Consequences
Undetected bugs are more likely to occur, since code changes are not verified at the class level.