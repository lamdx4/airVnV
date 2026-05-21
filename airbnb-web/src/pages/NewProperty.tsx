import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useMutation, useQueryClient, useQuery } from '@tanstack/react-query';
import { motion, AnimatePresence } from 'framer-motion';
import { useTranslation } from 'react-i18next';
import { propertiesApi } from '@/features/properties/api/properties';
import { masterDataApi } from '@/lib/master-data-api';
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
  Lock
} from 'lucide-react';

// shadcn/ui components
import { Button } from '@/components/ui/button';
import { Card, CardHeader, CardContent, CardFooter } from '@/components/ui/card';

// Section components
import { BasicInfoSection } from '@/features/properties/components/new-property/BasicInfoSection';
import { PricingSection } from '@/features/properties/components/new-property/PricingSection';
import { LocationSection } from '@/features/properties/components/new-property/LocationSection';
import { AmenitiesSection } from '@/features/properties/components/new-property/AmenitiesSection';
import { HouseRulesSection } from '@/features/properties/components/new-property/HouseRulesSection';



const DEFAULT_FALLBACK_CONFIG = [
  { id: 'street', label: 'Street Address', photonKeys: ['street', 'name'], isRequired: true },
  { id: 'admin2', label: 'City', photonKeys: ['city', 'town', 'county'], isRequired: true },
  { id: 'admin1', label: 'State / Province', photonKeys: ['state'], isRequired: true },
  { id: 'zipcode', label: 'Zip / Postal Code', photonKeys: ['postcode'], isRequired: false }
];

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
  streetAddress: z.string().min(3, 'Street address is required'),
  countryCode: z.string().min(2),
  guestCount: z.number().min(1),
  bedroomCount: z.number().min(0),
  bedCount: z.number().min(0),
  bathroomCount: z.number().min(0),
  allowPets: z.boolean().default(false),
  allowSmoking: z.boolean().default(false),
  allowEvents: z.boolean().default(false),
  checkInTime: z.string().min(4),
  checkOutTime: z.string().min(4),
  flexibleCheckOut: z.boolean().default(false),
  customRules: z.array(z.string()).default([]),
});

type FormData = z.infer<typeof formSchema>;

