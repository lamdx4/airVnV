import { useParams, useSearchParams } from 'react-router-dom';
import { useProperty } from '@/features/properties/hooks/useProperties';
import { useReviews } from '@/features/reviews/hooks/useReviews';
import { BookingWidget } from '@/features/booking';
import { MapPin, Star, Medal, Wifi, Car, Coffee, Tv, Eye } from 'lucide-react';
import { Skeleton } from '@/components/ui/skeleton';
import { ReviewList } from '@/features/reviews/components/ReviewList';

export default function PropertyDetail() {
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();
  const isPreview = searchParams.get('isPreview') === 'true';
  const { data: property, isLoading, isError } = useProperty(id || '');
  
  // Dynamic reviews query to get rating and review count instead of hardcoded placeholders
  const { data: reviewsData } = useReviews(id || '', 1, 50);

  if (isLoading) return <PropertyDetailSkeleton />;
  if (isError || !property) return <div className="p-12 text-center text-red-500 text-xl font-semibold">Failed to load property details.</div>;

  const reviewCount = reviewsData?.totalCount || 0;
  const reviewsList = reviewsData?.items || [];
  const averageRating = reviewCount > 0
    ? (reviewsList.reduce((sum, r) => sum + r.rating, 0) / reviewsList.length).toFixed(2)
    : 'New';

  const coverImage = property.images?.find(i => i.type === 1)?.url || 'https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=1200';
  const galleryImages = property.images?.filter(i => i.type === 0).slice(0, 4) || [];
  // Pad with placeholders if less than 4 gallery images
  while (galleryImages.length < 4) {
    galleryImages.push({ id: Math.random().toString(), url: 'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=600', type: 0, displayOrder: 0 });
  }

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
              <p className="font-bold text-sm">Host Preview Mode</p>
              <p className="text-xs text-slate-300 mt-0.5">This listing is currently in draft preview. Non-essential UI components (reviews, booking widget) are hidden or simulated.</p>
            </div>
          </div>
          <span className="text-[10px] uppercase font-bold tracking-widest bg-rausch px-3 py-1 rounded-full text-white">Draft</span>
        </div>
      )}

      {/* Header */}
      <h1 className="text-3xl font-semibold text-gray-900 mb-2">{property.title}</h1>
      <div className="flex items-center text-sm text-gray-600 mb-6 space-x-4">
        <span className="flex items-center font-medium text-gray-900">
          <Star className="w-4 h-4 mr-1 fill-current text-yellow-500" /> {averageRating}
        </span>
        {!isPreview && reviewCount > 0 && <span className="underline cursor-pointer">{reviewCount} reviews</span>}
        {!isPreview && reviewCount === 0 && <span className="text-slate-400">No reviews yet</span>}
        {isPreview && <span className="text-slate-400">0 reviews (Preview)</span>}
        <span className="flex items-center"><Medal className="w-4 h-4 mr-1" /> Superhost</span>
        <span className="flex items-center underline cursor-pointer"><MapPin className="w-4 h-4 mr-1" /> {property.displayAddress}</span>
      </div>

      {/* Image Gallery */}
      <div className="grid grid-cols-4 grid-rows-2 gap-2 h-[500px] mb-12 rounded-xl overflow-hidden">
        <div className="col-span-2 row-span-2 relative group cursor-pointer">
          <img src={coverImage} alt="Cover" className="w-full h-full object-cover group-hover:brightness-90 transition" />
        </div>
        {galleryImages.map((img, idx) => (
          <div key={img.id} className="relative group cursor-pointer">
            <img src={img.url} alt={`Gallery ${idx}`} className="w-full h-full object-cover group-hover:brightness-90 transition" />
          </div>
        ))}
      </div>

      {/* Two Column Layout */}
      <div className="flex flex-col lg:flex-row gap-12">
        {/* Left Column - Details */}
        <div className="flex-1">
          <div className="flex justify-between items-center pb-6 border-b border-gray-200">
            <div>
              <h2 className="text-2xl font-semibold text-gray-900 mb-1">Entire home hosted by {property.hostId.substring(0, 6)}</h2>
              <div className="text-gray-600 flex space-x-2">
                <span>{property.capacity?.guestCount || 4} guests</span>
                <span>•</span>
                <span>{property.capacity?.bedroomCount || 2} bedrooms</span>
                <span>•</span>
                <span>{property.capacity?.bedCount || 2} beds</span>
                <span>•</span>
                <span>{property.capacity?.bathroomCount || 1} baths</span>
              </div>
            </div>
            <div className="w-14 h-14 bg-gray-200 rounded-full flex items-center justify-center text-gray-500 font-bold text-xl uppercase">
              {property.hostId.substring(0, 1)}
            </div>
          </div>

          <div className="py-8 border-b border-gray-200">
            <div className="flex items-start mb-6">
              <Medal className="w-6 h-6 mr-4 text-gray-700" />
              <div>
                <h3 className="font-semibold text-gray-900 text-lg">Superhost</h3>
                <p className="text-gray-500">Superhosts are experienced, highly rated hosts who are committed to providing great stays for guests.</p>
              </div>
            </div>
            <div className="flex items-start">
              <MapPin className="w-6 h-6 mr-4 text-gray-700" />
              <div>
                <h3 className="font-semibold text-gray-900 text-lg">Great location</h3>
                <p className="text-gray-500">100% of recent guests gave the location a 5-star rating.</p>
              </div>
            </div>
          </div>

          <div className="py-8 border-b border-gray-200">
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">About this space</h2>
            <div className="text-gray-700 leading-relaxed whitespace-pre-wrap">
              {property.description}
            </div>
          </div>

          <div className="py-8 border-b border-gray-200">
            <h2 className="text-2xl font-semibold text-gray-900 mb-6">What this place offers</h2>
            <div className="grid grid-cols-2 gap-y-4 gap-x-8">
              {property.propertyAmenities?.slice(0, 10).map((amenity, i) => (
                <div key={amenity.amenityId || i} className="flex items-center text-gray-700 text-lg">
                  {/* Real implementation would map iconCode to Lucide icon dynamically */}
                  <Wifi className="w-6 h-6 mr-4 text-gray-500" />
                  {amenity.name}
                </div>
              ))}
              {(!property.propertyAmenities || property.propertyAmenities.length === 0) && (
                <>
                  <div className="flex items-center text-gray-700 text-lg"><Wifi className="w-6 h-6 mr-4 text-gray-500" /> Fast Wifi</div>
                  <div className="flex items-center text-gray-700 text-lg"><Car className="w-6 h-6 mr-4 text-gray-500" /> Free parking on premises</div>
                  <div className="flex items-center text-gray-700 text-lg"><Coffee className="w-6 h-6 mr-4 text-gray-500" /> Coffee maker</div>
                  <div className="flex items-center text-gray-700 text-lg"><Tv className="w-6 h-6 mr-4 text-gray-500" /> 55" HDTV</div>
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
              <div className="text-3xl font-extrabold text-slate-800 mb-2">${property.pricing.basePrice} <span className="text-sm text-slate-400 font-medium">/ night</span></div>
              <div className="border-t border-slate-200 my-4 pt-4 space-y-3">
                <div className="flex justify-between text-sm text-slate-500">
                  <span>Cleaning fee</span>
                  <span>${property.pricing.cleaningFee}</span>
                </div>
                <div className="flex justify-between text-sm text-slate-500">
                  <span>Service fee</span>
                  <span>${property.pricing.serviceFee}</span>
                </div>
              </div>
              <div className="bg-amber-50 border border-amber-200 text-amber-800 p-4 rounded-2xl text-xs leading-relaxed mb-6 font-medium">
                ⚠️ Guest bookings and interactive reservation calendar inputs are disabled in draft preview mode.
              </div>
              <button disabled className="w-full bg-slate-200 text-slate-400 py-4 rounded-2xl font-bold cursor-not-allowed text-sm">
                Reserve Disabled
              </button>
            </div>
          ) : (
            <BookingWidget
              propertyId={property.id}
              basePrice={property.pricing.basePrice}
              cleaningFee={property.pricing.cleaningFee}
              serviceFee={property.pricing.serviceFee}
              weekendPremiumPercent={property.pricing.weekendPremiumPercent}
              currencyCode={property.pricing.currencyCode}
            />
          )}
        </div>
      </div>
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
