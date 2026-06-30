import './App.css'
import { useTables } from './hooks/useTables'
import { TableList } from './components/TableList'

function App() {
  const { tables, loading, error } = useTables()

  return (
    <>
      <h1>DataverseSync Admin (Modern version v.0.02)</h1>
      <TableList tables={tables} loading={loading} error={error} />
    </>
  )
}

export default App
