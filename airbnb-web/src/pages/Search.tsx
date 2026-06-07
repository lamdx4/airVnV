import { useState, useEffect } from 'react'
import { Link, useSearchParams } from 'react-router'
import { Star, Heart } from 'lucide-react'
import { Icon } from '@iconify/react'
import { useSearchProperties } from '../features/search/hooks/useSearchProperties'
import { MapView } from '../features/search/components/MapView'
import { Skeleton } from '@/components/ui/skeleton'
import { useTranslation } from 'react-i18next'
import { Slider } from '@/components/ui/slider'
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel"

export default function Search() {
  const [searchParams, setSearchParams] = useSearchParams()
  const { t } = useTranslation()
  const [likedPlaces, setLikedPlaces] = useState<string[]>([])
  
  // Parse query params
  const latParam = searchParams.get('lat')
  const lonParam = searchParams.get('lon')
  const radParam = searchParams.get('radius')
  
  const [location, setLocation] = useState({ 
    latitude: latParam ? parseFloat(latParam) : 10.762622, 
    longitude: lonParam ? parseFloat(lonParam) : 106.660172 
  })
  
  // Convert radius to slider format [number]
  const initialRadius = radParam ? parseFloat(radParam) : 50
  const [radius, setRadius] = useState<number[]>([initialRadius])
  const [debouncedRadius, setDebouncedRadius] = useState(initialRadius)

  // Debounce slider to avoid spamming API
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedRadius(radius[0])
      
      // Update URL silently
      const newParams = new URLSearchParams(searchParams)
      newParams.set('radius', radius[0].toString())
      setSearchParams(newParams, { replace: true })
    }, 500)
    return () => clearTimeout(handler)
  }, [radius, searchParams, setSearchParams])

  // Get user location if no params provided
  useEffect(() => {
    if (!latParam || !lonParam) {
      if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition((position) => {
          setLocation({
            latitude: position.coords.latitude,
            longitude: position.coords.longitude
          });
          // Update URL
          const newParams = new URLSearchParams(searchParams)
          newParams.set('lat', position.coords.latitude.toString())
          newParams.set('lon', position.coords.longitude.toString())
          setSearchParams(newParams, { replace: true })
        }, () => {
          console.warn("Geolocation denied or failed. Using default location.");
        });
      }
    }
  }, [latParam, lonParam, searchParams, setSearchParams]);

  const { data: searchData, isLoading, isError } = useSearchProperties({
    latitude: location.latitude,
    longitude: location.longitude,
    radiusKm: debouncedRadius,
    page: 1,
    pageSize: 50 // fetch more for map
  })

  const toggleLike = (id: string) => {
    setLikedPlaces(prev =>
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    )
  }

  return (
    <div className="flex flex-col md:flex-row h-[calc(100vh-80px)] overflow-hidden">
      
      {/* Left side: Search Results */}
      <div className="w-full md:w-[60%] lg:w-[50%] h-full flex flex-col overflow-y-auto no-scrollbar p-6 border-r border-slate-200">
        
        {/* Header & Filter */}
        <div className="mb-6 space-y-4">
          <h1 className="text-2xl font-semibold text-slate-900">
            {searchData?.items.length || 0} {t('home.noProperties') === 'Không tìm thấy nhà/phòng nào.' ? 'chỗ ở tại khu vực bản đồ' : 'places in selected map area'}
          </h1>
          
          {/* Radius Filter */}
          <div className="bg-slate-50 p-4 rounded-xl border border-slate-100 flex items-center gap-6">
            <span className="text-sm font-medium text-slate-700 whitespace-nowrap">
              Radius: <span className="font-bold text-slate-900">{radius[0]} km</span>
            </span>
            <div className="flex-1 px-2">
              <Slider
                defaultValue={[initialRadius]}
                value={radius}
                max={200}
                min={1}
                step={1}
                onValueChange={setRadius}
              />
            </div>
          </div>
        </div>

        {/* Results Grid */}
        {isLoading ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-6 gap-y-10">
            {[...Array(6)].map((_, i) => (
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
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-6 gap-y-10">
            {searchData?.items.map((place) => (
              <Link key={place.id} to={`/properties/${place.id}`} className="group flex flex-col cursor-pointer transition-all">
                {/* Image Wrapper */}
                <div className="relative w-full aspect-[20/19] rounded-xl overflow-hidden bg-slate-100 mb-3 shadow-sm hover:shadow-md transition-shadow group/carousel">
                  {place.images && place.images.length > 0 ? (
                    <Carousel className="w-full h-full">
                      <CarouselContent className="h-full ml-0">
                        {place.images.map((img, idx) => (
                          <CarouselItem key={idx} className="pl-0 h-full w-full relative">
                            <img
                              src={img}
                              alt={`${place.title} - Image ${idx + 1}`}
                              className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500 ease-out"
                            />
                          </CarouselItem>
                        ))}
                      </CarouselContent>
                      {place.images.length > 1 && (
                        <div className="opacity-0 group-hover/carousel:opacity-100 transition-opacity duration-300">
                          <CarouselPrevious 
                            className="absolute left-2 top-1/2 -translate-y-1/2 h-8 w-8 bg-white/80 border-none hover:bg-white text-slate-900" 
                            onClick={(e) => { e.preventDefault(); e.stopPropagation(); }} 
                          />
                          <CarouselNext 
                            className="absolute right-2 top-1/2 -translate-y-1/2 h-8 w-8 bg-white/80 border-none hover:bg-white text-slate-900" 
                            onClick={(e) => { e.preventDefault(); e.stopPropagation(); }} 
                          />
                        </div>
                      )}
                    </Carousel>
                  ) : place.thumbnail ? (
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
                      e.preventDefault()
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
                      <span className="font-normal text-slate-900">{(place.rating ?? 0).toFixed(2)}</span>
                    </div>
                  </div>
                  <p className="text-[15px] text-slate-500 font-normal leading-tight truncate">{place.displayAddress || t('home.within50km')}</p>
                  <div className="pt-1.5">
                    <span className="text-[15px] text-slate-900 font-semibold">
                      {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: place.currency }).format(place.price)}
                    </span>
                    <span className="text-[15px] text-slate-900 font-normal"> / {t('home.night')}</span>
                  </div>
                </div>
              </Link>
            ))}
            {searchData?.items.length === 0 && (
              <div className="col-span-full text-center py-20 text-slate-500">
                {t('home.noProperties')}
              </div>
            )}
          </div>
        )}
      </div>

      {/* Right side: Map */}
      <div className="hidden md:block w-[40%] lg:w-[50%] h-full bg-slate-100">
        <MapView 
          latitude={location.latitude} 
          longitude={location.longitude} 
          markers={searchData?.items || []} 
        />
      </div>
      
    </div>
  )
}
