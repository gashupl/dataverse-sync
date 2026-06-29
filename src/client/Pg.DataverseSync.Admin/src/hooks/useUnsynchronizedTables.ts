import { useState, useCallback } from 'react'
import { Pg_getunsynchronizedtablesService } from '../generated/services/Pg_getunsynchronizedtablesService'

export interface UnsynchronizedTable {
  Name: string
  SchemaName: string
}

interface UseUnsynchronizedTablesResult {
  tables: UnsynchronizedTable[]
  loading: boolean
  error: string | null
  fetch: () => Promise<void>
}

export function useUnsynchronizedTables(): UseUnsynchronizedTablesResult {
  const [tables, setTables] = useState<UnsynchronizedTable[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const fetch = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const result = await Pg_getunsynchronizedtablesService.pg_getunsynchronizedtables()
      const raw = result.data as { tables?: string } | null
      const parsed: UnsynchronizedTable[] = raw?.tables ? JSON.parse(raw.tables) : []
      setTables(parsed)
    } catch (err) {
      setError('Failed to retrieve unsynchronized tables: ' + (err as Error).message)
    } finally {
      setLoading(false)
    }
  }, [])

  return { tables, loading, error, fetch }
}
