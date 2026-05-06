import React, { useState, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import { Location01Icon, PinIcon, Loading03Icon, SentIcon } from 'hugeicons-react';
import { useUpdateLocation } from '../hooks/useProperties';
import { toast } from 'sonner';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

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

// --- Open API v2 Interfaces ---
interface ProvinceResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  phone_code: number;
  wards: WardResponse[];
}

interface WardResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  province_code: number;
}

interface LocationManagerProps {
  propertyId: string;
  initialLat: number;
  initialLng: number;
  initialAddress: string;
  initialSubDivisions?: Record<string, string>;
}

export const LocationManager: React.FC<LocationManagerProps> = ({ 
  propertyId, 
  initialLat, 
  initialLng,
  initialAddress,
  initialSubDivisions
}) => {
  // Map State
  const [position, setPosition] = useState<[number, number]>([initialLat, initialLng]);
  const [isReverseGeocoding, setIsReverseGeocoding] = useState(false);
  
  // Address State
  const [streetAddress, setStreetAddress] = useState(initialAddress || '');
  const [selectedProvinceCode, setSelectedProvinceCode] = useState<string>(initialSubDivisions?.provinceCode || '');
  const [selectedWardCode, setSelectedWardCode] = useState<string>(initialSubDivisions?.wardCode || '');
  
  // API Data State
  const [provinces, setProvinces] = useState<ProvinceResponse[]>([]);
  const [wards, setWards] = useState<WardResponse[]>([]);
  const [isLoadingProvinces, setIsLoadingProvinces] = useState(false);
  const [isLoadingWards, setIsLoadingWards] = useState(false);

  const updateLocationMutation = useUpdateLocation();

  // Fetch all provinces on mount
  useEffect(() => {
    const fetchProvinces = async () => {
      setIsLoadingProvinces(true);
      try {
        const res = await fetch('https://provinces.open-api.vn/api/v2/p/');
        const data = await res.json();
        setProvinces(data);
      } catch (err) {
        console.error("Failed to fetch provinces", err);
      } finally {
        setIsLoadingProvinces(false);
      }
    };
    fetchProvinces();
  }, []);

  // Fetch wards when a province is selected (depth=2)
  useEffect(() => {
    if (!selectedProvinceCode) {
      setWards([]);
      return;
    }
    
    const fetchWards = async () => {
      setIsLoadingWards(true);
      try {
        const res = await fetch(`https://provinces.open-api.vn/api/v2/p/${selectedProvinceCode}?depth=2`);
        const data: ProvinceResponse = await res.json();
        setWards(data.wards || []);
      } catch (err) {
        console.error("Failed to fetch wards", err);
      } finally {
        setIsLoadingWards(false);
      }
    };
    
    fetchWards();
  }, [selectedProvinceCode]);

  // Handle Map Clicks
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

  // Reverse Geocode (Only updates Street Address/Detected Address now)
  const reverseGeocode = async (lat: number, lng: number) => {
    setIsReverseGeocoding(true);
    try {
      const response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}&zoom=18&addressdetails=1`);
      const data = await response.json();
      if (data && data.display_name) {
        // We use reverse geocoding purely as a suggestion for the street address
        // User still explicitly selects Province/Ward via dropdowns.
        setStreetAddress(data.display_name);
      }
    } catch (err) {
      console.error('Failed to fetch address');
    } finally {
      setIsReverseGeocoding(false);
    }
  };

  const handleSave = async () => {
    const selectedProvince = provinces.find(p => p.code.toString() === selectedProvinceCode);
    const selectedWard = wards.find(w => w.code.toString() === selectedWardCode);

    // Build the full display address
    const parts = [streetAddress];
    if (selectedWard) parts.push(selectedWard.name);
    if (selectedProvince) parts.push(selectedProvince.name);
    const fullAddress = parts.filter(Boolean).join(', ');

    try {
      await updateLocationMutation.mutateAsync({
        propertyId,
        data: {
          latitude: position[0],
          longitude: position[1],
          displayAddress: fullAddress,
          countryCode: 'VN',
          streetAddress: streetAddress,
          admin1Code: selectedProvinceCode,
          subDivisions: {
            province: selectedProvince?.name || '',
            provinceCode: selectedProvinceCode,
            ward: selectedWard?.name || '',
            wardCode: selectedWardCode
          }
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
            Property Address
          </h3>
          <p className="text-sm text-slate-500 mt-1">
            Specify the administrative boundaries and exact map coordinates.
          </p>
        </div>

        <div className="bg-slate-50 p-4 rounded-2xl border space-y-4">
            
            {/* Province Selection */}
            <div className="space-y-1">
              <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Province / City</label>
              <Select 
                value={selectedProvinceCode} 
                onValueChange={(val) => {
                  setSelectedProvinceCode(val);
                  setSelectedWardCode(''); // Reset ward when province changes
                }}
                disabled={isLoadingProvinces}
              >
                <SelectTrigger className="w-full bg-white h-11 border-slate-200">
                  <SelectValue placeholder={isLoadingProvinces ? "Loading provinces..." : "Select a province..."} />
                </SelectTrigger>
                <SelectContent>
                  {provinces.map((prov) => (
                    <SelectItem key={prov.code} value={prov.code.toString()}>
                      {prov.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Ward Selection */}
            <div className="space-y-1">
              <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Ward / Commune</label>
              <Select 
                value={selectedWardCode} 
                onValueChange={setSelectedWardCode}
                disabled={!selectedProvinceCode || isLoadingWards}
              >
                <SelectTrigger className="w-full bg-white h-11 border-slate-200">
                  <SelectValue placeholder={!selectedProvinceCode ? "Select province first" : (isLoadingWards ? "Loading wards..." : "Select a ward...")} />
                </SelectTrigger>
                <SelectContent>
                  {wards.map((ward) => (
                    <SelectItem key={ward.code} value={ward.code.toString()}>
                      {ward.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Street Address */}
            <div className="space-y-1">
                <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Street Address / House Number</label>
                <textarea 
                  value={streetAddress}
                  onChange={(e) => setStreetAddress(e.target.value)}
                  className={`w-full text-sm text-hof bg-white p-3 rounded-lg border min-h-[80px] focus:outline-none focus:border-rausch transition-all ${isReverseGeocoding ? 'opacity-50' : ''}`}
                  placeholder="e.g., 123 Nguyen Trai Street"
                />
            </div>

            <div className="space-y-1 pt-2 border-t border-slate-200">
                <label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">Coordinates</label>
                <div className="text-sm font-mono text-hof bg-white p-2 rounded-lg border">
                    {position[0].toFixed(6)}, {position[1].toFixed(6)}
                </div>
            </div>
        </div>

        <button
          onClick={handleSave}
          disabled={updateLocationMutation.isPending || isReverseGeocoding || !selectedProvinceCode || !selectedWardCode}
          className="w-full bg-hof text-white h-12 rounded-xl font-bold flex items-center justify-center gap-2 hover:bg-slate-800 transition-all disabled:bg-slate-300 disabled:cursor-not-allowed"
        >
          {updateLocationMutation.isPending ? <Loading03Icon className="h-5 w-5 animate-spin" /> : <SentIcon className="h-5 w-5" />}
          Save Location
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
