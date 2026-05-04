import React, { useState } from 'react';
import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import { Location01Icon, PinIcon, Loading03Icon, SentIcon } from 'hugeicons-react';
import { useUpdateLocation } from '../hooks/useProperties';
import { toast } from 'sonner';

// Fix leaflet icon issue in React
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

let DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});
L.Marker.prototype.options.icon = DefaultIcon;

interface LocationManagerProps {
  propertyId: string;
  initialLat: number;
  initialLng: number;
  initialAddress: string;
}

export const LocationManager: React.FC<LocationManagerProps> = ({ 
  propertyId, 
  initialLat, 
  initialLng,
  initialAddress 
}) => {
  const [position, setPosition] = useState<[number, number]>([initialLat, initialLng]);
  const [address, setAddress] = useState(initialAddress);
  const [isReverseGeocoding, setIsReverseGeocoding] = useState(false);
  
  const updateLocationMutation = useUpdateLocation();

  // Component to handle map clicks and marker movement
  function LocationMarker() {
    useMapEvents({
      click(e) {
        setPosition([e.latlng.lat, e.latlng.lng]);
        reverseGeocode(e.latlng.lat, e.latlng.lng);
      },
    });

    return position === null ? null : (
      <Marker position={position} draggable={true} eventHandlers={{
        dragend: (e) => {
            const marker = e.target;
            const pos = marker.getLatLng();
            setPosition([pos.lat, pos.lng]);
            reverseGeocode(pos.lat, pos.lng);
        }
      }} />
    );
  }

  const reverseGeocode = async (lat: number, lng: number) => {
    setIsReverseGeocoding(true);
    try {
      const response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}&zoom=18&addressdetails=1`);
      const data = await response.json();
      if (data && data.display_name) {
        setAddress(data.display_name);
      }
    } catch (err) {
      console.error('Failed to fetch address');
    } finally {
      setIsReverseGeocoding(false);
    }
  };

  const handleSave = async () => {
    try {
      await updateLocationMutation.mutateAsync({
        propertyId,
        data: {
          latitude: position[0],
          longitude: position[1],
          displayAddress: address,
          countryCode: 'VN', // Simplified for demo, should be extracted from reverse geocode
          streetAddress: address.split(',')[0] || address,
          admin1Code: 'VN-HN', // Simplified
          subDivisions: {}
        }
      });
      toast.success('Location updated successfully');
    } catch (err) {
      toast.error('Failed to update location');
    }
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
      {/* Sidebar Controls */}
      <div className="space-y-6">
        <div>
          <h3 className="text-lg font-bold text-hof flex items-center gap-2">
            <Location01Icon className="h-5 w-5 text-rausch" />
            Precise Location
          </h3>
          <p className="text-sm text-slate-500 mt-1">
            Drag the pin to the exact location of your property. Guests will only see this after booking.
          </p>
        </div>

        <div className="bg-slate-50 p-4 rounded-2xl border space-y-4">
            <div className="space-y-1">
                <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Coordinates</label>
                <div className="text-sm font-mono text-hof bg-white p-2 rounded-lg border">
                    {position[0].toFixed(6)}, {position[1].toFixed(6)}
                </div>
            </div>

            <div className="space-y-1">
                <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Detected Address</label>
                <div className={`text-sm text-hof bg-white p-3 rounded-lg border min-h-[80px] ${isReverseGeocoding ? 'opacity-50' : ''}`}>
                    {isReverseGeocoding ? 'Detecting address...' : address}
                </div>
            </div>
        </div>

        <button
          onClick={handleSave}
          disabled={updateLocationMutation.isPending || isReverseGeocoding}
          className="w-full bg-hof text-white h-12 rounded-xl font-bold flex items-center justify-center gap-2 hover:bg-slate-800 transition-all disabled:bg-slate-300"
        >
          {updateLocationMutation.isPending ? <Loading03Icon className="h-5 w-5 animate-spin" /> : <SentIcon className="h-5 w-5" />}
          Update Location
        </button>
      </div>

      {/* Map Display */}
      <div className="lg:col-span-2 aspect-video lg:aspect-auto h-[500px] rounded-3xl overflow-hidden border shadow-inner relative">
         <MapContainer center={position} zoom={16} scrollWheelZoom={true} className="h-full w-full">
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <LocationMarker />
          </MapContainer>
          
          <div className="absolute top-4 left-4 z-[1000] bg-white/95 backdrop-blur px-4 py-2 rounded-full shadow-lg border flex items-center gap-2 pointer-events-none">
             <PinIcon className="h-4 w-4 text-rausch" />
             <span className="text-xs font-bold text-hof">Click map or drag pin</span>
          </div>
      </div>
    </div>
  );
};
