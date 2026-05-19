import React from 'react';

export const Loading03Icon: React.FC<{ className?: string }> = ({ className }) => (
  <svg 
    className={className} 
    width="24" 
    height="24" 
    viewBox="0 0 24 24" 
    fill="none" 
    stroke="currentColor" 
    strokeWidth="2" 
    strokeLinecap="round" 
    strokeLinejoin="round"
  >
    <path d="M21 12a9 9 0 1 1-6.219-8.56" />
  </svg>
);

export const CheckDoubleIcon: React.FC<{ className?: string }> = ({ className }) => (
  <svg 
    className={className} 
    width="16" 
    height="16" 
    viewBox="0 0 24 24" 
    fill="none" 
    stroke="currentColor" 
    strokeWidth="2" 
    strokeLinecap="round" 
    strokeLinejoin="round"
  >
    <path d="M7 13l3 3 7-7" />
    <path d="M12 13l3 3 7-7" />
  </svg>
);
