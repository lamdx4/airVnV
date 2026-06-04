import { useState, useEffect, useRef, useMemo } from 'react';
import type { UseFormSetValue, UseFormWatch, FieldErrors } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { AlertCircle, LocateFixed } from 'lucide-react';
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { toast } from 'sonner';

import { VietnamAddressSelector } from '../VietnamAddressSelector';
import type { ProvinceResponse, WardResponse } from '../../types/vietnam-address';

// Fix Leaflet icon issue
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';

delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconUrl: markerIcon,
  iconRetinaUrl: markerIcon2x,
  shadowUrl: markerShadow,
});

interface LocationSectionProps {
  setValue: UseFormSetValue<any>;
  watch: UseFormWatch<any>;
  errors: FieldErrors<any>;
  control: any;
  dynamicAddressValues: Record<string, string>;
  setDynamicAddressValues: React.Dispatch<React.SetStateAction<Record<string, string>>>;
  onContinue: () => void;
}

export function LocationSection({
  setValue,
  watch,
  errors,
  dynamicAddressValues,
  setDynamicAddressValues,
  onContinue,
}: LocationSectionProps) {
  const { t } = useTranslation();
  const lat = watch('latitude') || 21.0285;
  const lng = watch('longitude') || 105.8542;
  const streetAddress = watch('streetAddress') || '';
  const unit = dynamicAddressValues['unit'] || '';

  const [selectedProvinceCode, setSelectedProvinceCode] = useState<number>();
  const [selectedWardCode, setSelectedWardCode] = useState<number>();

  const handleProvinceChange = (province: ProvinceResponse) => {
    setSelectedProvinceCode(province.code);
    setSelectedWardCode(undefined);
    setDynamicAddressValues(prev => ({
      ...prev,
      admin1Code: province.name,
      admin2Code: '', // reset ward
    }));
  };

  const handleWardChange = (ward: WardResponse) => {
    setSelectedWardCode(ward.code);
    setDynamicAddressValues(prev => ({
      ...prev,
      admin2Code: ward.name,
    }));

    // General center for ward
    const query = `${ward.name}, ${dynamicAddressValues['admin1Code']}, Vietnam`;
    fetch(`https://photon.komoot.io/api/?q=${encodeURIComponent(query)}&limit=1`)
      .then(res => res.json())
      .then(data => {
        if (data && data.features && data.features.length > 0) {
          const feature = data.features[0];
          const [lon, mapLat] = feature.geometry.coordinates;
          setValue('latitude', mapLat);
          setValue('longitude', lon);
        }
      })
      .catch(err => console.error('Photon geocoding failed:', err));
  };

  // Auto geocode fallback when manual dynamic inputs change for precise pinning
  useEffect(() => {
    const timer = setTimeout(() => {
      const admin1 = dynamicAddressValues['admin1Code'] || '';
      const admin2 = dynamicAddressValues['admin2Code'] || '';
      
      if (streetAddress.length > 5 && admin1) {
        const query = [streetAddress, admin2, admin1, 'Vietnam'].filter(Boolean).join(', ');
        
        fetch(`https://photon.komoot.io/api/?q=${encodeURIComponent(query)}&limit=1`)
          .then(res => res.json())
          .then(data => {
            if (data && data.features && data.features.length > 0) {
              const feature = data.features[0];
              const [lon, mapLat] = feature.geometry.coordinates;
              setValue('latitude', mapLat);
              setValue('longitude', lon);
              toast.success(t('location.autoMapSuccess'));
            }
          })
          .catch(err => console.error('Photon auto geocoding failed:', err));
      }
    }, 2000);

    return () => clearTimeout(timer);
  }, [streetAddress, dynamicAddressValues, setValue, t]);

  const handleGetLocation = () => {
    if (!navigator.geolocation) {
      toast.error(t('location.geoNotSupported'));
      return;
    }

    const toastId = toast.loading(t('location.locating'));
    navigator.geolocation.getCurrentPosition(
      (position) => {
        const { latitude, longitude } = position.coords;
        setValue('latitude', latitude);
        setValue('longitude', longitude);
        toast.dismiss(toastId);
        toast.success(t('location.locateSuccess'));
      },
      (error) => {
        toast.dismiss(toastId);
        toast.error(t('location.geoError'));
        console.error('Geolocation error:', error);
      },
      { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 }
    );
  };

  function LocationMarker() {
    const markerRef = useRef<any>(null);
    const eventHandlers = useMemo(
      () => ({
        dragend() {
          const marker = markerRef.current;
          if (marker != null) {
            const latLng = marker.getLatLng();
            setValue('latitude', latLng.lat);
            setValue('longitude', latLng.lng);
          }
        },
      }),
      []
    );

    useMapEvents({
      click(e) {
        setValue('latitude', e.latlng.lat);
        setValue('longitude', e.latlng.lng);
      },
    });

    return (
      <Marker draggable={true} eventHandlers={eventHandlers} position={[lat, lng]} ref={markerRef} />
    );
  }

  function MapPluginControls() {
    const map = useMap();
    useEffect(() => {
      map.flyTo([lat, lng], 14, { duration: 1.5 });
    }, [lat, lng, map]);
    return null;
  }

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">{t('location.title')}</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">{t('location.subtitle')}</p>
      </CardHeader>
      <CardContent className="px-6 py-6 space-y-6">
        
        {/* Vietnam Address Selection */}
        <VietnamAddressSelector 
          selectedProvinceCode={selectedProvinceCode}
          selectedWardCode={selectedWardCode}
          onProvinceChange={handleProvinceChange}
          onWardChange={handleWardChange}
        />

        {/* Detailed Address Inputs */}
        <div className="grid gap-4 sm:grid-cols-2">
          <div className="space-y-2 sm:col-span-2">
            <Label className="text-sm font-semibold text-slate-900 flex items-center gap-1">
              {t('location.streetAddressLabel')} <span className="text-red-500 font-bold">*</span>
            </Label>
            <Input
              type="text"
              value={streetAddress}
              onChange={(e) => setValue('streetAddress', e.target.value, { shouldValidate: true })}
              placeholder={t('location.streetAddressPlaceholder')}
              className="h-12 rounded-lg border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100"
            />
            {errors.streetAddress && (
              <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
                <AlertCircle className="h-4 w-4" /> {errors.streetAddress.message as string}
              </div>
            )}
          </div>
          
          <div className="space-y-2 sm:col-span-2">
            <Label className="text-sm font-semibold text-slate-900">
              {t('location.unitLabel')}
            </Label>
            <Input
              type="text"
              value={unit}
              onChange={(e) => setDynamicAddressValues(prev => ({ ...prev, unit: e.target.value }))}
              placeholder={t('location.unitPlaceholder')}
              className="h-12 rounded-lg border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100"
            />
          </div>
        </div>

        {/* Interactive Map */}
        <div className="space-y-2">
          <Label className="text-sm font-semibold text-slate-900">{t('location.adjustPinLabel')}</Label>
          <div className="relative w-full h-72 rounded-xl border border-slate-200 overflow-hidden shadow-inner bg-slate-100 z-0">
            <MapContainer center={[lat, lng]} zoom={13} scrollWheelZoom={false} className="w-full h-full z-0">
              <TileLayer
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              />
              <LocationMarker />
              <MapPluginControls />
            </MapContainer>
            
            <div className="absolute top-4 right-4 z-[400] flex flex-col gap-2">
              <Button
                type="button"
                variant="secondary"
                size="sm"
                onClick={handleGetLocation}
                className="bg-white hover:bg-slate-50 text-slate-900 shadow-md border border-slate-200 gap-2 h-10 px-4 rounded-lg font-semibold active:scale-95 transition-all whitespace-nowrap"
              >
                <LocateFixed className="h-4 w-4 text-[#FF5A5F]" />
                {t('location.useCurrent')}
              </Button>
            </div>
          </div>
        </div>

        <Button
          type="button"
          disabled={!streetAddress || !dynamicAddressValues['admin1Code'] || !dynamicAddressValues['admin2Code']}
          onClick={onContinue}
          className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm"
        >
          {t('location.continue')}
        </Button>
      </CardContent>
    </Card>
  );
}

