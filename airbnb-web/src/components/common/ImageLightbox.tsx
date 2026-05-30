import React, { useEffect, useState, useCallback } from 'react';
import { X, ChevronLeft, ChevronRight } from 'lucide-react';

interface ImageLightboxProps {
  isOpen: boolean;
  images: { id: string; url: string }[];
  initialIndex?: number;
  onClose: () => void;
}

export const ImageLightbox: React.FC<ImageLightboxProps> = ({
  isOpen,
  images,
  initialIndex = 0,
  onClose,
}) => {
  const [currentIndex, setCurrentIndex] = useState(initialIndex);

  useEffect(() => {
    if (isOpen) {
      setCurrentIndex(initialIndex);
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'unset';
    }
    return () => {
      document.body.style.overflow = 'unset';
    };
  }, [isOpen, initialIndex]);

  const handlePrev = useCallback(() => {
    setCurrentIndex((prev) => (prev === 0 ? images.length - 1 : prev - 1));
  }, [images.length]);

  const handleNext = useCallback(() => {
    setCurrentIndex((prev) => (prev === images.length - 1 ? 0 : prev + 1));
  }, [images.length]);

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (!isOpen) return;
      if (e.key === 'Escape') onClose();
      if (e.key === 'ArrowLeft') handlePrev();
      if (e.key === 'ArrowRight') handleNext();
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [isOpen, onClose, handlePrev, handleNext]);

  if (!isOpen || images.length === 0) return null;

  return (
    <div className="fixed inset-0 z-50 flex flex-col justify-between bg-black/95 backdrop-blur-xl p-4 md:p-8 animate-in fade-in duration-300">
      
      {/* Top Header Controls */}
      <div className="flex items-center justify-between text-white w-full max-w-7xl mx-auto h-16 shrink-0">
        <span className="text-sm font-semibold tracking-wider bg-white/10 px-4 py-2 rounded-full backdrop-blur-md">
          {currentIndex + 1} / {images.length}
        </span>
        <button
          onClick={onClose}
          className="p-3 bg-white/10 hover:bg-white/20 active:scale-95 rounded-full text-white transition backdrop-blur-md border border-white/5"
          aria-label="Close Lightbox"
        >
          <X className="w-6 h-6" />
        </button>
      </div>

      {/* Main Image Slider Wrapper */}
      <div className="flex-1 flex items-center justify-center relative max-w-7xl mx-auto w-full group py-4">
        
        {/* Left Arrow */}
        <button
          onClick={handlePrev}
          className="absolute left-2 md:left-4 z-10 p-4 bg-white/10 hover:bg-white/20 active:scale-95 text-white rounded-full transition backdrop-blur-md border border-white/5 opacity-0 group-hover:opacity-100 focus:opacity-100"
          aria-label="Previous Image"
        >
          <ChevronLeft className="w-6 h-6" />
        </button>

        {/* Display Active Image */}
        <div className="relative max-w-full max-h-[70vh] flex items-center justify-center select-none overflow-hidden rounded-2xl shadow-2xl border border-white/5 bg-slate-900/40">
          <img
            src={images[currentIndex].url}
            alt={`Viewer Image ${currentIndex + 1}`}
            className="object-contain max-w-full max-h-[70vh] w-auto h-auto transition-transform duration-500 ease-out animate-in zoom-in-95"
          />
        </div>

        {/* Right Arrow */}
        <button
          onClick={handleNext}
          className="absolute right-2 md:right-4 z-10 p-4 bg-white/10 hover:bg-white/20 active:scale-95 text-white rounded-full transition backdrop-blur-md border border-white/5 opacity-0 group-hover:opacity-100 focus:opacity-100"
          aria-label="Next Image"
        >
          <ChevronRight className="w-6 h-6" />
        </button>
      </div>

      {/* Bottom Thumbnail Strip */}
      <div className="w-full overflow-x-auto shrink-0 pb-4 pt-6 scrollbar-hide border-t border-white/5">
        <div className="flex items-center justify-center gap-3 w-max mx-auto px-4">
          {images.map((img, idx) => (
            <button
              key={img.id}
              onClick={() => setCurrentIndex(idx)}
              className={`h-16 w-24 rounded-xl overflow-hidden border-2 transition shrink-0 duration-300 relative ${
                idx === currentIndex
                  ? 'border-rausch scale-105 shadow-lg shadow-rausch/30 ring-2 ring-rausch/20'
                  : 'border-transparent opacity-40 hover:opacity-100 hover:scale-102'
              }`}
            >
              <img
                src={img.url}
                alt={`Thumbnail ${idx + 1}`}
                className="w-full h-full object-cover"
              />
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};
