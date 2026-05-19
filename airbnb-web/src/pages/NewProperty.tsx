import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useMutation, useQueryClient, useQuery } from '@tanstack/react-query';
import { motion, AnimatePresence } from 'framer-motion';
import { propertiesApi } from '@/features/properties/api/properties';
import { toCreatePropertyRequest } from '@/features/properties/utils/mappers';
import { toast } from 'sonner';
import { 
  ArrowLeft02Icon, 
  Loading03Icon,
} from 'hugeicons-react';
import { 
  Home, 
  Camera, 
  CheckCircle2, 
  AlertCircle,
  Zap,
  Award,
  Info,
  ChevronRight,
  LocateFixed,
  MapPinned
} from 'lucide-react';
import { Icon } from '@iconify/react';
import type { Amenity } from '@/features/properties/types';

// shadcn/ui components
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { 
  Card, 
  CardContent, 
  CardFooter, 
  CardHeader
} from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

// Leaflet
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

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

const formSchema = z.object({
  title: z.string().min(10, 'Title must be at least 10 characters'),
  description: z.string().min(30, 'Description must be at least 30 characters'),
  basePrice: z.number().min(1, 'Price must be greater than 0'),
  cleaningFee: z.number().default(0),
  serviceFee: z.number().default(0),
  weekendPremiumPercent: z.number().default(0),
  currencyCode: z.string().default('USD'),
  latitude: z.number(),
  longitude: z.number(),
  streetAddress: z.string().min(5, 'Street address is required'),
  countryCode: z.string().default('VN'),
  guestCount: z.number().min(1),
  bedroomCount: z.number().min(0),
  bedCount: z.number().min(0),
  bathroomCount: z.number().min(0),
  admin1Code: z.string().optional(), // Province
  admin2Code: z.string().optional(), // Ward
  allowPets: z.boolean().default(false),
  allowSmoking: z.boolean().default(false),
  allowEvents: z.boolean().default(false),
  checkInTime: z.string().min(4),
  checkOutTime: z.string().min(4),
  flexibleCheckOut: z.boolean().default(false),
});

type FormData = z.infer<typeof formSchema>;

