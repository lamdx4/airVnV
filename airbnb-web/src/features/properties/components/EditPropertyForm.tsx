import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { 
  Settings01Icon, 
  Image01Icon, 
  Task01Icon, 
  Location01Icon,
  Calendar03Icon,
  Tick02Icon,
  Loading03Icon,
  ViewIcon,
  ArrowLeft02Icon
} from 'hugeicons-react';
import { useProperty, useUpdateProperty, useUpdateStatus } from '../hooks/useProperties';
import { ImageManager } from './ImageManager';
import { AmenityManager } from './AmenityManager';
import { LocationManager } from './LocationManager';
import { AvailabilityCalendar } from './AvailabilityCalendar';
import { toast } from 'sonner';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { PropertyStatus, PropertyType } from '../types';
import { getStatusColor, getStatusText } from '../utils/status';

const schema = z.object({
  title: z.string().min(10, 'Title must be at least 10 characters'),
  description: z.string().min(20, 'Description must be at least 20 characters'),
  type: z.number().min(1, 'Property type is required'),
  basePrice: z.number().min(1, 'Price must be at least 1'),
  cleaningFee: z.number(),
  allowPets: z.boolean(),
  allowSmoking: z.boolean(),
  allowEvents: z.boolean(),
  checkInTime: z.string(),
  checkOutTime: z.string(),
  flexibleCheckOut: z.boolean()
});

type EditPropertyInput = z.infer<typeof schema>;

type TabType = 'general' | 'photos' | 'amenities' | 'location' | 'calendar';

