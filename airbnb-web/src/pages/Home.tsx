import { useState, useEffect } from 'react'
import { Star, Heart, Map as MapIcon, List as ListIcon } from 'lucide-react'
import { Icon } from '@iconify/react'
import { useSearchProperties } from '../features/search/hooks/useSearchProperties'
import { MapView } from '../features/search/components/MapView'
import { Skeleton } from '@/components/ui/skeleton'
import { Button } from '@/components/ui/button'
import { useTranslation } from 'react-i18next'

export default function Home() {
  const [likedPlaces, setLikedPlaces] = useState<string[]>([])
  const [showMap, setShowMap] = useState(false)
  const { t } = useTranslation()
  
  // Default coordinates (HCMC, Vietnam)
  const [location, setLocation] = useState({ latitude: 10.762622, longitude: 106.660172 })
  const [propertyType, setPropertyType] = useState<number | null>(null)

  useEffect(() => {
    if ("geolocation" in navigator) {
      navigator.geolocation.getCurrentPosition((position) => {
        setLocation({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude
        });
      }, () => {
        console.warn("Geolocation denied or failed. Using default location.");
      });
    }
  }, []);

  const { data: searchData, isLoading, isError } = useSearchProperties({
    latitude: location.latitude,
    longitude: location.longitude,
    radiusKm: 50,
    propertyType: propertyType !== null ? propertyType : undefined,
    page: 1,
    pageSize: 20
  })

  const toggleLike = (id: string) => {
    setLikedPlaces(prev =>
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    )
  }

  return (
    <div className="space-y-6 pb-24 relative">
      {/* Category Bar */}
      <div className="flex items-center gap-8 overflow-x-auto no-scrollbar py-4 px-2 border-b border-slate-100">
        {[
          { id: null, key: 'all', icon: 'hugeicons:earth' },
          { id: 1, key: 'apartment', icon: 'hugeicons:building-04' },
          { id: 2, key: 'house', icon: 'hugeicons:home-03' },
          { id: 3, key: 'villa', icon: 'hugeicons:castle-02' },
          { id: 4, key: 'homestay', icon: 'hugeicons:house-02' },
          { id: 5, key: 'hotel', icon: 'hugeicons:hotel-01' },
          { id: 6, key: 'resort', icon: 'hugeicons:swimming-pool' },
        ].map((category) => (
          <button
            key={category.id || 'all'}
            onClick={() => setPropertyType(category.id)}
            className={`flex flex-col items-center gap-2 min-w-max pb-3 border-b-2 transition-all ${
              propertyType === category.id
                ? 'border-slate-900 text-slate-900'
                : 'border-transparent text-slate-500 hover:text-slate-900 hover:border-slate-300'
            }`}
          >
            <Icon icon={category.icon} className="text-2xl" />
            <span className="text-sm font-semibold">{t(`home.categories.${category.key}`)}</span>
          </button>
        ))}
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-x-6 gap-y-10 pt-2">
          {[...Array(12)].map((_, i) => (
            <div key={i} className="flex flex-col gap-3">
              <Skeleton className="w-full aspect-[20/19] rounded-xl" />
              <Skeleton className="w-3/4 h-4" />
              <Skeleton className="w-1/2 h-4" />
            </div>
          ))}
        </div>
      ) : isError ? (
        <div className="text-center py-20 text-red-500 font-medium">
          {t('home.failedToLoad')}
        </div>
      ) : showMap ? (
        <div className="h-[calc(100vh-200px)] rounded-xl overflow-hidden mt-4">
          <MapView 
            latitude={location.latitude} 
            longitude={location.longitude} 
            markers={searchData?.items || []} 
          />
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-x-6 gap-y-10 pt-2">
          {searchData?.items.map((place) => (
            <div key={place.id} className="group flex flex-col cursor-pointer transition-all">
              {/* Image Wrapper */}
              <div className="relative w-full aspect-[20/19] rounded-xl overflow-hidden bg-slate-100 mb-3 shadow-sm hover:shadow-md transition-shadow">
                {place.thumbnail ? (
                  <img
                    src={place.thumbnail}
                    alt={place.title}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500 ease-out"
                  />
                ) : (
                  <div className="w-full h-full flex items-center justify-center text-slate-300 bg-slate-200">
                     <Icon icon="hugeicons:image-01" className="text-4xl" />
                  </div>
                )}
                
                {/* Heart Icon */}
                <button
                  onClick={(e) => {
                    e.stopPropagation()
                    toggleLike(place.id)
                  }}
                  className="absolute top-3 right-3 p-2 transition-transform active:scale-90 z-10"
                >
                  <Heart
                    className={`h-6 w-6 transition-all drop-shadow-md ${
                      likedPlaces.includes(place.id)
                        ? 'fill-[#FF5A5F] text-[#FF5A5F]'
                        : 'text-white/90 stroke-[2.5]'
                    }`}
                  />
                </button>
              </div>

              {/* Details */}
              <div className="space-y-0.5">
                <div className="flex justify-between items-start gap-2">
                  <h3 className="font-semibold text-slate-900 text-[15px] truncate">{place.title}</h3>
                  <div className="flex items-center gap-1 text-[14px] shrink-0">
                    <Star className="h-3 w-3 fill-slate-900 text-slate-900" />
                    <span className="font-normal text-slate-900">{place.rating.toFixed(2)}</span>
                  </div>
                </div>
                <p className="text-[15px] text-slate-500 font-normal leading-tight truncate">{(place as any).displayAddress || t('home.within50km')}</p>
                <div className="pt-1.5">
                  <span className="text-[15px] text-slate-900 font-semibold">
                    {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: place.currency }).format(place.price)}
                  </span>
                  <span className="text-[15px] text-slate-900 font-normal"> / {t('home.night')}</span>
                </div>
              </div>
            </div>
          ))}
          {searchData?.items.length === 0 && (
            <div className="col-span-full text-center py-20 text-slate-500">
              {t('home.noProperties')}
            </div>
          )}
        </div>
      )}

      {/* Floating Map Toggle Button */}
      <div className="fixed bottom-10 left-1/2 -translate-x-1/2 z-40">
        <Button
          onClick={() => setShowMap(!showMap)}
          className="bg-slate-950 hover:bg-slate-800 text-white px-6 py-6 rounded-full font-semibold shadow-lg hover:scale-105 transition-all flex items-center gap-2 h-auto"
        >
          {showMap ? (
            <>
              {t('home.showList')} <ListIcon className="w-5 h-5" />
            </>
          ) : (
            <>
              {t('home.showMap')} <MapIcon className="w-5 h-5" />
            </>
          )}
        </Button>
      </div>
    </div>
  )
}