export default function NewProperty() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { t, i18n } = useTranslation();
  
  // Dynamic Form Config States & Dynamic amenities
  const [selectedAmenities, setSelectedAmenities] = useState<string[]>([]);
  const [completedSections, setCompletedSections] = useState<Set<string>>(new Set());
  const [dynamicAddressValues, setDynamicAddressValues] = useState<Record<string, string>>({});

  const { register, handleSubmit, setValue, watch, control, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema) as any,
    defaultValues: {
      latitude: 21.0285,
      longitude: 105.8542,
      countryCode: 'VN',
      currencyCode: 'VND',
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
      streetAddress: '',
      customRules: [],
    }
  });

  const selectedCountryCode = watch('countryCode');

  // Load list of supported countries from API Master Data
  const { data: supportedCountries = [], isLoading: isLoadingCountries, isError: isCountriesError } = useQuery({
    queryKey: ['supportedCountries'],
    queryFn: () => masterDataApi.getSupportedCountries(),
    staleTime: 10 * 60 * 1000,
  });

  // Load Country Configuration purely via TanStack Query (Metadata-driven address form)
  const { data: countryMasterDataResponse, isLoading: isLoadingConfig, isError: isConfigError } = useQuery({
    queryKey: ['countryMasterData', selectedCountryCode],
    queryFn: () => propertiesApi.getCountryMasterData(selectedCountryCode),
    enabled: !!selectedCountryCode,
    staleTime: 5 * 60 * 1000,
  });

  // Derived address configuration from query server state
  const addressFormConfig = countryMasterDataResponse?.addressFormConfig && countryMasterDataResponse.addressFormConfig.length > 0
    ? countryMasterDataResponse.addressFormConfig
    : DEFAULT_FALLBACK_CONFIG;

  // Sync form defaults when TanStack Query returns fresh country master data
  useEffect(() => {
    if (countryMasterDataResponse) {
      const data = countryMasterDataResponse;
      setValue('currencyCode', data.nativeCurrency || 'USD');
      setValue('basePrice', data.nativeCurrency === 'USD' ? 100 : 1000000);

      if (data.defaultLatitude && data.defaultLongitude) {
        setValue('latitude', data.defaultLatitude);
        setValue('longitude', data.defaultLongitude);
      }
    }
  }, [countryMasterDataResponse, setValue]);

  // Reset dynamic address values when country changes to prevent data leak
  useEffect(() => {
    setDynamicAddressValues({});
  }, [selectedCountryCode]);

  // Fetch available amenities dynamic lists
  const { data: availableAmenities, isLoading: isLoadingAmenities, isError: isAmenitiesError } = useQuery({
    queryKey: ['amenities'],
    queryFn: () => propertiesApi.getAvailableAmenities()
  });

  const isSystemBlocked = isAmenitiesError || isConfigError || isCountriesError || !addressFormConfig || addressFormConfig.length === 0;

  const createMutation = useMutation({
    mutationFn: async (data: FormData) => {
      // Build display address dynamically based on config fields order
      const orderedParts: string[] = [];
      addressFormConfig.forEach((field: any) => {
        const val = dynamicAddressValues[field.id];
        if (val) {
          orderedParts.push(val);
        }
      });
      
      const countryName = supportedCountries.find(c => c.code === selectedCountryCode)?.name || '';
      orderedParts.push(countryName);

      const finalDisplayAddress = orderedParts.filter(Boolean).join(', ');

      const payload = toCreatePropertyRequest({
         ...data,
         displayAddress: finalDisplayAddress,
         admin1Code: dynamicAddressValues['admin1'] || dynamicAddressValues['admin1Code'],
         admin2Code: dynamicAddressValues['admin2'] || dynamicAddressValues['admin2Code'],
         subDivisions: dynamicAddressValues
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
    },
    onError: (err: any) => {
      const errMsg = err?.response?.data?.Message || 'Failed to create listing. Please try again.';
      toast.error(errMsg);
    }
  });

  const onSubmit = (data: FormData) => {
    createMutation.mutate(data);
  };

  const markSectionComplete = (section: string) => {
    setCompletedSections(prev => new Set(prev).add(section));
  };

  return (
    <div className="min-h-screen bg-linear-to-br from-background via-secondary to-background">
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
            <h1 className="text-3xl font-semibold text-slate-900 tracking-tight">{t('app.title')}</h1>
          </div>
          <div className="flex items-center gap-4">
             {/* Language Selector */}
             <div className="flex items-center gap-1 border border-slate-200 rounded-lg p-1 bg-white shadow-xs">
                <Button 
                  type="button"
                  variant={i18n.language.startsWith('en') ? 'default' : 'ghost'} 
                  size="sm" 
                  className={`h-8 rounded-md text-xs font-semibold px-3 ${i18n.language.startsWith('en') ? 'bg-[#FF5A5F] hover:bg-[#E04447]' : 'hover:bg-slate-50'}`}
                  onClick={() => {
                    i18n.changeLanguage('en');
                    queryClient.invalidateQueries();
                  }}
                >
                  EN
                </Button>
                <Button 
                  type="button"
                  variant={i18n.language.startsWith('vi') ? 'default' : 'ghost'} 
                  size="sm" 
                  className={`h-8 rounded-md text-xs font-semibold px-3 ${i18n.language.startsWith('vi') ? 'bg-[#FF5A5F] hover:bg-[#E04447]' : 'hover:bg-slate-50'}`}
                  onClick={() => {
                    i18n.changeLanguage('vi');
                    queryClient.invalidateQueries();
                  }}
                >
                  VI
                </Button>
             </div>
             <span className="text-sm font-semibold text-slate-500">{t('app.step', { step: completedSections.size + 1 })}</span>
             <Button variant="outline" className="h-10 rounded-lg text-sm font-semibold border-slate-300 hover:bg-slate-50 px-5">
                {t('app.saveExit')}
             </Button>
          </div>
        </div>
        <div className="h-[2px] bg-slate-100 w-full overflow-hidden">
            <motion.div 
                className="h-full bg-[#FF5A5F]"
                initial={{ width: 0 }}
                animate={{ width: `${(completedSections.size / 5) * 100}%` }}
                transition={{ duration: 0.4, ease: "easeOut" }}
            />
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          
          <div className="lg:col-span-2 space-y-8">
            <form onSubmit={handleSubmit(onSubmit as any)} className="space-y-8">
              
              <AnimatePresence mode="wait">
                {/* Section 1: The Basics (Title, Description, capacity) */}
                <BasicInfoSection 
                  register={register} 
                  watch={watch} 
                  setValue={setValue} 
                  errors={errors} 
                  onContinue={() => markSectionComplete('basic')} 
                />

                {/* Section 2: Pricing */}
                <PricingSection 
                  register={register} 
                  watch={watch} 
                  errors={errors} 
                  onContinue={() => markSectionComplete('capacity')} 
                />

                {/* Section 3: Location (Metadata-driven Address Form) */}
                <LocationSection 
                  setValue={setValue} 
                  watch={watch} 
                  errors={errors} 
                  control={control} 
                  addressFormConfig={addressFormConfig} 
                  isLoadingConfig={isLoadingConfig} 
                  isConfigError={isConfigError} 
                  dynamicAddressValues={dynamicAddressValues} 
                  setDynamicAddressValues={setDynamicAddressValues} 
                  onContinue={() => markSectionComplete('location')} 
                  supportedCountries={supportedCountries}
                  isLoadingCountries={isLoadingCountries}
                  isCountriesError={isCountriesError}
                />

                {/* Section 4: Amenities */}
                <AmenitiesSection 
                  isLoadingAmenities={isLoadingAmenities} 
                  isAmenitiesError={isAmenitiesError} 
                  availableAmenities={availableAmenities} 
                  selectedAmenities={selectedAmenities} 
                  setSelectedAmenities={setSelectedAmenities} 
                  onContinue={() => markSectionComplete('amenities')} 
                />

                {/* Section 5: House Rules */}
                <HouseRulesSection 
                  register={register} 
                  setValue={setValue} 
                  watch={watch} 
                />
              </AnimatePresence>

              {/* Submit Action */}
              <div className="pt-6">
                {isSystemBlocked && (
                  <div className="p-4 rounded-xl border border-red-200 bg-red-50/70 text-red-800 flex items-start gap-3 mb-4 animate-shake shadow-sm">
                    <AlertCircle className="h-5 w-5 text-red-600 mt-0.5 shrink-0" />
                    <div className="space-y-1 text-sm">
                      <h4 className="font-bold text-red-955">{t('rules.lockTitle')}</h4>
                      <p className="text-xs text-red-800 leading-relaxed">
                        {t('rules.lockSub')}
                      </p>
                      <ul className="list-disc list-inside text-xs text-red-800 pl-1 space-y-1">
                        {isCountriesError && (
                          <li>
                            <strong>Supported Countries Catalog:</strong> {t('rules.lockCountries')}
                          </li>
                        )}
                        {isAmenitiesError && (
                          <li>
                            <strong>Amenities Catalog:</strong> {t('rules.lockAmenities')}
                          </li>
                        )}
                        {isConfigError && (
                          <li>
                            <strong>Address Dynamic Config ({selectedCountryCode}):</strong> {t('rules.lockConfig', { countryCode: selectedCountryCode })}
                          </li>
                        )}
                        {(!addressFormConfig || addressFormConfig.length === 0) && (
                          <li>
                            <strong>Layout Integrity:</strong> {t('rules.lockIntegrity')}
                          </li>
                        )}
                      </ul>
                      <p className="text-[11px] text-red-600 mt-1 italic font-semibold">
                        {t('rules.lockFooter')}
                      </p>
                    </div>
                  </div>
                )}

                <Button
                  type="submit"
                  size="lg"
                  disabled={isSystemBlocked || createMutation.isPending}
                  className={`w-full h-14 rounded-xl font-bold text-lg shadow-md transition-all flex items-center justify-center gap-3 ${
                    isSystemBlocked 
                      ? 'bg-slate-200 text-slate-400 cursor-not-allowed hover:bg-slate-200 active:scale-100' 
                      : 'bg-[#FF5A5F] text-white hover:bg-[#E04447] active:scale-95'
                  }`}
                >
                  {createMutation.isPending ? (
                    <Loading03Icon className="h-6 w-6 animate-spin" />
                  ) : isSystemBlocked ? (
                    <>
                      <Lock className="h-5 w-5 text-slate-400" />
                      <span>{t('rules.publish')} (Locked)</span>
                    </>
                  ) : (
                    <>
                      <Zap className="h-5 w-5 fill-white/20" />
                      <span>{t('rules.publish')}</span>
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
