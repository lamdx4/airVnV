import { useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ImagePlus, X, AlertCircle } from 'lucide-react';
import { toast } from 'sonner';

interface PhotosSectionProps {
  selectedFiles: File[];
  setSelectedFiles: React.Dispatch<React.SetStateAction<File[]>>;
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

      setSelectedFiles(prev => [...prev, ...validFiles]);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const removeFile = (index: number) => {
    setSelectedFiles(prev => prev.filter((_, i) => i !== index));
  };

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">Thêm hình ảnh cho nơi ở của bạn</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">
          Bạn cần ít nhất 5 bức ảnh để bắt đầu. Hình ảnh đầu tiên sẽ được dùng làm ảnh bìa.
          <span className="block mt-1 text-xs text-rose-500 font-semibold">💡 Mẹo: Sau khi tạo xong tin đăng nháp, bạn có thể vào mục "Chỉnh sửa phòng" để tự do phân loại chi tiết từng bức ảnh cho phòng ngủ, phòng tắm, hoặc cảnh quan!</span>
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
            {selectedFiles.map((file, index) => (
              <div key={`${file.name}-${index}`} className="relative aspect-[4/3] rounded-lg overflow-hidden group border border-slate-200 shadow-sm">
                <img 
                  src={URL.createObjectURL(file)} 
                  alt="preview" 
                  className="w-full h-full object-cover"
                />
                <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                  <Button
                    type="button"
                    variant="destructive"
                    size="icon"
                    className="h-8 w-8 rounded-full"
                    onClick={(e) => {
                      e.stopPropagation();
                      removeFile(index);
                    }}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
                {index === 0 && (
                  <div className="absolute top-2 left-2 bg-white/90 backdrop-blur text-slate-900 text-[10px] font-bold px-2 py-1 rounded shadow-sm">
                    ẢNH BÌA
                  </div>
                )}
              </div>
            ))}
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
