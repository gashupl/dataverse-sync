import './App.css'
import { useTables } from './hooks/useTables'
import { useSaveTables } from './hooks/useSaveTables'
import { TableList } from './components/TableList'

function App() {
  const { tables, loading, error, refresh } = useTables();
  const { save, saving, error: saveError } = useSaveTables();

  const handleSave = async (pendingChanges: Map<string, boolean>) => {
    await save(pendingChanges);
    await refresh();
  };

  return (
    <>
      <div>
        <TableList
          tables={tables}
          loading={loading}
          error={error ?? saveError}
          onSave={handleSave}
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
