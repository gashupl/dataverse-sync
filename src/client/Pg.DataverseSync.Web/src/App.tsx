import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'

// DataverseSync Logo Component
const DataverseLogo = () => (
  <svg 
    width="32" 
    height="32" 
    viewBox="0 0 32 32" 
    fill="currentColor" 
    xmlns="http://www.w3.org/2000/svg"
    className="dataverse-logo"
  >
    {/* Central hub */}
    <circle cx="16" cy="16" r="4" fill="currentColor" className="logo-hub"/>
    
    {/* Data nodes */}
    <circle cx="6" cy="8" r="2.5" fill="currentColor" className="logo-node" opacity="0.8"/>
    <circle cx="26" cy="8" r="2.5" fill="currentColor" className="logo-node" opacity="0.8"/>
    <circle cx="6" cy="24" r="2.5" fill="currentColor" className="logo-node" opacity="0.8"/>
    <circle cx="26" cy="24" r="2.5" fill="currentColor" className="logo-node" opacity="0.8"/>
    
    {/* Data flow lines with arrows */}
    <path d="M8.5 9.5 L12.5 13.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" className="logo-flow" opacity="0.7"/>
    <path d="M23.5 9.5 L19.5 13.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" className="logo-flow" opacity="0.7"/>
    <path d="M8.5 22.5 L12.5 18.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" className="logo-flow" opacity="0.7"/>
    <path d="M23.5 22.5 L19.5 18.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" className="logo-flow" opacity="0.7"/>
    
    {/* Arrow heads */}
    <polygon points="13,12 15,14 13,16" fill="currentColor" className="logo-arrow" opacity="0.6"/>
    <polygon points="19,12 17,14 19,16" fill="currentColor" className="logo-arrow" opacity="0.6"/>
    <polygon points="13,20 15,18 13,16" fill="currentColor" className="logo-arrow" opacity="0.6"/>
    <polygon points="19,20 17,18 19,16" fill="currentColor" className="logo-arrow" opacity="0.6"/>
    
    {/* Sync orbits */}
    <circle cx="16" cy="16" r="8" fill="none" stroke="currentColor" strokeWidth="1" strokeDasharray="2,3" className="logo-orbit" opacity="0.3"/>
  </svg>
)

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <header className="app-header">
        <div className="header-content">
          <div className="logo-title-container">
            <DataverseLogo />
            <h1 className="app-title">DATAVERSE SYNC</h1>
          </div>
          <div className="auth-buttons">
            <button className="auth-btn login-btn">Login</button>
            <button className="auth-btn register-btn">Register</button>
          </div>
        </div>
      </header>
      
      <main className="main-content">
        <section id="center">
        <div className="hero">
          <img src={heroImg} className="base" width="170" height="179" alt="" />
          <img src={reactLogo} className="framework" alt="React logo" />
          <img src={viteLogo} className="vite" alt="Vite logo" />
        </div>
        <div>
          <h1>Get started</h1>
          <p>
            Edit <code>src/App.tsx</code> and save to test <code>HMR</code>
          </p>
        </div>
        <button
          className="counter"
          onClick={() => setCount((count) => count + 1)}
        >
          Count is {count}
        </button>
        </section>

        <div className="ticks"></div>

        <section id="next-steps">
        <div id="docs">
          <svg className="icon" role="presentation" aria-hidden="true">
            <use href="/icons.svg#documentation-icon"></use>
          </svg>
          <h2>Documentation</h2>
          <p>Your questions, answered</p>
          <ul>
            <li>
              <a href="https://vite.dev/" target="_blank">
                <img className="logo" src={viteLogo} alt="" />
                Explore Vite
              </a>
            </li>
            <li>
              <a href="https://react.dev/" target="_blank">
                <img className="button-icon" src={reactLogo} alt="" />
                Learn more
              </a>
            </li>
          </ul>
        </div>
        <div id="social">
        </div>
        </section>

        <div className="ticks"></div>
        <section id="spacer"></section>
      </main>
    </>
  )
}

export default App
