import { useState, useEffect } from 'react'
import './TableList.css'
import type { Table } from '../domain/model/Table'

interface TableListProps {
  tables: Table[]
  loading: boolean
  error: string | null
}

export function TableList({ tables: tablesProp, loading, error }: TableListProps) {
  const [tables, setTables] = useState<Table[]>(tablesProp)

  useEffect(() => {
    setTables(tablesProp)
  }, [tablesProp])

  function handleToggle(schemaName: string, checked: boolean) {
    setTables((prev) =>
      prev.map((t) => t.SchemaName === schemaName ? { ...t, IsSynchronized: checked } : t)
    )
  }

  if (loading) {
    return <div>Loading tables...</div>
  }

  return (
    <div style={{ textAlign: 'left' }}>
      {error && (
        <div className="table-list-error">Error: {error}</div>
      )}
      <div>
        <button onClick={() => console.log('Hello Vite + React!')}>
          Update Synchronization Settings
        </button>
      </div>
      <h4>Tables synchronization settings ({tables.length})</h4>
      <div className="table-list-scroll">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Schema Name</th>
              <th>Synchronized</th>
            </tr>
          </thead>
          <tbody>
            {tables.map((table) => (
              <tr key={table.SchemaName}>
                <td>{table.Name}</td>
                <td>{table.SchemaName}</td>
                <td>
                  <input
                    type="checkbox"
                    checked={table.IsSynchronized}
                    onChange={(e) => handleToggle(table.SchemaName, e.target.checked)}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
