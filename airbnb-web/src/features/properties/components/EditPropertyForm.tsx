import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Switch } from '@/components/ui/switch';
import { 
  Information01Icon, 
  Dollar01Icon, 
  Image01Icon, 
  Task01Icon,
  Tick02Icon,
  Loading03Icon
} from 'hugeicons-react';
import { useProperty, useUpdateProperty } from '../hooks/useProperties';
import { ImageManager } from './ImageManager';
import { toast } from 'sonner';

const schema = z.object({
  title: z.string().min(10, 'Title must be at least 10 characters'),
  description: z.string().min(20, 'Description must be at least 20 characters'),
  basePrice: z.number().min(1),
  cleaningFee: z.number().min(0),
  guestCount: z.number().min(1),
  bedroomCount: z.number().min(1),
  allowPets: z.boolean(),
  allowSmoking: z.boolean(),
  checkInTime: z.string(),
  checkOutTime: z.string(),
});

type FormData = z.infer<typeof schema>;

export const EditPropertyForm: React.FC<{ propertyId: string }> = ({ propertyId }) => {
  const { data: property, isLoading } = useProperty(propertyId);
  const updateMutation = useUpdateProperty();

  const { register, handleSubmit, formState: { errors, isDirty }, reset, setValue, watch } = useForm<FormData>({
    resolver: zodResolver(schema)
  });

  // Sync data when loaded
  React.useEffect(() => {
    if (property) {
      reset({
        title: property.title,
        description: property.description,
        basePrice: property.pricing.basePrice,
        cleaningFee: property.pricing.cleaningFee,
        guestCount: property.capacity.guestCount,
        bedroomCount: property.capacity.bedroomCount,
        allowPets: property.houseRules.allowPets,
        allowSmoking: property.houseRules.allowSmoking,
        checkInTime: property.houseRules.checkInTime,
        checkOutTime: property.houseRules.checkOutTime,
      });
    }
  }, [property, reset]);

  const onSubmit = async (data: FormData) => {
    try {
      await updateMutation.mutateAsync({
        id: propertyId,
        data: {
          ...data,
          // We map the structure to match what UpdateProperty Handler expects
          pricing: {
            basePrice: data.basePrice,
            cleaningFee: data.cleaningFee
          },
          capacity: {
            guestCount: data.guestCount,
            bedroomCount: data.bedroomCount
          },
          houseRules: {
            allowPets: data.allowPets,
            allowSmoking: data.allowSmoking,
            checkInTime: data.checkInTime,
            checkOutTime: data.checkOutTime
          }
        }
      });
      toast.success('Property updated successfully!');
    } catch (err: any) {
      toast.error(err.message || 'Failed to update property');
    }
  };

  if (isLoading) return (
    <div className="flex flex-col items-center justify-center h-64 gap-4">
      <Loading03Icon className="h-8 w-8 animate-spin text-rausch" />
      <p className="text-muted-foreground animate-pulse">Loading listing details...</p>
    </div>
  );

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-8 pb-20">
      <Tabs defaultValue="basic" className="w-full">
        <TabsList className="grid w-full grid-cols-4 h-14 bg-slate-100/50 p-1 rounded-2xl border border-slate-200">
          <TabsTrigger value="basic" className="rounded-xl data-[state=active]:bg-white data-[state=active]:shadow-sm">
            <Information01Icon className="mr-2 h-4 w-4" /> Basic Info
          </TabsTrigger>
          <TabsTrigger value="pricing" className="rounded-xl data-[state=active]:bg-white data-[state=active]:shadow-sm">
            <Dollar01Icon className="mr-2 h-4 w-4" /> Pricing & Rules
          </TabsTrigger>
          <TabsTrigger value="images" className="rounded-xl data-[state=active]:bg-white data-[state=active]:shadow-sm">
            <Image01Icon className="mr-2 h-4 w-4" /> Images
          </TabsTrigger>
          <TabsTrigger value="amenities" className="rounded-xl data-[state=active]:bg-white data-[state=active]:shadow-sm">
            <Task01Icon className="mr-2 h-4 w-4" /> Amenities
          </TabsTrigger>
        </TabsList>

        <div className="mt-8">
          <TabsContent value="basic" className="space-y-6">
            <Card className="border-slate-200 shadow-sm rounded-2xl overflow-hidden">
              <CardHeader className="bg-slate-50/50 border-b border-slate-100">
                <CardTitle>General Information</CardTitle>
                <CardDescription>Update your listing title and description.</CardDescription>
              </CardHeader>
              <CardContent className="pt-6 space-y-4">
                <div className="space-y-2">
                  <label className="text-sm font-semibold">Listing Title</label>
                  <Input {...register('title')} placeholder="e.g. Luxury Villa with Pool" />
                  {errors.title && <p className="text-xs text-red-500">{errors.title.message}</p>}
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-semibold">Description</label>
                  <Textarea {...register('description')} rows={6} placeholder="Describe your place..." />
                  {errors.description && <p className="text-xs text-red-500">{errors.description.message}</p>}
                </div>
              </CardContent>
            </Card>

            <Card className="border-slate-200 shadow-sm rounded-2xl overflow-hidden">
              <CardHeader className="bg-slate-50/50 border-b border-slate-100">
                <CardTitle>Capacity</CardTitle>
                <CardDescription>How many guests can your place accommodate?</CardDescription>
              </CardHeader>
              <CardContent className="pt-6 grid grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label className="text-sm font-semibold">Guest Count</label>
                  <Input type="number" {...register('guestCount', { valueAsNumber: true })} />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-semibold">Bedroom Count</label>
                  <Input type="number" {...register('bedroomCount', { valueAsNumber: true })} />
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="pricing" className="space-y-6">
            <Card className="border-slate-200 shadow-sm rounded-2xl overflow-hidden">
              <CardHeader className="bg-slate-50/50 border-b border-slate-100">
                <CardTitle>Pricing Settings</CardTitle>
                <CardDescription>Set your nightly price and fees.</CardDescription>
              </CardHeader>
              <CardContent className="pt-6 grid grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label className="text-sm font-semibold">Base Price ($ / night)</label>
                  <Input type="number" {...register('basePrice', { valueAsNumber: true })} />
                </div>
                <div className="space-y-2">
                  <label className="text-sm font-semibold">Cleaning Fee ($)</label>
                  <Input type="number" {...register('cleaningFee', { valueAsNumber: true })} />
                </div>
              </CardContent>
            </Card>

            <Card className="border-slate-200 shadow-sm rounded-2xl overflow-hidden">
              <CardHeader className="bg-slate-50/50 border-b border-slate-100">
                <CardTitle>House Rules</CardTitle>
                <CardDescription>What are the rules for your guests?</CardDescription>
              </CardHeader>
              <CardContent className="pt-6 space-y-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="font-semibold">Allow Pets</p>
                    <p className="text-sm text-muted-foreground">Can guests bring their furry friends?</p>
                  </div>
                  <Switch 
                    checked={watch('allowPets')} 
                    onCheckedChange={(checked) => setValue('allowPets', checked, { shouldDirty: true })} 
                  />
                </div>
                <div className="flex items-center justify-between">
                  <div>
                    <p className="font-semibold">Allow Smoking</p>
                    <p className="text-sm text-muted-foreground">Is smoking permitted inside?</p>
                  </div>
                  <Switch 
                    checked={watch('allowSmoking')} 
                    onCheckedChange={(checked) => setValue('allowSmoking', checked, { shouldDirty: true })} 
                  />
                </div>
                <div className="grid grid-cols-2 gap-6 pt-4 border-t">
                  <div className="space-y-2">
                    <label className="text-sm font-semibold">Check-in Time</label>
                    <Input {...register('checkInTime')} placeholder="e.g. 14:00" />
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-semibold">Check-out Time</label>
                    <Input {...register('checkOutTime')} placeholder="e.g. 11:00" />
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="images">
             <Card className="border-slate-200 shadow-sm rounded-2xl overflow-hidden">
               <CardContent className="pt-6">
                 <ImageManager propertyId={propertyId} images={property.images} />
               </CardContent>
             </Card>
          </TabsContent>

          <TabsContent value="amenities">
             <div className="p-8 border-2 border-dashed rounded-3xl text-center space-y-4 bg-slate-50/50">
                <Task01Icon className="mx-auto h-12 w-12 text-slate-300" />
                <div>
                  <h3 className="text-lg font-semibold">Amenities Coming Soon</h3>
                  <p className="text-muted-foreground">Manage property features and comforts.</p>
                </div>
             </div>
          </TabsContent>
        </div>
      </Tabs>

      {/* Floating Action Bar */}
      <div className="fixed bottom-8 left-1/2 -translate-x-1/2 w-full max-w-lg px-4 z-50">
        <div className="bg-white/80 backdrop-blur-xl border border-slate-200 shadow-2xl rounded-2xl p-4 flex items-center justify-between gap-4">
          <p className="text-sm font-medium text-slate-500 pl-2">
            {isDirty ? 'You have unsaved changes' : 'All changes saved'}
          </p>
          <Button 
            type="submit" 
            disabled={!isDirty || updateMutation.isPending}
            className="bg-rausch hover:bg-rausch/90 min-w-[140px] rounded-xl shadow-lg shadow-rausch/20 transition-all active:scale-95"
          >
            {updateMutation.isPending ? (
              <Loading03Icon className="mr-2 h-4 w-4 animate-spin" />
            ) : (
              <Tick02Icon className="mr-2 h-4 w-4" />
            )}
            Save Changes
          </Button>
        </div>
      </div>
    </form>
  );
};
