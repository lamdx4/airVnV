import { useState } from 'react';
import { useParams, useSearchParams } from 'react-router-dom';
import { useProperty } from '@/features/properties/hooks/useProperties';
import { useReviews } from '@/features/reviews/hooks/useReviews';
import { BookingWidget } from '@/features/booking';
import { BookingMode } from '@/features/booking/types';
import { MapPin, Star, Medal, Eye } from 'lucide-react';
import { Skeleton } from '@/components/ui/skeleton';
import { ReviewList } from '@/features/reviews/components/ReviewList';
import { ImageLightbox } from '@/components/common/ImageLightbox';
import { Button } from '@/components/ui/button';
import { Icon } from '@iconify/react';
import { useTranslation } from 'react-i18next';

export default function PropertyDetail() {
  const { t } = useTranslation();
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();
  const isPreview = searchParams.get('isPreview') === 'true';
  const { data: property, isLoading, isError } = useProperty(id || '');
  
  // State for image lightbox viewer
  const [isLightboxOpen, setIsLightboxOpen] = useState(false);
  const [activeImageIndex, setActiveImageIndex] = useState(0);

  // Dynamic reviews query to get rating and review count instead of hardcoded placeholders
  const { data: reviewsData } = useReviews(id || '', 1, 50);

  if (isLoading) return <PropertyDetailSkeleton />;
  if (isError || !property) return <div className="p-12 text-center text-red-500 text-xl font-semibold">{t('propertyDetail.failedToLoad')}</div>;

  const reviewCount = reviewsData?.totalCount || 0;
  const reviewsList = reviewsData?.items || [];
  const averageRating = reviewCount > 0
    ? (reviewsList.reduce((sum, r) => sum + r.rating, 0) / reviewsList.length).toFixed(2)
    : 'New';

  const allImages = property.images || [];

  const openImage = (index: number) => {
    setActiveImageIndex(index);
    setIsLightboxOpen(true);
  };

  const renderGallery = () => {
    if (allImages.length === 0) {
      return (
        <div className="w-full h-[400px] mb-12 bg-slate-50 rounded-2xl flex flex-col items-center justify-center text-slate-400 gap-3 border border-dashed border-slate-300">
          <svg className="w-12 h-12 stroke-current text-slate-300" viewBox="0 0 24 24" fill="none" strokeWidth="1.5">
            <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 15.75l5.159-5.159a2.25 2.25 0 013.182 0l5.159 5.159m-1.5-1.5l1.409-1.409a2.25 2.25 0 013.182 0l2.909 2.909m-18 3.75h16.5a1.5 1.5 0 001.5-1.5V6a1.5 1.5 0 00-1.5-1.5H3.75A1.5 1.5 0 002.25 6v12a1.5 1.5 0 001.5 1.5zm10.5-11.25h.008v.008h-.008V8.25zm.375 0a.375 0 11-.75 0 .375 0 01.75 0z" />
          </svg>
          <span className="font-semibold text-sm">No photos uploaded for this property yet</span>
        </div>
      );
    }

    if (allImages.length === 1) {
      return (
        <div onClick={() => openImage(0)} className="w-full h-[500px] mb-12 rounded-2xl overflow-hidden cursor-pointer group">
          <img src={allImages[0].url} alt="Cover" className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
        </div>
      );
    }

    if (allImages.length === 2) {
      return (
        <div className="grid grid-cols-2 gap-2 h-[500px] mb-12 rounded-2xl overflow-hidden">
          {allImages.slice(0, 2).map((img, idx) => (
            <div key={img.id} onClick={() => openImage(idx)} className="relative group cursor-pointer overflow-hidden">
              <img src={img.url} alt={`Gallery ${idx}`} className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
            </div>
          ))}
        </div>
      );
    }

    if (allImages.length === 3) {
      return (
        <div className="grid grid-cols-3 gap-2 h-[500px] mb-12 rounded-2xl overflow-hidden">
          <div onClick={() => openImage(0)} className="col-span-2 relative group cursor-pointer overflow-hidden">
            <img src={allImages[0].url} alt="Cover" className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
          </div>
          <div className="grid grid-rows-2 gap-2">
            {allImages.slice(1, 3).map((img, idx) => (
              <div key={img.id} onClick={() => openImage(idx + 1)} className="relative group cursor-pointer overflow-hidden">
                <img src={img.url} alt={`Gallery ${idx}`} className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
              </div>
            ))}
          </div>
        </div>
      );
    }

    if (allImages.length === 4) {
      return (
        <div className="grid grid-cols-4 gap-2 h-[500px] mb-12 rounded-2xl overflow-hidden">
          <div onClick={() => openImage(0)} className="col-span-2 row-span-2 relative group cursor-pointer overflow-hidden">
            <img src={allImages[0].url} alt="Cover" className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
          </div>
          <div className="col-span-2 grid grid-cols-2 gap-2">
            {allImages.slice(1, 4).map((img, idx) => (
              <div key={img.id} onClick={() => openImage(idx + 1)} className="relative group cursor-pointer overflow-hidden">
                <img src={img.url} alt={`Gallery ${idx}`} className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
              </div>
            ))}
          </div>
        </div>
      );
    }

    const cover = allImages.find(i => i.type === 1) || allImages[0];
    const gallery = allImages.filter(i => i.id !== cover.id).slice(0, 4);
    return (
      <div className="grid grid-cols-4 grid-rows-2 gap-2 h-[500px] mb-12 rounded-2xl overflow-hidden">
        <div onClick={() => openImage(allImages.indexOf(cover))} className="col-span-2 row-span-2 relative group cursor-pointer overflow-hidden">
          <img src={cover.url} alt="Cover" className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
        </div>
        {gallery.map((img) => (
          <div key={img.id} onClick={() => openImage(allImages.indexOf(img))} className="relative group cursor-pointer overflow-hidden">
            <img src={img.url} alt={`Gallery Image`} className="w-full h-full object-cover group-hover:brightness-90 transition duration-300" />
          </div>
        ))}
      </div>
    );
  };


  return (
    <div className="max-w-7xl mx-auto px-6 py-8">
      {/* Preview Mode Alert Banner */}
      {isPreview && (
        <div className="mb-6 bg-slate-900 border border-slate-800 text-white px-6 py-4 rounded-2xl flex items-center justify-between shadow-lg shadow-black/10 animate-pulse">
          <div className="flex items-center space-x-3">
            <div className="p-2 bg-white/10 rounded-xl">
              <Eye className="w-5 h-5 text-rausch" />
            </div>
            <div>
              <p className="font-bold text-sm">{t('propertyDetail.previewBannerTitle')}</p>
              <p className="text-xs text-slate-300 mt-0.5">{t('propertyDetail.previewBannerDesc')}</p>
            </div>
          </div>
          <span className="text-[10px] uppercase font-bold tracking-widest bg-rausch px-3 py-1 rounded-full text-white">{t('propertyDetail.previewBannerBadge')}</span>
        </div>
      )}

      {/* Header */}
      <h1 className="text-3xl font-semibold text-gray-900 mb-2">{property.title}</h1>
      <div className="flex items-center text-sm text-gray-600 mb-6 space-x-4">
        <span className="flex items-center font-medium text-gray-900">
          <Star className="w-4 h-4 mr-1 fill-current text-yellow-500" /> {averageRating}
        </span>
        {!isPreview && reviewCount > 0 && <span className="underline cursor-pointer">{reviewCount} {t('propertyDetail.reviews')}</span>}
        {!isPreview && reviewCount === 0 && <span className="text-slate-400">{t('propertyDetail.noReviews')}</span>}
        {isPreview && <span className="text-slate-400">0 {t('propertyDetail.reviewsPreview')}</span>}
        {(property as any).isSuperhost && (
          <span className="flex items-center"><Medal className="w-4 h-4 mr-1" /> {t('propertyDetail.superhost')}</span>
        )}
        {property.bookingMode === BookingMode.InstantBook ? (
          <span className="flex items-center bg-blue-50 text-blue-700 px-2.5 py-0.5 rounded border border-blue-200 font-bold tracking-wide uppercase text-[10px] shadow-sm">
            ⚡ {t('propertyDetail.instantBook', 'Instant Book')}
          </span>
        ) : (
          <span className="flex items-center bg-gray-100 text-gray-700 px-2.5 py-0.5 rounded border border-gray-200 font-bold tracking-wide uppercase text-[10px] shadow-sm">
            ✉️ {t('propertyDetail.requestToBook', 'Request to Book')}
          </span>
        )}
        <span className="flex items-center underline cursor-pointer"><MapPin className="w-4 h-4 mr-1" /> {property.displayAddress}</span>
      </div>

      {/* Dynamic Image Gallery */}
      {renderGallery()}

      {/* Two Column Layout */}
      <div className="flex flex-col lg:flex-row gap-12">
        {/* Left Column - Details */}
        <div className="flex-1">
          <div className="flex justify-between items-center pb-6 border-b border-gray-200">
            <div>
              <h2 className="text-2xl font-semibold text-gray-900 mb-1">{t('propertyDetail.entireHome', { host: property.hostId.substring(0, 6) })}</h2>
              <div className="text-gray-600 flex space-x-2">
                <span>{property.capacity?.guestCount || 4} {t('propertyDetail.guests')}</span>
                <span>•</span>
                <span>{property.capacity?.bedroomCount || 2} {t('propertyDetail.bedrooms')}</span>
                <span>•</span>
                <span>{property.capacity?.bedCount || 2} {t('propertyDetail.beds')}</span>
                <span>•</span>
                <span>{property.capacity?.bathroomCount || 1} {t('propertyDetail.baths')}</span>
              </div>
            </div>
            <div className="w-14 h-14 bg-gray-200 rounded-full flex items-center justify-center text-gray-500 font-bold text-xl uppercase">
              {property.hostId.substring(0, 1)}
            </div>
          </div>

          <div className="py-8 border-b border-gray-200">
            {(property as any).isSuperhost && (
              <div className="flex items-start mb-6">
                <Medal className="w-6 h-6 mr-4 text-gray-700" />
                <div>
                  <h3 className="font-semibold text-gray-900 text-lg">{t('propertyDetail.superhost')}</h3>
                  <p className="text-gray-500">{t('propertyDetail.superhostDesc')}</p>
                </div>
              </div>
            )}
            <div className="flex items-start">
              <MapPin className="w-6 h-6 mr-4 text-gray-700" />
              <div>
                <h3 className="font-semibold text-gray-900 text-lg">{t('propertyDetail.greatLocation')}</h3>
                <p className="text-gray-500">{t('propertyDetail.greatLocationDesc')}</p>
              </div>
            </div>
          </div>

          <div className="py-8 border-b border-gray-200">
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">{t('propertyDetail.aboutSpace')}</h2>
            <div className="text-gray-700 leading-relaxed whitespace-pre-wrap">
              {property.description}
            </div>
          </div>

          <div className="py-8 border-b border-gray-200">
            <h2 className="text-2xl font-semibold text-gray-900 mb-6">{t('propertyDetail.offers')}</h2>
            <div className="grid grid-cols-2 gap-y-4 gap-x-8">
              {property.propertyAmenities?.slice(0, 10).map((amenity, i) => (
                <div key={amenity.amenityId || i} className="flex items-center text-gray-700 text-lg">
                  <Icon icon={amenity.iconCode || "hugeicons:wifi"} className="w-6 h-6 mr-4 text-gray-500" />
                  {amenity.name}
                </div>
              ))}
              {(!property.propertyAmenities || property.propertyAmenities.length === 0) && (
                <>
                  <div className="flex items-center text-gray-700 text-lg"><Icon icon="hugeicons:wifi" className="w-6 h-6 mr-4 text-gray-500" /> Fast Wifi</div>
                  <div className="flex items-center text-gray-700 text-lg"><Icon icon="hugeicons:car" className="w-6 h-6 mr-4 text-gray-500" /> Free parking on premises</div>
                  <div className="flex items-center text-gray-700 text-lg"><Icon icon="hugeicons:coffee-cup" className="w-6 h-6 mr-4 text-gray-500" /> Coffee maker</div>
                  <div className="flex items-center text-gray-700 text-lg"><Icon icon="hugeicons:tv-01" className="w-6 h-6 mr-4 text-gray-500" /> 55" HDTV</div>
                </>
              )}
            </div>
          </div>

          {/* Reviews Section - Hidden in Preview */}
          {!isPreview && <ReviewList propertyId={property.id} />}
        </div>

        {/* Right Column - Booking Widget Sidebar */}
        <div className="w-full lg:w-[380px] shrink-0">
          {isPreview ? (
            <div className="bg-slate-50 border border-slate-200 rounded-3xl p-6 shadow-sm sticky top-6">
              <div className="text-3xl font-extrabold text-slate-800 mb-2">
                {new Intl.NumberFormat('en-US', { style: 'currency', currency: property.pricing.currencyCode || 'VND' }).format(property.pricing.basePrice)} 
                <span className="text-sm text-slate-400 font-medium"> {t('bookingWidget.perNight')}</span>
              </div>
              <div className="border-t border-slate-200 my-4 pt-4 space-y-3">
                <div className="flex justify-between text-sm text-slate-500">
                  <span>{t('bookingWidget.cleaningFee')}</span>
                  <span>{new Intl.NumberFormat('en-US', { style: 'currency', currency: property.pricing.currencyCode || 'VND' }).format(property.pricing.cleaningFee)}</span>
                </div>
                <div className="flex justify-between text-sm text-slate-500">
                  <span>{t('bookingWidget.serviceFee')}</span>
                  <span>{new Intl.NumberFormat('en-US', { style: 'currency', currency: property.pricing.currencyCode || 'VND' }).format(property.pricing.serviceFee)}</span>
                </div>
              </div>
              <div className="bg-amber-50 border border-amber-200 text-amber-800 p-4 rounded-2xl text-xs leading-relaxed mb-6 font-medium">
                ⚠️ Guest bookings and interactive reservation calendar inputs are disabled in draft preview mode.
              </div>
              <Button disabled className="w-full bg-slate-200 text-slate-400 py-4 rounded-2xl font-bold cursor-not-allowed text-sm">
                {t('propertyDetail.reserveDisabled')}
              </Button>
            </div>
          ) : (
            <BookingWidget
              propertyId={property.id}
              basePrice={property.pricing.basePrice}
              cleaningFee={property.pricing.cleaningFee}
              serviceFee={property.pricing.serviceFee}
              weekendPremiumPercent={property.pricing.weekendPremiumPercent}
              currencyCode={property.pricing.currencyCode}
              bookingMode={property.bookingMode}
            />
          )}
        </div>
      </div>

      {/* Fully reusable fullscreen Image Lightbox */}
      <ImageLightbox
        isOpen={isLightboxOpen}
        images={allImages}
        initialIndex={activeImageIndex}
        onClose={() => setIsLightboxOpen(false)}
      />
    </div>
  );
}

function PropertyDetailSkeleton() {
  return (
    <div className="max-w-7xl mx-auto px-6 py-8 animate-pulse">
      <Skeleton className="w-2/3 h-10 mb-4" />
      <Skeleton className="w-1/3 h-6 mb-8" />
      <Skeleton className="w-full h-[500px] rounded-xl mb-12" />
      <div className="flex gap-12">
        <div className="flex-1 space-y-6">
          <Skeleton className="w-full h-32 rounded-lg" />
          <Skeleton className="w-full h-48 rounded-lg" />
          <Skeleton className="w-full h-64 rounded-lg" />
        </div>
        <Skeleton className="w-[380px] h-[500px] rounded-xl" />
      </div>
    </div>
  );
}
