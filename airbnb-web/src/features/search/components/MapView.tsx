import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import type { PropertyMapMarker } from '../types';
import L from 'leaflet';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

// Fix Leaflet default marker icon issue in React
delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

// Custom Price Marker Icon
const createPriceIcon = (price: number, currency: string) => {
  const formattedPrice = new Intl.NumberFormat('vi-VN', { style: 'currency', currency }).format(price);
  
  return L.divIcon({
    className: 'custom-price-marker',
    html: `<div style="
      background-color: white; 
      border-radius: 20px; 
      padding: 5px 10px; 
      box-shadow: 0 2px 4px rgba(0,0,0,0.2); 
      font-weight: bold; 
      font-size: 14px; 
      color: #222;
      border: 1px solid #ddd;
      transition: transform 0.2s;
    ">${formattedPrice}</div>`,
    iconSize: [80, 30],
    iconAnchor: [40, 15],
  });
};

// Component to dynamically change map view when coordinates change
function ChangeView({ center, zoom }: { center: [number, number], zoom: number }) {
  const map = useMap();
  useEffect(() => {
    map.setView(center, zoom);
  }, [center, zoom, map]);
  return null;
}

interface MapViewProps {
  latitude: number;
  longitude: number;
  markers: PropertyMapMarker[];
}

export function MapView({ latitude, longitude, markers }: MapViewProps) {
  const navigate = useNavigate();

  return (
    <div className="w-full h-full relative rounded-xl overflow-hidden shadow-inner">
      <MapContainer 
        center={[latitude, longitude]} 
        zoom={12} 
        scrollWheelZoom={true} 
        style={{ height: '100%', width: '100%' }}
      >
        <ChangeView center={[latitude, longitude]} zoom={12} />
        
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        
        {/* User Location Marker */}
        <Marker position={[latitude, longitude]}>
          <Popup>You are here</Popup>
        </Marker>

        {/* Property Markers */}
        {markers.map(marker => (
          <Marker 
            key={marker.id} 
            position={[marker.latitude, marker.longitude]}
            icon={createPriceIcon(marker.price, marker.currency)}
            eventHandlers={{
              click: () => {
                navigate(`/properties/${marker.id}`);
              }
            }}
          >
            <Popup>
              <div className="flex flex-col gap-2 p-1 min-w-[200px] cursor-pointer" onClick={() => navigate(`/properties/${marker.id}`)}>
                {marker.thumbnail && (
                  <img src={marker.thumbnail} alt={marker.title} className="w-full h-32 object-cover rounded-lg" />
                )}
                <div className="font-semibold text-base">{marker.title}</div>
                <div className="flex items-center text-sm">
                  <span className="font-bold mr-1">{marker.rating.toFixed(2)}</span>
                  <span>⭐</span>
                </div>
              </div>
            </Popup>
          </Marker>
        ))}
      </MapContainer>
    </div>
  );
}
