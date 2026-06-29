import type { Pg_synctables } from '../generated/models/Pg_synctablesModel'

interface SyncTableListProps {
  syncTables: Pg_synctables[]
  loading: boolean
  error: string | null
  onReload: () => void
}

export function SyncTableList({ syncTables, loading, error, onReload }: SyncTableListProps) {
  return (
    <div>
      <button onClick={onReload} disabled={loading}>
        {loading ? 'Loading...' : 'Reload Sync Tables'}
      </button>

      {error && (
        <div style={{ color: 'red', margin: '10px 0' }}>Error: {error}</div>
      )}

      {syncTables.length > 0 && (
        <div style={{ margin: '20px 0' }}>
          <h3>Sync Tables ({syncTables.length})</h3>
          <ul style={{ textAlign: 'left', maxHeight: '200px', overflow: 'auto' }}>
            {syncTables.map((table, index) => (
              <li key={table.pg_synctableid || index}>
                {table.pg_name || 'No name'}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  )
}
