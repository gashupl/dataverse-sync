import { useState, useEffect } from 'react'
import './TableList.css'
import type { Table } from '../domain/model/Table'

interface TableListProps {
  readonly tables: Table[];
  readonly loading: boolean;
  readonly error: string | null;
  readonly saving: boolean;
  readonly onSave: (pendingChanges: Map<string, boolean>) => void;
}

export function TableList({ tables: tablesProp, loading, error, saving, onSave }: TableListProps) {
  const [tables, setTables] = useState<Table[]>(tablesProp); 
  const [pendingChanges, setPendingChanges] = useState<Map<string, boolean>>(new Map());

  useEffect(() => {
    setTables(tablesProp); 
    setPendingChanges(new Map()); 
  }, [tablesProp])

  function handleToggle(schemaName: string, checked: boolean) {
    setTables((prev) =>
      prev.map((t) => t.SchemaName === schemaName ? { ...t, IsSynchronized: checked } : t)
    ); 

    setPendingChanges(prev => {
      const next = new Map(prev); 
      // Find original value from the incoming prop
      const original = tablesProp.find(t => t.SchemaName === schemaName)?.IsSynchronized;
      if (original === checked) {
        next.delete(schemaName) // change was reverted, no longer pending
      } else {
        next.set(schemaName, checked);
      }
      return next
    }); 
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
        <button
          onClick={() => onSave(pendingChanges)}
          disabled={pendingChanges.size === 0 || saving}>
          Update Synchronization Settings
        </button>
        {saving && <span style={{ marginLeft: '0.75rem' }}>Saving changes...</span>}
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
