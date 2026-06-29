import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { Pg_synctablesService } from './generated/services/Pg_synctablesService';
import type { Pg_synctables } from './generated/models/Pg_synctablesModel';
import { Pg_getunsynchronizedtablesService } from './generated';

function App() {
  const [count, setCount] = useState(0)
  const [pgSynctables, setPgSynctables] = useState<Pg_synctables[]>([])
  const [dvLoading, setDvLoading] = useState(false)
  const [dvError, setDvError] = useState<string | null>(null)


  const getDataverseData = async () => {
    try {
      console.log('Executing getDataverseData...');
      setDvLoading(true);
      setDvError(null); 
      console.log('Retrieving Pg_synctables from Dataverse...');
      const result = await Pg_synctablesService.getAll();
      console.log('Dataverse response:', result);
      if (result.data) {
        console.log('Pg_synctables retrieved:', result.data);
        setPgSynctables(result.data);
      } else {
        console.log('No Pg_synctables found.');
        setPgSynctables([])
      }
    } catch (err) {
      const errorMessage = 'Failed to retrieve Pg_synctables: ' + (err as Error).message
      console.error(errorMessage, err);
      setDvError(errorMessage)
    } finally {
      setDvLoading(false)
    }
  }

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
        <button onClick={getDataverseData} disabled={dvLoading} style={{ marginLeft: '10px' }}>
          {dvLoading ? 'Loading...' : 'Reload Pg_synctables'}
        </button>
        <button onClick={async () => {
          const result = await Pg_getunsynchronizedtablesService.pg_getunsynchronizedtables(); 
          console.log(result);
        }}>New Button</button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>

      {dvError && (
        <div style={{ color: 'red', margin: '10px 0' }}>
          Error: {dvError}
        </div>
      )}

      {pgSynctables.length > 0 && (
        <div style={{ margin: '20px 0' }}>
          <h3>Dataverse Pg_synctables ({pgSynctables.length})</h3>
          <ul style={{ textAlign: 'left', maxHeight: '200px', overflow: 'auto' }}>
            {pgSynctables.map((pgSynctable, index) => (
              <li key={pgSynctable.pg_synctableid || index}>
                {pgSynctable.pg_name || 'No name'}
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p> */}
    </>
  )
}

export default App
