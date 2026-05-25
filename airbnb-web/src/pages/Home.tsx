import { useState } from 'react'
import { Star, Heart } from 'lucide-react'
import { Icon } from '@iconify/react'
import { useNavigate } from 'react-router-dom'

// Lấy ảnh từ thư mục locations
const locationImages = Object.values(
  import.meta.glob<{ default: string }>('../assets/locations/*.{png,jpg,jpeg,webp}', { eager: true })
).map(mod => mod.default);

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
  const navigate = useNavigate()
  const [activeCategory, setActiveCategory] = useState('beach')
  const [likedPlaces, setLikedPlaces] = useState<string[]>([])

  const places = [
    { id: '11111111-1111-7111-8111-111111111111', title: 'Biệt thự nghỉ dưỡng Đà Lạt VIP', desc: 'Đà Lạt, Lâm Đồng, Việt Nam', dates: 'May 22 - 27', price: '150.00 $', rating: '4.98', image: locationImages[0] },
    { id: '2', title: 'Đà Nẵng, Việt Nam', desc: '150 km away', dates: 'May 12 - 17', price: '2,500,000 ₫', rating: '4.92', image: locationImages[1] },
    { id: '3', title: 'Hội An, Việt Nam', desc: '180 km away', dates: 'May 15 - 20', price: '1,800,000 ₫', rating: '4.88', image: locationImages[2] },
    { id: '4', title: 'Phú Quốc, Việt Nam', desc: '850 km away', dates: 'Jun 10 - 15', price: '3,200,000 ₫', rating: '4.95', image: locationImages[3] },
    { id: '5', title: 'Hạ Long, Việt Nam', desc: '1,200 km away', dates: 'Jul 5 - 10', price: '4,000,000 ₫', rating: '4.91', image: locationImages[4] },
  ]

  const toggleLike = (id: string) => {
    setLikedPlaces(prev =>
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    )
  }

  return (
    <div className="space-y-6">
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

      {/* Listing Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 gap-x-6 gap-y-10 pt-2">
        {places.map((place) => (
          <div 
            key={place.id} 
            onClick={() => navigate(`/properties/${place.id}`)}
            className="group flex flex-col cursor-pointer transition-all"
          >
            {/* Image Wrapper */}
            <div className="relative w-full aspect-[20/19] rounded-xl overflow-hidden bg-slate-100 mb-3 shadow-sm hover:shadow-md transition-shadow">
              {place.image ? (
                <img
                  src={place.image}
                  alt={place.title}
                  className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500 ease-out"
                />
              ) : (
                <div className="w-full h-full flex items-center justify-center text-slate-300">
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
                  <span className="font-normal text-slate-900">{place.rating}</span>
                </div>
              </div>
              <p className="text-[15px] text-slate-500 font-normal leading-tight">{place.desc}</p>
              <p className="text-[15px] text-slate-500 font-normal leading-tight">{place.dates}</p>
              <div className="pt-1.5">
                <span className="text-[15px] text-slate-900 font-semibold">{place.price}</span>
                <span className="text-[15px] text-slate-900 font-normal"> night</span>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
