import './App.css'
import { useTables } from './hooks/useTables'
import { TableList } from './components/TableList'

function App() {
  const { tables, loading, error } = useTables()

  return (
    <>
      <div>
        <TableList tables={tables} loading={loading} error={error} />
      </div>
      <div className="footer">
        <p>DataverseSync Admin (Modern version v.0.03)</p>
      </div>

    </>
  )
}

export default App
