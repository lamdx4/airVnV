import React, { useState, useEffect } from 'react';
import { 
  DndContext, 
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  rectSortingStrategy,
  useSortable
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { 
  ImageAdd01Icon, 
  Delete02Icon, 
  Loading03Icon,
  DragDropIcon,
  StarIcon
} from 'hugeicons-react';
import type { PropertyImage } from '../types';
import { useAddImages, useRemoveImage, useReorderImages } from '../hooks/useProperties';
import { toast } from 'sonner';
import { useTranslation } from 'react-i18next';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';


const getImageTypeColor = (type: number): string => {
  switch (type) {
    case 0: return 'bg-rose-50 text-rose-600 border border-rose-100'; // Cover red
    case 1: return 'bg-slate-50/90 text-slate-600 border border-slate-200'; // Gallery gray
    case 2: return 'bg-blue-50/90 text-blue-600 border border-blue-100'; // Room blue
    case 3: return 'bg-emerald-50/90 text-emerald-600 border border-emerald-100'; // Bathroom green
    case 4: return 'bg-purple-50/90 text-purple-600 border border-purple-100'; // View purple
    default: return 'bg-slate-50/90 text-slate-600 border border-slate-200';
  }
};

interface ImageManagerProps {
  propertyId: string;
  images: PropertyImage[];
}

interface SortableImageProps {
  image: PropertyImage;
  onRemove: (id: string) => void;
  isRemoving: boolean;
}

const SortableImage: React.FC<SortableImageProps> = ({ image, onRemove, isRemoving }) => {
  const { t } = useTranslation();
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging
  } = useSortable({ id: image.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    zIndex: isDragging ? 50 : 'auto',
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div 
      ref={setNodeRef} 
      style={style}
      className="group relative aspect-[4/3] rounded-2xl overflow-hidden border bg-slate-100 shadow-sm transition-all hover:shadow-md"
    >
      <img 
        src={image.url} 
        alt="Property" 
        className="h-full w-full object-cover"
      />
      
      {/* Drag Handle Overlay */}
      <div 
        {...attributes} 
        {...listeners}
        className="absolute inset-0 bg-black/0 group-hover:bg-black/10 transition-colors cursor-grab active:cursor-grabbing flex items-center justify-center opacity-0 group-hover:opacity-100"
      >
        <div className="p-2 bg-white/90 rounded-full shadow-lg">
          <DragDropIcon className="h-5 w-5 text-hof" />
        </div>
      </div>

      {/* Badge & Actions */}
      <div className="absolute top-3 left-3 flex gap-2">
        <div className={`backdrop-blur px-3 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider flex items-center gap-1 shadow-sm ${getImageTypeColor(image.type)}`}>
          {image.type === 0 && <StarIcon className="h-3 w-3 fill-rose-600 text-rose-600" />}
          {image.type === 0 ? t('photos.cover') : 
           image.type === 1 ? t('photos.gallery') : 
           image.type === 2 ? t('photos.room') : 
           image.type === 3 ? t('photos.bathroom') : 
           image.type === 4 ? t('photos.view') : t('photos.gallery')}
        </div>
      </div>

      <button
        onClick={(e) => {
          e.stopPropagation();
          onRemove(image.id);
        }}
        disabled={isRemoving}
        className="absolute top-3 right-3 p-2 bg-white/95 backdrop-blur hover:bg-white text-slate-400 hover:text-rausch rounded-xl shadow-sm transition-all opacity-0 group-hover:opacity-100"
      >
        <Delete02Icon className="h-5 w-5" />
      </button>
    </div>
  );
};

export const ImageManager: React.FC<ImageManagerProps> = ({ propertyId, images }) => {
  const { t } = useTranslation();
  const addImagesMutation = useAddImages();
  const removeImageMutation = useRemoveImage();
  const reorderImagesMutation = useReorderImages();

  // Local state for immediate UI feedback during drag
  const [items, setItems] = useState<PropertyImage[]>([]);
  const [uploadType, setUploadType] = useState<number>(1);

  // Sync local state and auto-configure upload category defaults
  useEffect(() => {
    setItems([...images].sort((a, b) => a.displayOrder - b.displayOrder));
    
    // Default to Cover (0) if there isn't one yet, otherwise Gallery (1)
    const hasCover = images.some(i => i.type === 0);
    setUploadType(hasCover ? 1 : 0);
  }, [images]);

  const sensors = useSensors(
    useSensor(PointerSensor, {
        activationConstraint: {
            distance: 8,
        },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (over && active.id !== over.id) {
      const oldIndex = items.findIndex((item) => item.id === active.id);
      const newIndex = items.findIndex((item) => item.id === over.id);

      const newItems = arrayMove(items, oldIndex, newIndex);
      setItems(newItems);

      const orders = newItems.map((item, index) => ({
        imageId: item.id,
        displayOrder: index
      }));

      try {
        await reorderImagesMutation.mutateAsync({ propertyId, orders });
        toast.success(t('photos.orderSaved'));
      } catch (err) {
        toast.error(t('photos.orderSaveFailed'));
        setItems([...images].sort((a, b) => a.displayOrder - b.displayOrder));
      }
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    if (files.length === 0) return;

    try {
      await addImagesMutation.mutateAsync({ propertyId, files, type: uploadType });
      toast.success(t('photos.uploadSuccess'));
    } catch (err) {
      toast.error(t('photos.uploadFailed'));
    }
  };

  const handleRemove = async (imageId: string) => {
    try {
      await removeImageMutation.mutateAsync({ propertyId, imageId });
      toast.success(t('photos.removeSuccess'));
    } catch (err) {
      toast.error(t('photos.removeFailed'));
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h3 className="text-lg font-bold text-hof">{t('photos.title')}</h3>
          <p className="text-sm text-slate-500">{t('photos.subtitle')}</p>
        </div>
        
        <div className="flex items-center gap-3">
          <div className="w-48">
            <Select 
              value={uploadType.toString()} 
              onValueChange={(val) => setUploadType(parseInt(val))}
            >
              <SelectTrigger className="rounded-xl border-slate-200 bg-white">
                <SelectValue placeholder={t('photos.uploadCategory')} />
              </SelectTrigger>
              <SelectContent className="rounded-xl">
                <SelectItem value="0">{t('photos.coverImage')}</SelectItem>
                <SelectItem value="1">{t('photos.generalGallery')}</SelectItem>
                <SelectItem value="2">{t('photos.rooms')}</SelectItem>
                <SelectItem value="3">{t('photos.bathrooms')}</SelectItem>
                <SelectItem value="4">{t('photos.views')}</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="relative">
            <input
              type="file"
              multiple
              accept="image/*"
              onChange={handleFileUpload}
              className="absolute inset-0 opacity-0 cursor-pointer"
              disabled={addImagesMutation.isPending}
            />
            <button 
              disabled={addImagesMutation.isPending}
              className="flex items-center gap-2 px-4 py-2 bg-slate-900 text-white rounded-xl hover:bg-slate-800 transition-colors disabled:bg-slate-300 font-medium text-sm"
            >
              {addImagesMutation.isPending ? <Loading03Icon className="h-5 w-5 animate-spin" /> : <ImageAdd01Icon className="h-5 w-5" />}
              <span>{t('photos.uploadPhotos')}</span>
            </button>
          </div>
        </div>
      </div>

      <DndContext 
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
      >
        <SortableContext 
          items={items.map(i => i.id)}
          strategy={rectSortingStrategy}
        >
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
            {items.map((image) => (
              <SortableImage 
                key={image.id} 
                image={image} 
                onRemove={handleRemove}
                isRemoving={removeImageMutation.isPending}
              />
            ))}
            
            {items.length < 10 && (
                <div className="relative aspect-[4/3] rounded-2xl border-2 border-dashed border-slate-200 flex flex-col items-center justify-center gap-2 text-slate-400 hover:border-rausch hover:text-rausch transition-all bg-slate-50/50">
                    <input
                        type="file"
                        multiple
                        accept="image/*"
                        onChange={handleFileUpload}
                        className="absolute inset-0 opacity-0 cursor-pointer"
                    />
                    <ImageAdd01Icon className="h-8 w-8" />
                    <span className="text-xs font-semibold">{t('photos.addMore')}</span>
                </div>
            )}
          </div>
        </SortableContext>
      </DndContext>

      {items.length === 0 && (
        <div className="py-20 text-center bg-slate-50 rounded-3xl border-2 border-dashed flex flex-col items-center gap-4">
          <div className="p-4 bg-white rounded-2xl shadow-sm">
            <ImageAdd01Icon className="h-10 w-10 text-slate-300" />
          </div>
          <div className="space-y-1">
            <p className="font-bold text-hof">{t('photos.emptyTitle')}</p>
            <p className="text-sm text-slate-500">{t('photos.emptySubtitle')}</p>
          </div>
        </div>
      )}
    </div>
  );
};