export default function NewProperty() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [selectedAmenities, setSelectedAmenities] = useState<string[]>([]);
  const [completedSections, setCompletedSections] = useState<Set<string>>(new Set());
  
  // Location Data States
  const [provinces, setProvinces] = useState<any[]>([]);
  const [wards, setWards] = useState<any[]>([]);

  const { register, handleSubmit, setValue, watch, control, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema) as any,
    defaultValues: {
      latitude: 21.0285,
      longitude: 105.8542,
      countryCode: 'VN',
      currencyCode: 'USD',
      guestCount: 2,
      bedroomCount: 1,
      bedCount: 1,
      bathroomCount: 1,
      checkInTime: '14:00',
      checkOutTime: '12:00',
      allowPets: false,
      allowSmoking: false,
      allowEvents: false,
      flexibleCheckOut: false,
      basePrice: 50,
      cleaningFee: 0,
      serviceFee: 0,
      weekendPremiumPercent: 0,
    }
  });

  const lat = watch('latitude');
  const lng = watch('longitude');
  const selectedProvinceCode = watch('admin1Code');

  const onSubmit = (data: FormData) => {
    createMutation.mutate(data);
  };

  const markSectionComplete = (section: string) => {
    setCompletedSections(prev => new Set(prev).add(section));
  };

  // Load Provinces (v2)
  useEffect(() => {
    fetch('https://provinces.open-api.vn/api/v2/p/')
      .then(res => res.json())
      .then(data => setProvinces(data))
      .catch(err => console.error('Error loading provinces:', err));
  }, []);

  // Load Wards based on Province (v2 - CORRECT ENDPOINT)
  useEffect(() => {
    if (selectedProvinceCode) {
      fetch(`https://provinces.open-api.vn/api/v2/w/?province=${selectedProvinceCode}`)
        .then(res => res.json())
        .then(data => setWards(data || []))
        .catch(err => console.error('Error loading wards:', err));
    }
  }, [selectedProvinceCode]);

  // Automatically center map when address/ward changes (v2)
  const selectedWardCode = watch('admin2Code');
  const streetAddress = watch('streetAddress');

  useEffect(() => {
    // Only search if we have at least province and some street info or ward
    if (selectedProvinceCode && (selectedWardCode || streetAddress?.length > 5)) {
      const timer = setTimeout(() => {
        const ward = wards.find(w => w.code.toString() === selectedWardCode);
        const province = provinces.find(p => p.code.toString() === selectedProvinceCode);
        
        const queryParts = [];
        if (streetAddress) queryParts.push(streetAddress);
        if (ward) queryParts.push(ward.name);
        if (province) queryParts.push(province.name);
        queryParts.push('Vietnam');

        const query = queryParts.join(', ');
        
        fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}&limit=1`)
          .then(res => res.json())
          .then(results => {
            if (results && results.length > 0) {
              const { lat, lon } = results[0];
              setValue('latitude', parseFloat(lat));
              setValue('longitude', parseFloat(lon));
            }
          })
          .catch(err => console.error('Auto-center geocoding error:', err));
      }, 1500); // Debounce 1.5s

      return () => clearTimeout(timer);
    }
  }, [selectedWardCode, selectedProvinceCode, streetAddress, provinces, wards]);

  const { data: availableAmenities, isLoading: isLoadingAmenities } = useQuery({
    queryKey: ['amenities'],
    queryFn: () => propertiesApi.getAvailableAmenities()
  });

  const createMutation = useMutation({
    mutationFn: async (data: FormData) => {
      const province = provinces.find(p => p.code.toString() === data.admin1Code)?.name;
      const ward = wards.find(w => w.code.toString() === data.admin2Code)?.name;
      
      const parts = [data.streetAddress];
      if (ward) parts.push(ward);
      if (province) parts.push(province);
      const finalDisplayAddress = parts.filter(Boolean).join(', ');

      const payload = toCreatePropertyRequest({
         ...data,
         displayAddress: finalDisplayAddress
      });

      const propertyResponse = await propertiesApi.createProperty(payload);
      
      if (selectedAmenities.length > 0 && propertyResponse.id) {
          await Promise.all(
              selectedAmenities.map(amenityId => 
                  propertiesApi.addAmenity(propertyResponse.id, amenityId)
              )
          );
      }
      
      return propertyResponse;
    },
    onSuccess: (data) => {
      toast.success('Listing published successfully!');
      queryClient.invalidateQueries({ queryKey: ['host-properties'] });
      navigate(`/host/homes/${data.id}/edit?tab=photos`);
    }
  });

  const handleGetLocation = () => {
    if (!navigator.geolocation) {
      toast.error('Geolocation is not supported by your browser');
      return;
    }

    const toastId = toast.loading('Detecting your location...');
    
    navigator.geolocation.getCurrentPosition(
      (position) => {
        const { latitude, longitude } = position.coords;
        setValue('latitude', latitude, { shouldValidate: true });
        setValue('longitude', longitude, { shouldValidate: true });
        toast.dismiss(toastId);
        toast.success('Location detected!');
      },
      (error) => {
        toast.dismiss(toastId);
        toast.error('Unable to retrieve your location. Please check your browser permissions.');
        console.error('Geolocation error:', error);
      },
      { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 }
    );
  };

  const handleSearchAddress = async () => {
    const province = provinces.find(p => p.code.toString() === watch('admin1Code'))?.name;
    const ward = wards.find(w => w.code.toString() === watch('admin2Code'))?.name;
    const street = watch('streetAddress');

    if (!street || !province) {
      toast.error('Please enter at least Street Address and Province');
      return;
    }

    const query = [street, ward, province, 'Vietnam'].filter(Boolean).join(', ');
    const toastId = toast.loading('Searching for address on map...');

    try {
      const response = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}&limit=1`);
      const data = await response.json();

      if (data && data.length > 0) {
        const { lat, lon } = data[0];
        setValue('latitude', parseFloat(lat), { shouldValidate: true });
        setValue('longitude', parseFloat(lon), { shouldValidate: true });
        toast.dismiss(toastId);
        toast.success('Address found and pinned!');
      } else {
        toast.dismiss(toastId);
        toast.error('Could not find this address on the map. Please pin it manually.');
      }
    } catch (error) {
      toast.dismiss(toastId);
      toast.error('Search failed. Please try again or pin manually.');
      console.error('Geocoding error:', error);
    }
  };

  // Map Components
  function LocationMarker() {
    useMapEvents({
      click(e) {
        setValue('latitude', e.latlng.lat);
        setValue('longitude', e.latlng.lng);
      },
    });

    return <Marker position={[lat, lng]} />;
  }

  function MapPluginControls() {
    const map = useMap();
    useEffect(() => {
      map.setView([lat, lng]);
    }, [lat, lng, map]);
    return null;
  }

  return (
    <div className="min-h-screen bg-linear-to-br from-background via-secondary to-background">
      {/* Standard Airbnb Header */}
      <header className="sticky top-0 z-40 border-b border-border/40 bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60">
        <div className="mx-auto max-w-6xl px-4 py-6 sm:px-6 lg:px-8 flex items-center justify-between">
          <div className="flex items-center gap-4">
            <Button 
              variant="ghost" 
              size="icon" 
              onClick={() => navigate('/host/homes')}
              className="rounded-full h-10 w-10 hover:bg-slate-50 transition-colors"
            >
               <ArrowLeft02Icon className="h-5 w-5 text-slate-700" />
            </Button>
            <h1 className="text-3xl font-semibold text-slate-900 tracking-tight">List Your Property</h1>
          </div>
          <div className="flex items-center gap-4">
             <span className="text-sm font-semibold text-slate-500">Step {completedSections.size + 1} of 5</span>
             <Button variant="outline" className="h-10 rounded-lg text-sm font-semibold border-slate-300 hover:bg-slate-50 px-5">
                Save & Exit
             </Button>
          </div>
        </div>
        {/* Simple Progress Bar */}
        <div className="h-[2px] bg-slate-100 w-full overflow-hidden">
            <motion.div 
                className="h-full bg-[#FF5A5F]"
                initial={{ width: 0 }}
                animate={{ width: `${(completedSections.size / 5) * 100}%` }}
                transition={{ duration: 0.4, ease: "easeOut" }}
            />
        </div>
      </header>

      {/* Main Layout Template */}
      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          
          {/* Form Area (2/3) */}
          <div className="lg:col-span-2 space-y-8">
            <form onSubmit={handleSubmit(onSubmit as any)} className="space-y-8">
              
              <AnimatePresence mode="wait">
                {/* Section 1: The Basics */}
                <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
                  <CardHeader className="px-6 pt-6 pb-2 space-y-2">
                    <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">The Basics</h2>
                    <p className="text-base font-normal text-slate-600 leading-relaxed">Tell us where your place is and what it's like.</p>
                  </CardHeader>
                  <CardContent className="px-6 py-6 space-y-6">
                    <div className="space-y-2">
                      <Label htmlFor="title" className="text-sm font-semibold text-slate-900">Property Title</Label>
                      <p className="text-xs text-slate-500">Short titles work best. You can always change it later.</p>
                      <Input
                        id="title"
                        {...register('title')}
                        placeholder="e.g. Modern Apartment in Downtown"
                        className="h-12 px-4 py-3 rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100 transition-all placeholder-slate-400"
                      />
                      {errors.title && (
                        <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
                          <AlertCircle className="h-4 w-4" /> {errors.title.message}
                        </div>
                      )}
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="description" className="text-sm font-semibold text-slate-900">Description</Label>
                      <p className="text-xs text-slate-500">Share what makes your place special.</p>
                      <Textarea
                        id="description"
                        {...register('description')}
                        placeholder="Describe your property, amenities, and neighborhood..."
                        rows={5}
                        className="rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100 p-4 transition-all placeholder-slate-400"
                      />
                      {errors.description && (
                        <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
                          <AlertCircle className="h-4 w-4" /> {errors.description.message}
                        </div>
                      )}
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div className="space-y-2">
                        <Label className="text-sm font-semibold text-slate-900">Price per night</Label>
                        <div className="relative group">
                          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 font-semibold">$</span>
                          <Input 
                            type="number"
                            {...register('basePrice', { valueAsNumber: true })}
                            className="h-12 pl-8 pr-4 rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100"
                          />
                        </div>
                      </div>
                      <div className="space-y-2">
                        <Label className="text-sm font-semibold text-slate-900">Cleaning fee</Label>
                        <div className="relative group">
                           <span className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 font-semibold">$</span>
                           <Input 
                              type="number"
                              {...register('cleaningFee', { valueAsNumber: true })}
                              className="h-12 pl-8 pr-4 rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100"
                           />
                        </div>
                      </div>
                    </div>

                    <Button
                      type="button"
                      onClick={() => markSectionComplete('basic')}
                      className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm"
                    >
                      Continue <ChevronRight className="ml-2 h-4 w-4" />
                    </Button>
                  </CardContent>
                </Card>

                {/* Section 2: Capacity */}
                <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
                  <CardHeader className="px-6 pt-6 pb-2 space-y-2">
                    <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">Capacity</h2>
                    <p className="text-base font-normal text-slate-600 leading-relaxed">Who can stay at your place?</p>
                  </CardHeader>
                  <CardContent className="px-6 py-6 space-y-8">
                    <div className="grid gap-4 sm:grid-cols-2">
                      {[
                        { label: 'Guests', field: 'guestCount' as const },
                        { label: 'Bedrooms', field: 'bedroomCount' as const },
                        { label: 'Beds', field: 'bedCount' as const },
                        { label: 'Bathrooms', field: 'bathroomCount' as const },
                      ].map(({ label, field }) => (
                        <div key={field} className="flex items-center justify-between py-1 px-1 border-b border-slate-50 last:border-0">
                          <Label className="text-base font-semibold text-slate-900">{label}</Label>
                          <div className="flex items-center gap-4">
                            <Button 
                              type="button" 
                              variant="outline" 
                              size="icon"
                              className="h-9 w-9 rounded-full border-slate-300 hover:border-slate-900 transition-colors"
                              onClick={() => setValue(field, Math.max(0, (watch(field) || 0) - 1))}
                            >
                              -
                            </Button>
                            <span className="w-4 text-center font-semibold text-slate-900">{watch(field) || 0}</span>
                            <Button 
                              type="button" 
                              variant="outline" 
                              size="icon"
                              className="h-9 w-9 rounded-full border-slate-300 hover:border-slate-900 transition-colors"
                              onClick={() => setValue(field, (watch(field) || 0) + 1)}
                            >
                              +
                            </Button>
                          </div>
                        </div>
                      ))}
                    </div>
                    <Button
                      type="button"
                      onClick={() => markSectionComplete('capacity')}
                      className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm"
                    >
                      Next: Location
                    </Button>
                  </CardContent>
                </Card>

                {/* Section 3: Location */}
                <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
                  <CardHeader className="px-6 pt-6 pb-2 space-y-2">
                    <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">Location</h2>
                    <p className="text-base font-normal text-slate-600 leading-relaxed">Where is your place located?</p>
                  </CardHeader>
                  <CardContent className="px-6 py-6 space-y-6">
                    <div className="grid gap-4 sm:grid-cols-2">
                      <div className="space-y-2">
                        <Label className="text-sm font-semibold text-slate-900">Province / City</Label>
                        <Controller
                          name="admin1Code"
                          control={control}
                          render={({ field }) => (
                            <Select onValueChange={field.onChange} value={field.value}>
                              <SelectTrigger className="h-12 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100">
                                <SelectValue placeholder="Select province" />
                              </SelectTrigger>
                              <SelectContent>
                                {provinces.map(p => (
                                  <SelectItem key={p.code} value={p.code.toString()}>{p.name}</SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                          )}
                        />
                      </div>
                      <div className="space-y-2">
                        <Label className="text-sm font-semibold text-slate-900">Ward / Phường / Xã</Label>
                        <Controller
                          name="admin2Code"
                          control={control}
                          render={({ field }) => (
                            <Select onValueChange={field.onChange} value={field.value} disabled={!selectedProvinceCode}>
                              <SelectTrigger className="h-12 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100">
                                <SelectValue placeholder="Select ward" />
                              </SelectTrigger>
                              <SelectContent>
                                {wards.map(w => (
                                  <SelectItem key={w.code} value={w.code.toString()}>{w.name}</SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                          )}
                        />
                      </div>
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="address" className="text-sm font-semibold text-slate-900">Street Address</Label>
                      <Input
                        id="address"
                        {...register('streetAddress')}
                        placeholder="e.g. 123 Nguyen Hue Street"
                        className="h-12 rounded-lg border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100"
                      />
                    </div>
                    <div className="h-[350px] rounded-lg overflow-hidden border border-slate-200 relative z-0">
                      <MapContainer center={[lat, lng]} zoom={13} className="h-full w-full" scrollWheelZoom={false}>
                        <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                        <LocationMarker />
                        <MapPluginControls />
                      </MapContainer>
                      {/* Location Controls Container */}
                      <div className="absolute top-4 right-4 z-[1000] flex flex-col gap-2">
                        {/* Search from Address Button */}
                        <Button
                          type="button"
                          variant="secondary"
                          size="sm"
                          onClick={handleSearchAddress}
                          className="bg-white hover:bg-slate-50 text-slate-900 shadow-md border border-slate-200 gap-2 h-10 px-4 rounded-lg font-semibold active:scale-95 transition-all whitespace-nowrap"
                        >
                          <MapPinned className="h-4 w-4 text-blue-500" />
                          Lấy từ địa chỉ đã nhập
                        </Button>

                        {/* Get Current Location Button */}
                        <Button
                          type="button"
                          variant="secondary"
                          size="sm"
                          onClick={handleGetLocation}
                          className="bg-white hover:bg-slate-50 text-slate-900 shadow-md border border-slate-200 gap-2 h-10 px-4 rounded-lg font-semibold active:scale-95 transition-all whitespace-nowrap"
                        >
                          <LocateFixed className="h-4 w-4 text-[#FF5A5F]" />
                          Lấy vị trí hiện tại
                        </Button>
                      </div>

                      {/* Subdued map overlay */}
                      <div className="absolute bottom-4 left-4 z-[1000] bg-white/90 px-3 py-1.5 rounded-md text-[10px] font-bold text-slate-500 shadow-sm border border-slate-200">
                          {lat.toFixed(4)}, {lng.toFixed(4)}
                      </div>
                    </div>
                    <Button
                      type="button"
                      onClick={() => markSectionComplete('location')}
                      className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm"
                    >
                      Confirm Location
                    </Button>
                  </CardContent>
                </Card>

                {/* Section 4: Amenities */}
                <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
                  <CardHeader className="px-6 pt-6 pb-2 space-y-2">
                    <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">Amenities</h2>
                    <p className="text-base font-normal text-slate-600 leading-relaxed">What does your place offer?</p>
                  </CardHeader>
                  <CardContent className="px-6 py-6 space-y-6">
                    {isLoadingAmenities ? (
                      <div className="flex flex-col items-center justify-center py-12 gap-3">
                        <Loading03Icon className="h-8 w-8 animate-spin text-pink-500" />
                        <span className="text-xs font-semibold text-slate-400 uppercase tracking-widest">Finding Amenities...</span>
                      </div>
                    ) : (
                      <div className="grid gap-3 grid-cols-1 sm:grid-cols-2">
                        {availableAmenities?.map((amenity: Amenity) => {
                          const isSelected = selectedAmenities.includes(amenity.id);
                          return (
                            <label key={amenity.id} className={`flex items-center gap-4 p-4 rounded-lg border transition-all cursor-pointer ${
                              isSelected ? 'border-pink-500 bg-pink-50 ring-1 ring-pink-100' : 'border-slate-200 hover:bg-slate-50'
                            }`}>
                              <Checkbox
                                checked={isSelected}
                                onCheckedChange={() => {
                                  setSelectedAmenities(prev => 
                                    isSelected ? prev.filter(id => id !== amenity.id) : [...prev, amenity.id]
                                  );
                                }}
                                className="h-5 w-5 rounded-md border-slate-300 data-[state=checked]:bg-[#FF5A5F] data-[state=checked]:border-[#FF5A5F] transition-colors"
                              />
                              <div className="flex items-center gap-3">
                                  <Icon icon={amenity.iconCode || 'hugeicons:tick-02'} className={`text-xl ${isSelected ? 'text-pink-600' : 'text-slate-400'}`} />
                                  <span className="text-sm font-semibold text-slate-900">{amenity.name}</span>
                              </div>
                            </label>
                          )
                        })}
                      </div>
                    )}
                    <Button
                      type="button"
                      onClick={() => markSectionComplete('amenities')}
                      className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm"
                    >
                      Continue <ChevronRight className="ml-2 h-4 w-4" />
                    </Button>
                  </CardContent>
                </Card>

                {/* Section 5: House Rules */}
                <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
                  <CardHeader className="px-6 pt-6 pb-2 space-y-2">
                    <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">House Rules</h2>
                    <p className="text-base font-normal text-slate-600 leading-relaxed">Clear rules help ensure a smooth stay.</p>
                  </CardHeader>
                  <CardContent className="px-6 py-6 space-y-8">
                    <div className="grid gap-3 sm:grid-cols-2">
                      {[
                        { id: 'allowPets', label: 'Allow Pets' },
                        { id: 'allowSmoking', label: 'Allow Smoking' },
                        { id: 'allowEvents', label: 'Allow Events' },
                        { id: 'flexibleCheckOut', label: 'Flexible Check-out' },
                      ].map((rule) => {
                        const isChecked = watch(rule.id as any);
                        return (
                          <label key={rule.id} className={`flex items-center justify-between p-4 rounded-lg border transition-all cursor-pointer ${
                            isChecked ? 'border-pink-500 bg-pink-50' : 'border-slate-200 hover:bg-slate-50'
                          }`}>
                            <span className="text-sm font-semibold text-slate-900">{rule.label}</span>
                            <Checkbox
                              checked={isChecked}
                              onCheckedChange={(checked) => setValue(rule.id as any, checked === true)}
                              className="h-5 w-5 rounded-md border-slate-300 data-[state=checked]:bg-[#FF5A5F] data-[state=checked]:border-[#FF5A5F]"
                            />
                          </label>
                        )
                      })}
                    </div>
                    
                    <div className="grid grid-cols-2 gap-8 pt-6 border-t border-slate-100">
                        <div className="space-y-2">
                            <Label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Check-in After</Label>
                            <Input type="time" {...register('checkInTime')} className="h-11 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100" />
                        </div>
                        <div className="space-y-2">
                            <Label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">Check-out Before</Label>
                            <Input type="time" {...register('checkOutTime')} className="h-11 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100" />
                        </div>
                    </div>

                    <div className="p-4 bg-blue-50 border border-blue-100 rounded-lg flex gap-3">
                        <Info className="h-5 w-5 text-blue-600 shrink-0 mt-0.5" />
                        <p className="text-xs font-medium text-blue-700 leading-relaxed">
                            By publishing, you agree to comply with Airbnb's community standards and local hosting laws.
                        </p>
                    </div>
                  </CardContent>
                </Card>
              </AnimatePresence>

              {/* Submit Action */}
              <div className="pt-6">
                <Button
                  type="submit"
                  size="lg"
                  disabled={createMutation.isPending}
                  className="w-full h-14 rounded-xl bg-[#FF5A5F] text-white font-bold text-lg hover:bg-[#E04447] active:scale-95 shadow-md transition-all flex items-center justify-center gap-3"
                >
                  {createMutation.isPending ? (
                    <Loading03Icon className="h-6 w-6 animate-spin" />
                  ) : (
                    <>
                      <Zap className="h-5 w-5 fill-white/20" />
                      <span>Publish Listing</span>
                    </>
                  )}
                </Button>
              </div>
            </form>
          </div>

          {/* Expert Sidebar (1/3) */}
          <div className="lg:col-span-1 space-y-6">
            <div className="sticky top-28 space-y-6">
                {/* Tips Card */}
                <Card className="rounded-xl border border-slate-200 shadow-sm bg-white overflow-hidden">
                    <CardHeader className="px-6 pt-6 pb-2">
                        <div className="flex items-center gap-2 text-[#FF5A5F]">
                            <Award className="h-5 w-5" />
                            <h3 className="text-lg font-semibold text-slate-900">Hosting Tips</h3>
                        </div>
                    </CardHeader>
                    <CardContent className="px-6 py-6 space-y-6">
                        {[
                            { 
                                title: 'Stunning Photos', 
                                desc: 'Listings with high-res photos get 3x more bookings.', 
                                icon: Camera 
                            },
                            { 
                                title: 'Honest Details', 
                                desc: 'Help guests set expectations with accurate descriptions.', 
                                icon: Home 
                            },
                            { 
                                title: 'Right Pricing', 
                                desc: 'Stay competitive by checking similar properties nearby.', 
                                icon: Zap 
                            }
                        ].map((tip, i) => (
                             <div key={i} className="flex items-start gap-4 group">
                                <div className="p-2.5 bg-slate-50 rounded-xl group-hover:bg-pink-50 transition-colors flex items-center justify-center shrink-0">
                                    <tip.icon className="h-5 w-5 text-slate-400 group-hover:text-pink-500 transition-colors" />
                                </div>
                                <div className="space-y-1">
                                    <h4 className="text-sm font-semibold text-slate-900">{tip.title}</h4>
                                    <p className="text-xs text-slate-500 leading-relaxed font-normal">{tip.desc}</p>
                                </div>
                            </div>
                        ))}
                    </CardContent>
                    <CardFooter className="px-6 pb-6 pt-0 border-t border-slate-50 flex items-center justify-center">
                        <Button variant="ghost" className="text-xs font-semibold text-slate-500 hover:text-slate-900 underline">
                            View full hosting guide
                        </Button>
                    </CardFooter>
                </Card>

                {/* Checklist Card */}
                <Card className="rounded-xl border border-slate-200 shadow-sm bg-white text-slate-900 overflow-hidden relative group">
                    <div className="absolute top-0 right-0 w-24 h-24 bg-pink-500/5 rounded-full blur-3xl -mr-12 -mt-12 group-hover:scale-150 transition-transform" />
                    <CardHeader className="px-6 pt-6 pb-2">
                        <h3 className="text-lg font-semibold text-slate-900">Publish Checklist</h3>
                        <div className="mt-4 flex items-center gap-3">
                            <div className="flex-1 h-1.5 bg-slate-100 rounded-full overflow-hidden">
                                <motion.div 
                                    className="h-full bg-[#FF5A5F]"
                                    initial={{ width: 0 }}
                                    animate={{ width: `${(completedSections.size / 5) * 100}%` }}
                                />
                            </div>
                            <span className="text-xs font-bold text-[#FF5A5F]">{Math.round((completedSections.size / 5) * 100)}%</span>
                        </div>
                    </CardHeader>
                    <CardContent className="px-6 py-6 space-y-4">
                         {[
                            { id: 'basic', label: 'Basic Details' },
                            { id: 'capacity', label: 'Capacity & Rooms' },
                            { id: 'location', label: 'Location' },
                            { id: 'amenities', label: 'Amenities' },
                            { id: 'rules', label: 'House Rules' },
                         ].map((item) => (
                             <div key={item.id} className="flex items-center gap-3">
                                <div className={`h-5 w-5 rounded-md border flex items-center justify-center transition-all ${
                                    completedSections.has(item.id) ? 'bg-[#FF5A5F] border-[#FF5A5F]' : 'border-slate-200 bg-slate-50'
                                }`}>
                                    {completedSections.has(item.id) && <CheckCircle2 className="h-3 w-3 text-white" />}
                                </div>
                                <span className={`text-xs font-medium transition-colors ${
                                    completedSections.has(item.id) ? 'text-slate-900' : 'text-slate-400'
                                }`}>
                                    {item.label}
                                </span>
                             </div>
                         ))}
                    </CardContent>
                </Card>
            </div>
          </div>

        </div>
      </main>
    </div>
  );
}
