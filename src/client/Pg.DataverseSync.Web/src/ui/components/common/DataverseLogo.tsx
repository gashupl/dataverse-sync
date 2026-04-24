/**
 * DataverseSync Logo Component
 * Reusable SVG logo for the application
 */
export function DataverseLogo() {
  return (
    <svg 
      width="32" 
      height="32" 
      viewBox="0 0 32 32" 
      fill="currentColor" 
      xmlns="http://www.w3.org/2000/svg"
      className="dataverse-logo"
      aria-label="DataverseSync Logo"
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
  );
}