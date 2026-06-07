import { Link } from 'react-router';
import { useState } from 'react'
import { Star, Heart } from 'lucide-react'
import { Icon } from '@iconify/react'
import { usePublicProperties } from '../features/properties/hooks/usePublicProperties'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { useTranslation } from 'react-i18next'
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination"
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel"

export default function Home() {
  const [likedPlaces, setLikedPlaces] = useState<string[]>([])
  const { t } = useTranslation()
  const [propertyType, setPropertyType] = useState<number | null>(null)
  const [page, setPage] = useState(1)

  const handlePropertyTypeChange = (id: number | null) => {
    setPropertyType(id)
    setPage(1)
  }

  const { data: searchData, isLoading, isError } = usePublicProperties(
    page,
    20,
    propertyType !== null ? propertyType : undefined
  )

  const toggleLike = (id: string) => {
    setLikedPlaces(prev =>
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    )
  }

  return (
    <div className="space-y-6 pb-24 relative">
      {/* Category Bar */}
      <div className="flex items-center justify-start md:justify-center gap-8 overflow-x-auto no-scrollbar py-4 px-2 border-b border-slate-100">
        {[
          { id: null, key: 'all', icon: 'hugeicons:earth' },
          { id: 1, key: 'apartment', icon: 'hugeicons:building-04' },
          { id: 2, key: 'house', icon: 'hugeicons:home-03' },
          { id: 3, key: 'villa', icon: 'hugeicons:castle-02' },
          { id: 4, key: 'homestay', icon: 'hugeicons:house-02' },
          { id: 5, key: 'hotel', icon: 'hugeicons:hotel-01' },
          { id: 6, key: 'resort', icon: 'hugeicons:swimming-pool' },
        ].map((category) => (
          <Button
            variant="ghost"
            key={category.id || 'all'}
            onClick={() => handlePropertyTypeChange(category.id)}
            className={`flex flex-col items-center justify-center gap-2 h-auto px-4 py-3 rounded-none border-b-2 transition-all hover:bg-transparent bg-transparent ${
              propertyType === category.id
                ? 'border-slate-900 text-slate-900'
                : 'border-transparent text-slate-500 hover:text-slate-900 hover:border-slate-300'
            }`}
          >
            <Icon icon={category.icon} className="text-2xl" />
            <span className="text-sm font-semibold">{t(`home.categories.${category.key}`)}</span>
          </Button>
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
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-x-6 gap-y-10 pt-2">
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
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={(e) => {
                    e.preventDefault()
                    e.stopPropagation()
                    toggleLike(place.id)
                  }}
                  className="absolute top-2 right-2 h-10 w-10 rounded-full bg-transparent hover:bg-white/20 transition-transform active:scale-90 z-10"
                >
                  <Heart
                    className={`h-6 w-6 transition-all drop-shadow-md ${
                      likedPlaces.includes(place.id)
                        ? 'fill-[#FF5A5F] text-[#FF5A5F]'
                        : 'text-white/90 stroke-[2.5]'
                    }`}
                  />
                </Button>
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

      {/* Pagination Controls */}
      {!isLoading && !isError && searchData && searchData.totalCount > 20 && (
        <div className="py-12 mt-8 border-t border-slate-100">
          <Pagination>
            <PaginationContent>
              <PaginationItem>
                <PaginationPrevious 
                  onClick={() => {
                    if (page > 1) {
                      setPage(p => p - 1)
                      window.scrollTo({ top: 0, behavior: 'smooth' })
                    }
                  }}
                  className={page === 1 ? "pointer-events-none opacity-50" : "cursor-pointer"}
                />
              </PaginationItem>
              
              {Array.from({ length: Math.ceil(searchData.totalCount / 20) }).map((_, i) => {
                const pageNum = i + 1;
                // Only show current page, first, last, and pages around current
                if (pageNum === 1 || pageNum === Math.ceil(searchData.totalCount / 20) || Math.abs(pageNum - page) <= 1) {
                  return (
                    <PaginationItem key={pageNum}>
                      <PaginationLink 
                        isActive={pageNum === page}
                        onClick={() => {
                          setPage(pageNum)
                          window.scrollTo({ top: 0, behavior: 'smooth' })
                        }}
                        className="cursor-pointer"
                      >
                        {pageNum}
                      </PaginationLink>
                    </PaginationItem>
                  )
                }
                
                // Show ellipsis if there's a gap
                if (pageNum === 2 && page > 3) {
                  return <PaginationItem key="ellipsis-start"><PaginationEllipsis /></PaginationItem>
                }
                if (pageNum === Math.ceil(searchData.totalCount / 20) - 1 && page < Math.ceil(searchData.totalCount / 20) - 2) {
                  return <PaginationItem key="ellipsis-end"><PaginationEllipsis /></PaginationItem>
                }
                
                return null;
              })}

              <PaginationItem>
                <PaginationNext 
                  onClick={() => {
                    if (page < Math.ceil(searchData.totalCount / 20)) {
                      setPage(p => p + 1)
                      window.scrollTo({ top: 0, behavior: 'smooth' })
                    }
                  }}
                  className={page >= Math.ceil(searchData.totalCount / 20) ? "pointer-events-none opacity-50" : "cursor-pointer"}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </div>
      )}
    </div>
  )
}
