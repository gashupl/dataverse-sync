import { expect, test, describe } from 'vitest'
import { TableService } from '../../src/domain/TableService'
import type { IOperationResult } from '@microsoft/power-apps/data'
import type { Pg_synctables } from '../../src/generated/models/Pg_synctablesModel'

function makeUnsyncResult(tables: { Name: string; SchemaName: string }[]): IOperationResult<Record<string, unknown>> {
  return { success: true, data: { alltables: JSON.stringify(tables) } } as IOperationResult<Record<string, unknown>>
}

function makeSyncResult(records: Partial<Pg_synctables>[]): IOperationResult<Pg_synctables[]> {
  return { success: true, data: records as Pg_synctables[] } as IOperationResult<Pg_synctables[]>
}

const emptyUnsync = makeUnsyncResult([])
const emptySync = makeSyncResult([])

describe('TableService.createList', () => {
  test('returns empty array when both results are empty', () => {
    const result = TableService.createList(emptyUnsync, emptySync)
    expect(result).toEqual([])
  })

  test('maps unsynchronized tables with IsSynchronized false', () => {
    const unsync = makeUnsyncResult([
      { Name: 'Account', SchemaName: 'account' },
      { Name: 'Contact', SchemaName: 'contact' },
    ])

    const result = TableService.createList(unsync, emptySync)

    expect(result).toEqual([
      { Name: 'Account', SchemaName: 'account', IsSynchronized: false },
      { Name: 'Contact', SchemaName: 'contact', IsSynchronized: false },
    ])
  })

  test('marks tables as synchronized when SchemaName matches a sync record', () => {
    const unsync = makeUnsyncResult([
      { Name: 'Opportunity', SchemaName: 'opportunity' },
      { Name: 'Lead', SchemaName: 'lead' },
    ])
    const sync = makeSyncResult([
      { pg_name: 'opportunity' },
      { pg_name: 'lead' },
    ])

    const result = TableService.createList(unsync, sync)

    expect(result).toEqual([
      { Name: 'Opportunity', SchemaName: 'opportunity', IsSynchronized: true },
      { Name: 'Lead', SchemaName: 'lead', IsSynchronized: true },
    ])
  })

  test('sets IsSynchronized correctly for mixed tables and sorts synchronized first', () => {
    const unsync = makeUnsyncResult([
      { Name: 'Account', SchemaName: 'account' },
      { Name: 'Opportunity', SchemaName: 'opportunity' },
    ])
    const sync = makeSyncResult([{ pg_name: 'opportunity' }])

    const result = TableService.createList(unsync, sync)

    expect(result[0].IsSynchronized).toBe(true)
    expect(result[1].IsSynchronized).toBe(false)
  })

  test('returns only tables from the all-tables result', () => {
    const unsync = makeUnsyncResult([
      { Name: 'Account', SchemaName: 'account' },
      { Name: 'Contact', SchemaName: 'contact' },
    ])
    const sync = makeSyncResult([
      { pg_name: 'opportunity' },
    ])

    const result = TableService.createList(unsync, sync)

    expect(result).toHaveLength(2)
  })

  test('handles null data in unsynchronized result', () => {
    const nullUnsync = { success: true, data: null } as unknown as IOperationResult<Record<string, unknown>>
    const sync = makeSyncResult([{ pg_name: 'opportunity' }])

    const result = TableService.createList(nullUnsync, sync)

    expect(result).toEqual([])
  })

  test('handles missing tables property in unsynchronized result', () => {
    const noTablesUnsync = { success: true, data: {} } as IOperationResult<Record<string, unknown>>

    const result = TableService.createList(noTablesUnsync, emptySync)

    expect(result).toEqual([])
  })

  test('handles null data in sync result', () => {
    const unsync = makeUnsyncResult([{ Name: 'Account', SchemaName: 'account' }])
    const nullSync = { success: true, data: null } as unknown as IOperationResult<Pg_synctables[]>

    const result = TableService.createList(unsync, nullSync)

    expect(result).toEqual([
      { Name: 'Account', SchemaName: 'account', IsSynchronized: false },
    ])
  })
})
