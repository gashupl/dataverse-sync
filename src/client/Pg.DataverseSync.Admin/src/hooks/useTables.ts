import { useState, useEffect } from 'react'
import { Pg_getunsynchronizedtablesService } from '../generated/services/Pg_getunsynchronizedtablesService'
import { Pg_synctablesService } from '../generated/services/Pg_synctablesService'
import { TableService } from '../domain/TableService'
import type { Table } from '../domain/model/Table'

interface UseTablesResult {
  tables: Table[]
  loading: boolean
  error: string | null
}

export function useTables(): UseTablesResult {
  const [tables, setTables] = useState<Table[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    async function load() {
      setLoading(true)
      setError(null)
      try {
        const [unsynchronizedResult, syncTablesResult] = await Promise.all([
          Pg_getunsynchronizedtablesService.pg_getunsynchronizedtables(),
          Pg_synctablesService.getAll(),
        ])
        setTables(TableService.createList(unsynchronizedResult, syncTablesResult))
      } catch (err) {
        setError('Failed to load tables: ' + (err as Error).message)
      } finally {
        setLoading(false)
      }
    }

    load()
  }, [])

  return { tables, loading, error }
}
