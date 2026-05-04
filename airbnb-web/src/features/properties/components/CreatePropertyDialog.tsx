import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { 
  Dialog, 
  DialogContent, 
  DialogDescription, 
  DialogHeader, 
  DialogTitle, 
  DialogTrigger,
  DialogFooter
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { PlusSignIcon, Location01Icon } from 'hugeicons-react';
import { useCreateProperty } from '../hooks/useProperties';
import { toCreatePropertyRequest } from '../utils/mappers';
import { CreatePropertyFormData } from '../types';
import { toast } from 'sonner';

// Leaflet imports
import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

// Fix Leaflet icon issue
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';
let DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});
L.Marker.prototype.options.icon = DefaultIcon;

const schema = z.object({
  title: z.string().min(10, 'Title must be at least 10 characters'),
  description: z.string().min(20, 'Description must be at least 20 characters'),
  basePrice: z.number().min(1, 'Price must be greater than 0'),
  displayAddress: z.string().min(5, 'Address is required'),
  latitude: z.number(),
  longitude: z.number(),
});

type FormData = z.infer<typeof schema>;

// Component xử lý sự kiện click trên bản đồ
const LocationMarker = ({ position, setPosition }: { position: [number, number], setPosition: (pos: [number, number]) => void }) => {
  useMapEvents({
    click(e) {
      setPosition([e.latlng.lat, e.latlng.lng]);
    },
  });

  return position ? <Marker position={position} /> : null;
};

export const CreatePropertyDialog: React.FC = () => {
  const [open, setOpen] = React.useState(false);
  const [position, setPosition] = useState<[number, number]>([10.762622, 106.660172]); // Default HCMC
  
  const createMutation = useCreateProperty();
  
  const { register, handleSubmit, formState: { errors }, reset, setValue } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      title: '',
      description: '',
      basePrice: 50,
      displayAddress: '',
      latitude: 10.762622,
      longitude: 106.660172,
    }
  });

  // Đồng bộ hóa tọa độ từ bản đồ vào form
  React.useEffect(() => {
    setValue('latitude', position[0]);
    setValue('longitude', position[1]);
  }, [position, setValue]);

  const onSubmit = async (data: FormData) => {
    try {
      const payload = toCreatePropertyRequest(data);
      await createMutation.mutateAsync(payload);
      toast.success('Property created successfully!');
      setOpen(false);
      reset();
    } catch (err: any) {
      toast.error(err.message || 'Failed to create property');
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="bg-rose-500 hover:bg-rose-600 shadow-md transition-all hover:scale-105 active:scale-95">
          <PlusSignIcon className="mr-2 h-4 w-4" /> Create Listing
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[700px] max-h-[90vh] overflow-y-auto rounded-2xl">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-hof">Create new listing</DialogTitle>
          <DialogDescription>
            Pin your location on the map and fill in the details.
          </DialogDescription>
        </DialogHeader>
        
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 py-4">
          {/* Map Picker */}
          <div className="space-y-2">
            <label className="text-sm font-medium flex items-center gap-2">
              <Location01Icon className="h-4 w-4 text-rausch" /> 
              Pin Location (Click to move)
            </label>
            <div className="h-[250px] w-full rounded-xl border overflow-hidden z-0">
              <MapContainer 
                center={position} 
                zoom={13} 
                scrollWheelZoom={false} 
                style={{ height: '100%', width: '100%' }}
              >
                <TileLayer
                  attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                  url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                <LocationMarker position={position} setPosition={setPosition} />
              </MapContainer>
            </div>
            <div className="grid grid-cols-2 gap-2 text-[10px] text-muted-foreground">
              <span>Lat: {position[0].toFixed(6)}</span>
              <span>Lng: {position[1].toFixed(6)}</span>
            </div>
          </div>

          <div className="space-y-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Title</label>
              <Input 
                {...register('title')} 
                placeholder="e.g. Modern Apartment in District 1"
                className={errors.title ? 'border-red-500' : ''}
              />
              {errors.title && <p className="text-xs text-red-500">{errors.title.message}</p>}
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Description</label>
              <Textarea 
                {...register('description')} 
                placeholder="Tell guests about your place..."
                className={errors.description ? 'border-red-500' : ''}
                rows={3}
              />
              {errors.description && <p className="text-xs text-red-500">{errors.description.message}</p>}
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Base Price ($)</label>
                <Input 
                  type="number"
                  {...register('basePrice', { valueAsNumber: true })} 
                  className={errors.basePrice ? 'border-red-500' : ''}
                />
                {errors.basePrice && <p className="text-xs text-red-500">{errors.basePrice.message}</p>}
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Address</label>
                <Input 
                  {...register('displayAddress')} 
                  placeholder="Street address"
                  className={errors.displayAddress ? 'border-red-500' : ''}
                />
                {errors.displayAddress && <p className="text-xs text-red-500">{errors.displayAddress.message}</p>}
              </div>
            </div>
          </div>

          <DialogFooter className="pt-4 border-t">
            <Button 
              type="button" 
              variant="ghost" 
              onClick={() => setOpen(false)}
              disabled={createMutation.isPending}
            >
              Cancel
            </Button>
            <Button 
              type="submit" 
              className="bg-rausch hover:bg-rausch/90 min-w-[120px]"
              disabled={createMutation.isPending}
            >
              {createMutation.isPending ? 'Creating...' : 'Create Listing'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};
