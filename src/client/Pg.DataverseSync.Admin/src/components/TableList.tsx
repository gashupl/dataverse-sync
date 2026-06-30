import type { Table } from '../domain/model/Table'

interface TableListProps {
  tables: Table[]
  loading: boolean
  error: string | null
}

export function TableList({ tables, loading, error }: TableListProps) {
  if (loading) {
    return <div>Loading tables...</div>
  }

  return (
    <div>
      {error && (
        <div style={{ color: 'red', margin: '10px 0' }}>Error: {error}</div>
      )}

      <h3>Tables ({tables.length})</h3>
      <div style={{ maxHeight: '400px', overflow: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
          <thead>
            <tr>
              <th style={{ borderBottom: '1px solid #ccc', padding: '6px 12px' }}>Name</th>
              <th style={{ borderBottom: '1px solid #ccc', padding: '6px 12px' }}>Schema Name</th>
              <th style={{ borderBottom: '1px solid #ccc', padding: '6px 12px' }}>Synchronized</th>
            </tr>
          </thead>
          <tbody>
            {tables.map((table) => (
              <tr key={table.SchemaName}>
                <td style={{ padding: '4px 12px' }}>{table.Name}</td>
                <td style={{ padding: '4px 12px' }}>{table.SchemaName}</td>
                <td style={{ padding: '4px 12px' }}>{table.IsSynchronized ? 'Yes' : 'No'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
