import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { ArrowLeft02Icon } from 'hugeicons-react';
import { EditPropertyForm } from '@/features/properties/components/EditPropertyForm';

const EditProperty: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  if (!id) return <div>Invalid Property ID</div>;

  return (
    <div className="max-w-5xl mx-auto py-8 px-4">
      <div className="mb-8 flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button 
            variant="ghost" 
            size="icon" 
            onClick={() => navigate('/host/homes')}
            className="rounded-full hover:bg-slate-100"
          >
            <ArrowLeft02Icon className="h-6 w-6" />
          </Button>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Edit Listing</h1>
            <p className="text-muted-foreground text-sm">Update your property information and settings.</p>
          </div>
        </div>
      </div>

      <EditPropertyForm propertyId={id} />
    </div>
  );
};

export default EditProperty;
