import type { IOperationResult } from '@microsoft/power-apps/data'
import type { Pg_synctables } from '../generated/models/Pg_synctablesModel'
import type { Table } from './model/Table'

interface UnsynchronizedTableRaw {
  Name: string
  SchemaName: string
}

export class TableService {
  public static createList(
    unsynchronizedResult: IOperationResult<Record<string, unknown>>,
    syncTablesResult: IOperationResult<Pg_synctables[]>
  ): Table[] {
    const raw = unsynchronizedResult.data as { tables?: string } | null
    const unsynchronized: UnsynchronizedTableRaw[] = raw?.tables ? JSON.parse(raw.tables) : []

    const unsynchronizedTables: Table[] = unsynchronized.map((t) => ({
      Name: t.Name,
      SchemaName: t.SchemaName,
      IsSynchronized: false,
    }))

    const synchronizedTables: Table[] = (syncTablesResult.data ?? []).map((t) => ({
      Name: t.pg_name,
      SchemaName: t.pg_name,
      IsSynchronized: true,
    }))

    return [...synchronizedTables, ...unsynchronizedTables]
  }
}
