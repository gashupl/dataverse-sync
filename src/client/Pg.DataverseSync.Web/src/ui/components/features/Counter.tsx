import { useState } from 'react';

/**
 * Counter component
 * Example interactive component demonstrating React state
 */
export function Counter() {
  const [count, setCount] = useState(0);

  return (
    <div>
      <h1>Get started</h1>
      <p>
        Edit <code>src/App.tsx</code> and save to test <code>HMR</code>
      </p>
      <button
        className="counter"
        onClick={() => setCount((count) => count + 1)}
      >
        Count is {count}
      </button>
    </div>
  );
}