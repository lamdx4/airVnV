import React, { useCallback } from 'react';
import { Button } from '@/components/ui/button';
import { 
  CloudUploadIcon, 
  Delete02Icon, 
  Image01Icon, 
  Loading03Icon,
  PlusSignIcon
} from 'hugeicons-react';
import { PropertyImage, ImageType } from '../types';
import { useAddImages, useRemoveImage } from '../hooks/useProperties';
import { toast } from 'sonner';

interface ImageManagerProps {
  propertyId: string;
  images: PropertyImage[];
}

export const ImageManager: React.FC<ImageManagerProps> = ({ propertyId, images }) => {
  const addImagesMutation = useAddImages();
  const removeImageMutation = useRemoveImage();
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    if (files.length === 0) return;

    try {
      await addImagesMutation.mutateAsync({
        propertyId,
        files,
        type: ImageType.Gallery // Default to gallery for bulk upload
      });
      toast.success(`Successfully uploaded ${files.length} images`);
      if (fileInputRef.current) fileInputRef.current.value = '';
    } catch (err: any) {
      toast.error('Failed to upload images');
    }
  };

  const handleRemove = async (imageId: string) => {
    try {
      await removeImageMutation.mutateAsync({ propertyId, imageId });
      toast.success('Image removed');
    } catch (err: any) {
      toast.error('Failed to remove image');
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold">Property Photos</h3>
          <p className="text-sm text-muted-foreground">Add at least 5 photos to get started.</p>
        </div>
        <Button 
          onClick={() => fileInputRef.current?.click()}
          disabled={addImagesMutation.isPending}
          className="bg-slate-900 hover:bg-slate-800 text-white rounded-xl"
        >
          {addImagesMutation.isPending ? (
            <Loading03Icon className="mr-2 h-4 w-4 animate-spin" />
          ) : (
            <CloudUploadIcon className="mr-2 h-4 w-4" />
          )}
          Upload Photos
        </Button>
        <input 
          type="file" 
          multiple 
          accept="image/*" 
          className="hidden" 
          ref={fileInputRef}
          onChange={handleFileSelect}
        />
      </div>

      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {/* Upload Placeholder */}
        <div 
          onClick={() => fileInputRef.current?.click()}
          className="aspect-square border-2 border-dashed border-slate-200 rounded-2xl flex flex-col items-center justify-center gap-2 hover:border-rausch hover:bg-rausch/5 transition-all cursor-pointer group"
        >
          <div className="p-3 bg-slate-100 rounded-full group-hover:bg-rausch/10 transition-all">
            <PlusSignIcon className="h-6 w-6 text-slate-400 group-hover:text-rausch" />
          </div>
          <span className="text-xs font-medium text-slate-500 group-hover:text-rausch">Add more</span>
        </div>

        {/* Image Grid */}
        {images.sort((a, b) => a.order - b.order).map((image) => (
          <div key={image.id} className="group relative aspect-square rounded-2xl overflow-hidden border border-slate-100 shadow-sm">
            <img 
              src={image.url} 
              alt="Property" 
              className="w-full h-full object-cover transition-transform group-hover:scale-105"
            />
            <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-2">
               <Button 
                variant="destructive" 
                size="icon" 
                className="h-9 w-9 rounded-xl shadow-lg"
                onClick={() => handleRemove(image.id)}
                disabled={removeImageMutation.isPending}
               >
                 <Delete02Icon className="h-4 w-4" />
               </Button>
            </div>
            {image.type === 0 && (
              <div className="absolute top-2 left-2 bg-white/90 backdrop-blur px-2 py-1 rounded-lg text-[10px] font-bold shadow-sm">
                COVER IMAGE
              </div>
            )}
          </div>
        ))}
      </div>

      {images.length === 0 && !addImagesMutation.isPending && (
        <div className="py-12 flex flex-col items-center justify-center text-slate-400 gap-2">
          <Image01Icon className="h-12 w-12 opacity-20" />
          <p className="text-sm">No photos uploaded yet</p>
        </div>
      )}
    </div>
  );
};
