import { expect, test, describe } from 'vitest'
import { TableService } from '../../src/domain/TableService'
import type { IOperationResult } from '@microsoft/power-apps/data'
import type { Pg_synctables } from '../../src/generated/models/Pg_synctablesModel'

function makeUnsyncResult(tables: { Name: string; SchemaName: string }[]): IOperationResult<Record<string, unknown>> {
  return { success: true, data: { tables: JSON.stringify(tables) } } as IOperationResult<Record<string, unknown>>
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

  test('maps synchronized tables with IsSynchronized true', () => {
    const sync = makeSyncResult([
      { pg_name: 'opportunity' },
      { pg_name: 'lead' },
    ])

    const result = TableService.createList(emptyUnsync, sync)

    expect(result).toEqual([
      { Name: 'opportunity', SchemaName: 'opportunity', IsSynchronized: true },
      { Name: 'lead', SchemaName: 'lead', IsSynchronized: true },
    ])
  })

  test('synchronized tables appear before unsynchronized tables', () => {
    const unsync = makeUnsyncResult([{ Name: 'Account', SchemaName: 'account' }])
    const sync = makeSyncResult([{ pg_name: 'opportunity' }])

    const result = TableService.createList(unsync, sync)

    expect(result[0].IsSynchronized).toBe(true)
    expect(result[1].IsSynchronized).toBe(false)
  })

  test('combines both results into a single list', () => {
    const unsync = makeUnsyncResult([
      { Name: 'Account', SchemaName: 'account' },
      { Name: 'Contact', SchemaName: 'contact' },
    ])
    const sync = makeSyncResult([
      { pg_name: 'opportunity' },
    ])

    const result = TableService.createList(unsync, sync)

    expect(result).toHaveLength(3)
  })

  test('handles null data in unsynchronized result', () => {
    const nullUnsync = { success: true, data: null } as unknown as IOperationResult<Record<string, unknown>>
    const sync = makeSyncResult([{ pg_name: 'opportunity' }])

    const result = TableService.createList(nullUnsync, sync)

    expect(result).toEqual([
      { Name: 'opportunity', SchemaName: 'opportunity', IsSynchronized: true },
    ])
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
