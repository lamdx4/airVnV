import React from 'react';
import { HostPropertyDashboard } from '@/features/properties/components/HostPropertyDashboard';

const HostDashboard: React.FC = () => {
  return (
    <div className="container mx-auto py-10">
      <HostPropertyDashboard />
    </div>
  );
};

export default HostDashboard;
