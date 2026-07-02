import './App.css'
import { useTables } from './hooks/useTables'
import { useSaveTables } from './hooks/useSaveTables'
import { TableList } from './components/TableList'

function App() {
  const { tables, loading, error } = useTables()
  const { save, saving, error: saveError } = useSaveTables()

  return (
    <>
      <div>
        <TableList
          tables={tables}
          loading={loading}
          error={error ?? saveError}
          onSave={save}
        />
        {saving && <div>Saving...</div>}
      </div>
      <div className="footer">
        <p>DataverseSync Admin (Modern version v.0.03)</p>
      </div>

    </>
  )
}

export default App
