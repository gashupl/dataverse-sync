import type { UnsynchronizedTable } from '../hooks/useUnsynchronizedTables'

interface UnsynchronizedTableListProps {
  tables: UnsynchronizedTable[]
  loading: boolean
  error: string | null
  onFetch: () => void
}

export function UnsynchronizedTableList({ tables, loading, error, onFetch }: UnsynchronizedTableListProps) {
  return (
    <div>
      <button onClick={onFetch} disabled={loading}>
        {loading ? 'Loading...' : 'Get Unsynchronized Tables'}
      </button>

      {error && (
        <div style={{ color: 'red', margin: '10px 0' }}>Error: {error}</div>
      )}

      {tables.length > 0 && (
        <div style={{ margin: '20px 0' }}>
          <h3>Unsynchronized Tables ({tables.length})</h3>
          <div style={{ maxHeight: '300px', overflow: 'auto' }}>
            <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
              <thead>
                <tr>
                  <th style={{ borderBottom: '1px solid #ccc', padding: '6px 12px' }}>Name</th>
                  <th style={{ borderBottom: '1px solid #ccc', padding: '6px 12px' }}>Schema Name</th>
                </tr>
              </thead>
              <tbody>
                {tables.map((table) => (
                  <tr key={table.SchemaName}>
                    <td style={{ padding: '4px 12px' }}>{table.Name}</td>
                    <td style={{ padding: '4px 12px' }}>{table.SchemaName}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  )
}