export const EditPropertyForm: React.FC<{ propertyId: string }> = ({ propertyId }) => {
  const [searchParams] = useSearchParams();
  const initialTab = searchParams.get('tab') as TabType;
  
  const [activeTab, setActiveTab] = useState<TabType>(
    ['general', 'photos', 'amenities', 'location', 'calendar'].includes(initialTab) 
        ? initialTab 
        : 'general'
  );
  const { data: property, isLoading } = useProperty(propertyId);
  const updateMutation = useUpdateProperty();
  const updateStatusMutation = useUpdateStatus();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset
  } = useForm<EditPropertyInput>({
    resolver: zodResolver(schema),
    defaultValues: {
      title: '',
      description: '',
      type: PropertyType.Apartment,
      basePrice: 0,
      cleaningFee: 0,
      allowPets: false,
      allowSmoking: false,
      allowEvents: false,
      checkInTime: '14:00',
      checkOutTime: '11:00',
      flexibleCheckOut: false
    }
  });

  useEffect(() => {
    if (property) {
      reset({
        title: property.title,
        description: property.description,
        type: property.type,
        basePrice: property.pricing.basePrice,
        cleaningFee: property.pricing.cleaningFee,
        allowPets: property.houseRules.allowPets,
        allowSmoking: property.houseRules.allowSmoking,
        allowEvents: property.houseRules.allowEvents,
        checkInTime: property.houseRules.checkInTime,
        checkOutTime: property.houseRules.checkOutTime,
        flexibleCheckOut: property.houseRules.flexibleCheckOut
      });
    }
  }, [property, reset]);

  const onSubmit = async (data: EditPropertyInput) => {
    try {
      // Map to proper update payload
      const payload = {
        title: data.title,
        description: data.description,
        type: data.type,
        pricing: {
            basePrice: data.basePrice,
            cleaningFee: data.cleaningFee || 0
        },
        houseRules: {
            allowPets: data.allowPets,
            allowSmoking: data.allowSmoking,
            allowEvents: data.allowEvents,
            checkInTime: data.checkInTime,
            checkOutTime: data.checkOutTime,
            flexibleCheckOut: data.flexibleCheckOut
        }
      };
      await updateMutation.mutateAsync({ propertyId, data: payload });
      toast.success('Basic info updated');
    } catch (err) {
      toast.error('Failed to update property');
    }
  };

  const handlePublish = async () => {
    try {
      await updateStatusMutation.mutateAsync({ 
        propertyId, 
        status: PropertyStatus.Published 
      });
      toast.success('Your listing is now live!');
    } catch (err: any) {
      const message = err.response?.data?.message || 'Check requirements before publishing';
      toast.error(message, {
        description: 'Ensure 5+ photos, cover image, and full details.'
      });
    }
  };

  if (isLoading || !property) return (
    <div className="flex flex-col items-center justify-center py-20 gap-4">
        <Loading03Icon className="h-10 w-10 animate-spin text-rausch" />
        <p className="text-slate-400 font-medium">Fetching listing details...</p>
    </div>
  );

  const tabs: { id: TabType, label: string, icon: any }[] = [
    { id: 'general', label: 'Basic Info', icon: Settings01Icon },
    { id: 'photos', label: 'Photos', icon: Image01Icon },
    { id: 'amenities', label: 'Amenities', icon: Task01Icon },
    { id: 'location', label: 'Location', icon: Location01Icon },
    { id: 'calendar', label: 'Calendar', icon: Calendar03Icon },
  ];

  return (
    <div className="max-w-6xl mx-auto pb-20">
      {/* Top Navigation */}
      <div className="flex items-center gap-4 mb-8">
         <Button variant="ghost" className="rounded-full h-10 w-10 p-0" asChild>
            <a href="/host/properties"><ArrowLeft02Icon className="h-6 w-6" /></a>
         </Button>
         <div>
            <h1 className="text-2xl font-bold text-hof">Manage Listing</h1>
            <p className="text-sm text-slate-500">Edit your property details and manage availability.</p>
         </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* Left Sidebar: Controls & Status */}
        <div className="lg:col-span-4 space-y-6 lg:sticky lg:top-8">
            <div className="bg-white p-6 rounded-3xl border shadow-sm space-y-6">
                <div className="aspect-video rounded-2xl overflow-hidden border bg-slate-100 relative">
                    <img 
                        src={property.images?.find(i => i.type === 1)?.url || 'https://placehold.co/600x400?text=No+Cover'} 
                        className="h-full w-full object-cover"
                        alt="Cover"
                    />
                    <div className="absolute top-3 left-3">
                        <Badge className={`${getStatusColor(property.status)} border-none px-3 py-1 shadow-sm`}>
                            {getStatusText(property.status)}
                        </Badge>
                    </div>
                </div>

                <div>
                    <h2 className="text-xl font-bold text-hof line-clamp-1">{property.title}</h2>
                    <p className="text-sm text-slate-500 flex items-center gap-1 mt-1">
                        <Location01Icon className="h-4 w-4" />
                        {property.displayAddress || 'No location set'}
                    </p>
                </div>

                <div className="flex flex-col gap-3 pt-2">
                    {property.status === PropertyStatus.Draft && (
                        <Button 
                            onClick={handlePublish}
                            disabled={updateStatusMutation.isPending}
                            className="bg-rausch hover:bg-rausch-dark text-white rounded-2xl h-14 font-bold text-lg shadow-lg shadow-rausch/20"
                        >
                            {updateStatusMutation.isPending ? <Loading03Icon className="h-6 w-6 animate-spin" /> : <Tick02Icon className="h-6 w-6" />}
                            Publish Listing
                        </Button>
                    )}
                    <Button variant="outline" className="rounded-2xl h-12 border-slate-200 font-bold flex gap-2" asChild>
                        <a href={`/properties/${propertyId}`} target="_blank">
                            <ViewIcon className="h-5 w-5" />
                            Preview Live
                        </a>
                    </Button>
                </div>
            </div>

            {/* Tab Navigation Menu */}
            <div className="bg-white rounded-3xl border shadow-sm overflow-hidden p-2">
                {tabs.map(tab => (
                    <button
                        key={tab.id}
                        onClick={() => setActiveTab(tab.id)}
                        className={`
                            w-full flex items-center gap-4 px-6 py-4 rounded-2xl font-bold transition-all
                            ${activeTab === tab.id 
                                ? 'bg-hof text-white shadow-md' 
                                : 'text-slate-500 hover:bg-slate-50 hover:text-hof'
                            }
                        `}
                    >
                        <tab.icon className={`h-5 w-5 ${activeTab === tab.id ? 'text-rausch' : 'text-slate-300'}`} />
                        {tab.label}
                    </button>
                ))}
            </div>
        </div>

        {/* Right Content Area */}
        <div className="lg:col-span-8 bg-white p-8 rounded-[2rem] border shadow-sm min-h-[600px]">
            {activeTab === 'general' && (
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
                    <div className="space-y-4">
                        <h3 className="text-xl font-bold text-hof">Basic Information</h3>
                        <div className="space-y-1">
                            <label className="text-xs font-bold uppercase text-slate-400 tracking-wider">Listing Title</label>
                            <input 
                                {...register('title')}
                                className="w-full text-lg p-4 rounded-2xl border-2 border-slate-100 focus:border-rausch outline-none transition-all font-medium"
                                placeholder="Catchy title for your home"
                            />
                            {errors.title && <p className="text-xs text-rausch font-semibold mt-1">{errors.title.message}</p>}
                        </div>
                        
                        <div className="space-y-1">
                            <label className="text-xs font-bold uppercase text-slate-400 tracking-wider">Description</label>
                            <textarea 
                                {...register('description')}
                                rows={6}
                                className="w-full p-4 rounded-2xl border-2 border-slate-100 focus:border-rausch outline-none transition-all text-hof leading-relaxed"
                                placeholder="Tell guests what makes your place special..."
                            />
                            {errors.description && <p className="text-xs text-rausch font-semibold mt-1">{errors.description.message}</p>}
                        </div>

                        <div className="space-y-1">
                            <label className="text-xs font-bold uppercase text-slate-400 tracking-wider">Property Type</label>
                            <select 
                                {...register('type', { valueAsNumber: true })}
                                className="w-full p-4 rounded-2xl border-2 border-slate-100 focus:border-rausch outline-none transition-all text-hof font-medium bg-white appearance-none"
                            >
                                {Object.entries(PropertyType).map(([key, value]) => (
                                    <option key={value as number} value={value as number}>{key}</option>
                                ))}
                            </select>
                            {errors.type && <p className="text-xs text-rausch font-semibold mt-1">{errors.type.message}</p>}
                        </div>
                    </div>

                    <div className="grid grid-cols-2 gap-6">
                        <div className="space-y-1">
                            <label className="text-xs font-bold uppercase text-slate-400 tracking-wider">Base Price (USD)</label>
                            <input 
                                type="number"
                                {...register('basePrice', { valueAsNumber: true })}
                                className="w-full p-4 rounded-2xl border-2 border-slate-100 focus:border-rausch outline-none transition-all font-bold"
                            />
                        </div>
                        <div className="space-y-1">
                            <label className="text-xs font-bold uppercase text-slate-400 tracking-wider">Cleaning Fee (USD)</label>
                            <input 
                                type="number"
                                {...register('cleaningFee', { valueAsNumber: true })}
                                className="w-full p-4 rounded-2xl border-2 border-slate-100 focus:border-rausch outline-none transition-all font-bold"
                            />
                        </div>
                    </div>

                    <Button 
                        type="submit" 
                        disabled={updateMutation.isPending}
                        className="bg-hof hover:bg-slate-800 text-white rounded-2xl h-14 px-10 font-bold flex gap-2"
                    >
                        {updateMutation.isPending && <Loading03Icon className="h-5 w-5 animate-spin" />}
                        Save Changes
                    </Button>
                </form>
            )}

            {activeTab === 'photos' && (
                <ImageManager propertyId={propertyId} images={property.images || []} />
            )}

            {activeTab === 'amenities' && (
                <AmenityManager propertyId={propertyId} selectedAmenities={property.propertyAmenities || []} />
            )}

            {activeTab === 'location' && (
                <LocationManager 
                    propertyId={propertyId} 
                    initialLat={property.latitude} 
                    initialLng={property.longitude}
                    initialAddress={property.displayAddress || ''}
                    initialSubDivisions={property.subDivisions}
                />
            )}

            {activeTab === 'calendar' && (
                <AvailabilityCalendar 
                    propertyId={propertyId} 
                    availabilities={property.availabilities || []} 
                />
            )}
        </div>
      </div>
    </div>
  );
};
