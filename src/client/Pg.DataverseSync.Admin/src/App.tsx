import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { useSyncTables } from './hooks/useSyncTables'
import { useUnsynchronizedTables } from './hooks/useUnsynchronizedTables'
import { SyncTableList } from './components/SyncTableList'
import { UnsynchronizedTableList } from './components/UnsynchronizedTableList'

function App() {
  const [count, setCount] = useState(0)
  const syncTablesHook = useSyncTables()
  const unsyncTablesHook = useUnsynchronizedTables()

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>DataverseSync Admin (Modern version v.0.01)</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>

      <SyncTableList
        syncTables={syncTablesHook.syncTables}
        loading={syncTablesHook.loading}
        error={syncTablesHook.error}
        onReload={syncTablesHook.fetch}
      />

      <UnsynchronizedTableList
        tables={unsyncTablesHook.tables}
        loading={unsyncTablesHook.loading}
        error={unsyncTablesHook.error}
        onFetch={unsyncTablesHook.fetch}
      />
    </>
  )
}

export default App
