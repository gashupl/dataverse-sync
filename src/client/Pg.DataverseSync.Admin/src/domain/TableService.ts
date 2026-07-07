import type { IOperationResult } from '@microsoft/power-apps/data'
import type { Pg_synctables } from '../generated/models/Pg_synctablesModel'
import type { Table } from './model/Table'

interface TableRaw {
  Name: string
  SchemaName: string
}

export class TableService {

  public static createList(
    allTablesResult: IOperationResult<Record<string, unknown>>,
    syncTablesResult: IOperationResult<Pg_synctables[]>
  ): Table[] {
    
    
    const raw = allTablesResult.data as { alltables?: string } | null; 
    console.log(raw); 
    const all: TableRaw[] = raw?.alltables ? JSON.parse(raw.alltables) : []; 

    console.log(all); 
    const synchronizedTables: Table[] = (syncTablesResult.data ?? []).map((t) => ({
      Name: t.pg_name,
      SchemaName: t.pg_name,
      IsSynchronized: true,
    })); 

    console.log(synchronizedTables); 

    const allTables: Table[] = all.map((t) => ({
      Name: t.Name,
      SchemaName: t.SchemaName,
      IsSynchronized: synchronizedTables.some((s) => s.SchemaName === t.SchemaName),
    })); 



    return [...allTables].sort((a, b) => Number(b.IsSynchronized) - Number(a.IsSynchronized)); 
  }
}
