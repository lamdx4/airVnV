import { useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ImagePlus, X, AlertCircle, Star } from 'lucide-react';
import { toast } from 'sonner';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

const getImageTypeText = (type: number): string => {
  switch (type) {
    case 0: return 'Cover';
    case 1: return 'Gallery';
    case 2: return 'Room';
    case 3: return 'Bathroom';
    case 4: return 'View';
    default: return 'Gallery';
  }
};

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

const getImageTypeBorder = (type: number): string => {
  switch (type) {
    case 0: return 'border-rose-300 ring-2 ring-rose-500/20'; // Cover highlighted border
    default: return 'border-slate-200';
  }
};

interface PhotosSectionProps {
  selectedFiles: { file: File; type: number; id: string; }[];
  setSelectedFiles: React.Dispatch<React.SetStateAction<{ file: File; type: number; id: string; }[]>>;
  onContinue: () => void;
}

export function PhotosSection({
  selectedFiles,
  setSelectedFiles,
  onContinue,
}: PhotosSectionProps) {
  const { t } = useTranslation();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const newFiles = Array.from(e.target.files);
      
      // Filter valid types and sizes
      const validFiles = newFiles.filter(file => {
        const isImage = file.type.startsWith('image/');
        const isUnder5MB = file.size <= 5 * 1024 * 1024;
        if (!isImage) toast.error(`File ${file.name} is not an image.`);
        if (!isUnder5MB) toast.error(`File ${file.name} exceeds 5MB limit.`);
        return isImage && isUnder5MB;
      });

      setSelectedFiles(prev => {
        const hasCover = prev.some(f => f.type === 0);
        
        const mapped = validFiles.map((file, i) => {
          // If no cover exists, the very first file becomes Cover (0), others Gallery (1)
          const isCover = !hasCover && prev.length === 0 && i === 0;
          return {
            file,
            type: isCover ? 0 : 1,
            id: Math.random().toString(36).substring(2, 9)
          };
        });
        
        return [...prev, ...mapped];
      });

      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const removeFile = (id: string) => {
    setSelectedFiles(prev => {
      const remaining = prev.filter(f => f.id !== id);
      
      // If we removed the cover image, automatically promote the next remaining image to Cover
      const hasCover = remaining.some(f => f.type === 0);
      if (!hasCover && remaining.length > 0) {
        remaining[0] = { ...remaining[0], type: 0 };
      }
      return remaining;
    });
  };

  const handleTypeChange = (id: string, newType: number) => {
    setSelectedFiles(prev => {
      return prev.map(item => {
        if (item.id === id) {
          return { ...item, type: newType };
        }
        // If the new type is Cover (0), change all other old covers to Gallery (1)
        if (newType === 0 && item.type === 0) {
          return { ...item, type: 1 };
        }
        return item;
      });
    });
  };

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">Thêm hình ảnh cho nơi ở của bạn</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">
          Bạn cần ít nhất 5 bức ảnh để bắt đầu. Vui lòng phân loại ảnh để khách hàng dễ dàng hình dung không gian của bạn.
        </p>
      </CardHeader>
      <CardContent className="px-6 py-6 space-y-6">
        <div 
          className="border-2 border-dashed border-slate-300 rounded-xl p-8 flex flex-col items-center justify-center bg-slate-50 hover:bg-slate-100 transition-colors cursor-pointer"
          onClick={() => fileInputRef.current?.click()}
        >
          <div className="h-14 w-14 rounded-full bg-pink-50 flex items-center justify-center mb-4 text-[#FF5A5F]">
            <ImagePlus className="h-7 w-7" />
          </div>
          <span className="text-slate-900 font-semibold mb-1">Click để thêm ảnh</span>
          <span className="text-slate-500 text-sm">PNG, JPG hoặc WEBP (Tối đa 5MB)</span>
          <input
            type="file"
            ref={fileInputRef}
            onChange={handleFileChange}
            multiple
            accept="image/jpeg, image/png, image/webp"
            className="hidden"
          />
        </div>

        {selectedFiles.length > 0 && (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4 mt-6">
            {selectedFiles.map((item) => {
              const previewUrl = URL.createObjectURL(item.file);
              return (
                <div 
                  key={item.id} 
                  className={`relative aspect-[4/3] rounded-2xl overflow-hidden group border shadow-sm transition-all duration-300 ${getImageTypeBorder(item.type)}`}
                >
                  <img 
                    src={previewUrl} 
                    alt="preview" 
                    className="w-full h-full object-cover"
                  />
                  
                  {/* Category Selector overlay at bottom */}
                  <div className="absolute inset-x-0 bottom-0 bg-slate-950/85 backdrop-blur-xs p-1.5 flex items-center justify-between opacity-100 sm:opacity-0 sm:group-hover:opacity-100 transition-opacity">
                    <Select 
                      value={item.type.toString()} 
                      onValueChange={(val) => handleTypeChange(item.id, parseInt(val))}
                    >
                      <SelectTrigger className="h-7 w-full rounded-lg text-[10px] font-semibold bg-white border-none text-slate-900 shadow-sm py-0">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent className="rounded-xl">
                        <SelectItem value="0">⭐ Cover Image</SelectItem>
                        <SelectItem value="1">🖼️ General Gallery</SelectItem>
                        <SelectItem value="2">🛏️ Rooms & Spaces</SelectItem>
                        <SelectItem value="3">🚿 Bathrooms</SelectItem>
                        <SelectItem value="4">🌅 Scenic Views</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Badge displaying type */}
                  <div className="absolute top-2 left-2 flex gap-1">
                    <div className={`backdrop-blur px-2 py-0.5 rounded-full text-[9px] font-extrabold uppercase tracking-wider flex items-center gap-0.5 shadow-sm ${getImageTypeColor(item.type)}`}>
                      {item.type === 0 && <Star className="h-2.5 w-2.5 fill-rose-600 text-rose-600 animate-pulse" />}
                      {getImageTypeText(item.type)}
                    </div>
                  </div>

                  {/* Remove Button overlay */}
                  <Button
                    type="button"
                    variant="destructive"
                    size="icon"
                    className="absolute top-2 right-2 h-7 w-7 rounded-full opacity-0 group-hover:opacity-100 transition-opacity shadow-md"
                    onClick={(e) => {
                      e.stopPropagation();
                      removeFile(item.id);
                    }}
                  >
                    <X className="h-3.5 w-3.5" />
                  </Button>
                </div>
              );
            })}
          </div>
        )}

        {selectedFiles.length > 0 && selectedFiles.length < 5 && (
          <div className="flex items-center gap-2 text-amber-600 text-sm font-medium mt-4 p-3 bg-amber-50 rounded-lg">
            <AlertCircle className="h-5 w-5" />
            Vui lòng chọn thêm {5 - selectedFiles.length} ảnh nữa để có thể đăng.
          </div>
        )}

        <Button
          type="button"
          onClick={onContinue}
          disabled={selectedFiles.length < 5}
          className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm mt-6"
        >
          {t('location.continue')}
        </Button>
      </CardContent>
    </Card>
  );
}
