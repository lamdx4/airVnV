import { useState, useEffect, useRef, useMemo } from 'react';
import type { UseFormSetValue, UseFormWatch, FieldErrors } from 'react-hook-form';
import { Controller } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { AlertCircle, LocateFixed, Search } from 'lucide-react';
import { Loading03Icon } from 'hugeicons-react';
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { toast } from 'sonner';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

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
  addressFormConfig: any[];
  isLoadingConfig: boolean;
  isConfigError: boolean;
  dynamicAddressValues: Record<string, string>;
  setDynamicAddressValues: React.Dispatch<React.SetStateAction<Record<string, string>>>;
  onContinue: () => void;
  supportedCountries: any[];
  isLoadingCountries: boolean;
  isCountriesError: boolean;
}

export function LocationSection({
  setValue,
  watch,
  errors,
  control,
  addressFormConfig,
  isLoadingConfig,
  isConfigError,
  dynamicAddressValues,
  setDynamicAddressValues,
  onContinue,
  supportedCountries,
  isLoadingCountries,
  isCountriesError,
}: LocationSectionProps) {
  const { t } = useTranslation();
  const lat = watch('latitude') || 21.0285;
  const lng = watch('longitude') || 105.8542;
  const selectedCountryCode = watch('countryCode') || 'VN';

  // Photon Autocomplete States
  const [searchQuery, setSearchQuery] = useState('');
  const [suggestions, setSuggestions] = useState<any[]>([]);
  const [showSuggestions, setShowSuggestions] = useState(false);

  // Auto geocode fallback when manual dynamic inputs change
  useEffect(() => {
    const timer = setTimeout(() => {
      const street = dynamicAddressValues['street'] || '';
      const admin2 = dynamicAddressValues['admin2'] || '';
      const admin1 = dynamicAddressValues['admin1'] || '';
      
      if (street.length > 5 && admin1) {
        const countryName = supportedCountries.find(c => c.code === selectedCountryCode)?.name || '';
        const query = [street, admin2, admin1, countryName].filter(Boolean).join(', ');
        
        fetch(`https://photon.komoot.io/api/?q=${encodeURIComponent(query)}&limit=1`)
          .then(res => res.json())
          .then(data => {
            if (data && data.features && data.features.length > 0) {
              const feature = data.features[0];
              const [lon, lat] = feature.geometry.coordinates;
              setValue('latitude', lat);
              setValue('longitude', lon);
            }
          })
          .catch(err => console.error('Photon auto geocoding failed:', err));
      }
    }, 2000);

    return () => clearTimeout(timer);
  }, [dynamicAddressValues, selectedCountryCode, setValue, supportedCountries]);

  const handleSearchInputChange = async (val: string) => {
    setSearchQuery(val);
    if (val.length < 3) {
      setSuggestions([]);
      setShowSuggestions(false);
      return;
    }

    try {
      const countryName = supportedCountries.find(c => c.code === selectedCountryCode)?.name || '';
      const query = `${val}, ${countryName}`;
      const res = await fetch(`https://photon.komoot.io/api/?q=${encodeURIComponent(query)}&limit=5`);
      const data = await res.json();
      if (data && data.features) {
        setSuggestions(data.features);
        setShowSuggestions(true);
      }
    } catch (err) {
      console.error('Photon suggestions fetch failed:', err);
    }
  };

  const handleSelectSuggestion = (feature: any) => {
    const [lon, lat] = feature.geometry.coordinates;
    const props = feature.properties;

    console.log('🚀 [Geocoding] Raw Photon properties:', props);
    console.log('📋 [Geocoding] Address form config from service:', addressFormConfig);

    setValue('latitude', lat);
    setValue('longitude', lon);

    const updatedValues: Record<string, string> = {};
    
    addressFormConfig.forEach((field: any) => {
      let foundValue = '';
      for (const key of field.photonKeys) {
        if (props[key]) {
          foundValue = props[key].toString();
          break;
        }
      }
      
      // Smart joining for street address with house number prefix
      if (field.id === 'street') {
        const houseNo = props.housenumber || '';
        const street = props.street || foundValue || '';
        if (houseNo && street) {
          foundValue = `${houseNo} ${street}`.trim();
        } else {
          foundValue = street || foundValue;
        }
      }

      updatedValues[field.id] = foundValue;
    });

    setDynamicAddressValues(updatedValues);
    
    const resolvedStreetAddress = updatedValues['street'] || updatedValues['streetAddress'] || props.name || props.street || 'Selected Location';
    setValue('streetAddress', resolvedStreetAddress, { shouldValidate: true });
    
    setSearchQuery(props.name || props.street || searchQuery);
    setShowSuggestions(false);
    setSuggestions([]);
    toast.success(t('location.autoMapSuccess'));
  };

  // Get current user geolocations
  const handleGetLocation = () => {
    if (!navigator.geolocation) {
      toast.error('Geolocation is not supported by your browser.');
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
        toast.error('Unable to retrieve your location. Please check browser permissions.');
        console.error('Geolocation error:', error);
      },
      { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 }
    );
  };

  // Leaflet Location Pin (Draggable)
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
      [],
    );

    useMapEvents({
      click(e) {
        setValue('latitude', e.latlng.lat);
        setValue('longitude', e.latlng.lng);
      },
    });

    return (
      <Marker
        draggable={true}
        eventHandlers={eventHandlers}
        position={[lat, lng]}
        ref={markerRef}
      />
    );
  }

  function MapPluginControls() {
    const map = useMap();
    useEffect(() => {
      map.setView([lat, lng]);
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
        
        {/* Country Selector */}
        <div className="space-y-2">
          <Label className="text-sm font-semibold text-slate-900">{t('location.country')}</Label>
          {isCountriesError && (
            <div className="p-3 rounded-lg border border-red-200 bg-red-50 text-red-800 text-xs flex items-center gap-2 mb-2">
              <AlertCircle className="h-4 w-4 text-red-600 shrink-0" />
              <span>{t('location.errorFetchCountries')}</span>
            </div>
          )}
          <Controller
            name="countryCode"
            control={control}
            render={({ field }) => (
              <Select onValueChange={field.onChange} value={field.value}>
                <SelectTrigger className="h-12 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100">
                  <SelectValue placeholder={isLoadingCountries ? "Loading countries..." : t('location.country')} />
                </SelectTrigger>
                <SelectContent>
                  {supportedCountries?.map(c => (
                    <SelectItem key={c.code} value={c.code}>{c.name}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
        </div>

        {/* Photon Search Autocomplete Bar */}
        <div className="space-y-2 relative">
          <Label htmlFor="photonSearch" className="text-sm font-semibold text-slate-900">{t('location.search')}</Label>
          <div className="relative group">
            <span className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400">
              <Search className="h-5 w-5" />
            </span>
            <Input 
              id="photonSearch"
              type="text"
              value={searchQuery}
              onChange={(e) => handleSearchInputChange(e.target.value)}
              placeholder={t('location.searchPlaceholder')}
              className="h-12 pl-12 pr-4 rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100"
            />
          </div>

          {showSuggestions && suggestions.length > 0 && (
            <div className="absolute left-0 right-0 mt-1.5 bg-white border border-slate-200 rounded-lg shadow-xl z-[2000] overflow-hidden">
              <div className="p-2 bg-slate-50 border-b border-slate-100 text-[10px] uppercase font-bold text-slate-400 tracking-wider">
                Suggestions from OpenStreetMap
              </div>
              {suggestions.map((feature, i) => {
                const p = feature.properties;
                const formatted = [
                  p.housenumber ? `${p.housenumber} ${p.street || ''}` : p.street || p.name,
                  p.district || p.locality,
                  p.city,
                  p.country
                ].filter(Boolean).join(', ');

                return (
                  <button
                    key={i}
                    type="button"
                    onClick={() => handleSelectSuggestion(feature)}
                    className="w-full text-left px-4 py-3 text-sm text-slate-700 hover:bg-slate-50 transition-colors border-b border-slate-100 last:border-0"
                  >
                    {formatted}
                  </button>
                );
              })}
            </div>
          )}
        </div>

        {/* Leaflet Interactive Map */}
        <div className="space-y-2">
          <Label className="text-sm font-semibold text-slate-900">Adjust Coordinates (Drag Pin)</Label>
          <div className="relative w-full h-72 rounded-xl border border-slate-200 overflow-hidden shadow-inner bg-slate-100">
            <MapContainer
              center={[lat, lng]}
              zoom={13}
              scrollWheelZoom={false}
              className="w-full h-full"
            >
              <TileLayer
                attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              />
              <LocationMarker />
              <MapPluginControls />
            </MapContainer>
            
            <div className="absolute top-4 right-4 z-[1000] flex flex-col gap-2">
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

            <div className="absolute bottom-4 left-4 z-[1000] bg-white/90 px-3 py-1.5 rounded-md text-[10px] font-bold text-slate-500 shadow-sm border border-slate-200">
              {lat.toFixed(4)}, {lng.toFixed(4)}
            </div>
          </div>
        </div>

        {/* Dynamic Address Inputs based on metadata */}
        <div className="pt-4 border-t border-slate-100 space-y-4">
          <div className="flex items-center justify-between pb-1">
            <h3 className="text-md font-bold text-slate-900">Review Address Details</h3>
            <span className="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Dynamic Fields</span>
          </div>

          {isLoadingConfig ? (
            <div className="flex items-center justify-center py-6 gap-2">
              <Loading03Icon className="h-5 w-5 animate-spin text-pink-500" />
              <span className="text-sm font-semibold text-slate-400">Loading form configuration...</span>
            </div>
          ) : isConfigError ? (
            <div className="p-4 rounded-xl border border-red-200 bg-red-50/50 flex items-start gap-3">
              <AlertCircle className="h-5 w-5 text-red-600 mt-0.5 shrink-0" />
              <div className="space-y-1">
                <h4 className="text-sm font-bold text-red-900">Address Layout Service Offline</h4>
                <p className="text-xs text-red-700 leading-relaxed">
                  {t('location.errorFetchConfig', { countryCode: selectedCountryCode })}
                </p>
              </div>
            </div>
          ) : (
            <div className="grid gap-4 sm:grid-cols-2">
              {addressFormConfig.map((field: any, index: number) => {
                const isRequired = field.isRequired;
                const isStreet = field.id === 'street';
                const isLastOdd = addressFormConfig.length % 2 !== 0 && index === addressFormConfig.length - 1;
                const colSpanClass = (isStreet || isLastOdd) ? 'sm:col-span-2' : '';
                
                return (
                  <div key={field.id} className={`space-y-2 ${colSpanClass}`}>
                    <Label className="text-sm font-semibold text-slate-900 flex items-center gap-1">
                      {field.label}
                      {isRequired && <span className="text-red-500 font-bold">*</span>}
                    </Label>
                    <Input
                      type={field.type || 'text'}
                      value={dynamicAddressValues[field.id] || ''}
                      onChange={(e) => {
                        const val = e.target.value;
                        setDynamicAddressValues(prev => ({ ...prev, [field.id]: val }));
                        if (isStreet) {
                          setValue('streetAddress', val, { shouldValidate: true });
                        }
                      }}
                      placeholder={`Enter ${field.label.toLowerCase()}`}
                      className="h-12 rounded-lg border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100"
                    />
                  </div>
                )
              })}
            </div>
          )}
          
          {errors.streetAddress && (
            <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
              <AlertCircle className="h-4 w-4" /> {errors.streetAddress.message as string}
            </div>
          )}
        </div>

        <Button
          type="button"
          disabled={!watch('streetAddress')}
          onClick={onContinue}
          className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm"
        >
          {t('location.continue')}
        </Button>
      </CardContent>
    </Card>
  );
}
