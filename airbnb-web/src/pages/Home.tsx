import { useState, useEffect } from 'react'
import { Star, Heart, Map as MapIcon, List as ListIcon } from 'lucide-react'
import { Icon } from '@iconify/react'
import { useSearchProperties } from '../features/search/hooks/useSearchProperties'
import { MapView } from '../features/search/components/MapView'
import { Skeleton } from '@/components/ui/skeleton'

const categories = [
  { id: 'beach', label: 'Beachfront', icon: 'hugeicons:beach-02' },
  { id: 'mountain', label: 'Amazing pools', icon: 'hugeicons:swimming-pool' },
  { id: 'lake', label: 'Islands', icon: 'hugeicons:island-01' },
  { id: 'cabin', label: 'Cabins', icon: 'hugeicons:home-01' },
  { id: 'mansion', label: 'Mansions', icon: 'hugeicons:castle-01' },
  { id: 'farm', label: 'Farms', icon: 'hugeicons:farm-01' },
  { id: 'camping', label: 'Camping', icon: 'hugeicons:tent-01' },
  { id: 'arctic', label: 'Arctic', icon: 'hugeicons:snow-02' },
  { id: 'desert', label: 'Desert', icon: 'hugeicons:desert-01' },
  { id: 'trending', label: 'Trending', icon: 'hugeicons:zap' },
]

export default function Home() {
  const [activeCategory, setActiveCategory] = useState('beach')
  const [likedPlaces, setLikedPlaces] = useState<string[]>([])
  const [showMap, setShowMap] = useState(false)
  
  // Default coordinates (HCMC, Vietnam)
  const [location, setLocation] = useState({ latitude: 10.762622, longitude: 106.660172 })

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
      <div className="sticky top-[80px] z-30 bg-white/95 backdrop-blur-sm -mx-12 px-12 pt-4">
        <div className="flex items-center gap-8 overflow-x-auto pb-3 scrollbar-none border-b border-slate-100">
          {categories.map((cat) => (
            <button
              key={cat.id}
              onClick={() => setActiveCategory(cat.id)}
              className={`flex flex-col items-center gap-2 min-w-[70px] pb-3 border-b-2 transition-all duration-200 group relative ${
                activeCategory === cat.id
                  ? 'border-slate-900 text-slate-900'
                  : 'border-transparent text-slate-500 hover:text-slate-900 hover:border-slate-200'
              }`}
            >
              <Icon 
                icon={cat.icon} 
                className={`text-[24px] ${
                  activeCategory === cat.id ? 'opacity-100' : 'opacity-60'
                }`} 
              />
              <span className={`text-xs whitespace-nowrap font-semibold tracking-tight ${activeCategory === cat.id ? 'text-slate-900' : 'text-slate-500'}`}>
                {cat.label}
              </span>
            </button>
          ))}
        </div>
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
          Failed to load properties. Make sure backend is running.
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
                <p className="text-[15px] text-slate-500 font-normal leading-tight">Within 50 km</p>
                <div className="pt-1.5">
                  <span className="text-[15px] text-slate-900 font-semibold">
                    {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: place.currency }).format(place.price)}
                  </span>
                  <span className="text-[15px] text-slate-900 font-normal"> night</span>
                </div>
              </div>
            </div>
          ))}
          {searchData?.items.length === 0 && (
            <div className="col-span-full text-center py-20 text-slate-500">
              No properties found in this area.
            </div>
          )}
        </div>
      )}

      {/* Floating Map Toggle Button */}
      <div className="fixed bottom-10 left-1/2 -translate-x-1/2 z-40">
        <button
          onClick={() => setShowMap(!showMap)}
          className="bg-slate-900 text-white px-6 py-3 rounded-full font-semibold shadow-lg hover:scale-105 transition-transform flex items-center gap-2"
        >
          {showMap ? (
            <>
              Show list <ListIcon className="w-5 h-5" />
            </>
          ) : (
            <>
              Show map <MapIcon className="w-5 h-5" />
            </>
          )}
        </button>
      </div>
    </div>
  )
}
