import { useState } from 'react'
import { Star, Heart } from 'lucide-react'

// Lấy ảnh từ thư mục locations
const locationImages = Object.values(
  import.meta.glob<{ default: string }>('../assets/locations/*.{png,jpg,jpeg,webp}', { eager: true })
).map(mod => mod.default);

const categories = [
  { id: 'beach', label: 'Bãi biển', icon: '🏖️' },
  { id: 'mountain', label: 'Núi rừng', icon: '⛰️' },
  { id: 'lake', label: 'Hồ nước', icon: '🛶' },
  { id: 'cabin', label: 'Nhà gỗ', icon: '🛖' },
  { id: 'mansion', label: 'Biệt thự', icon: '🏰' },
  { id: 'pool', label: 'Hồ bơi tuyệt đẹp', icon: '🏊' },
]

export default function Home() {
  const [activeCategory, setActiveCategory] = useState('beach')
  const [likedPlaces, setLikedPlaces] = useState<number[]>([])

  const places = [
    { id: 1, title: 'Đà Nẵng, Việt Nam', desc: 'Gần Cầu Vàng & Bà Nà Hills', dates: '12 - 17 thg 5', price: '2.500.000 ₫', rating: '4.92', image: locationImages[0] },
    { id: 2, title: 'Hội An, Việt Nam', desc: 'Phố cổ & Sông Hoài thơ mộng', dates: '15 - 20 thg 5', price: '1.800.000 ₫', rating: '4.88', image: locationImages[1] },
    { id: 3, title: 'Phú Quốc, Việt Nam', desc: 'Bãi Sao - Hoàng hôn tuyệt đẹp', dates: '10 - 15 thg 6', price: '3.200.000 ₫', rating: '4.95', image: locationImages[2] },
    { id: 4, title: 'Hạ Long, Việt Nam', desc: 'Kỳ quan thiên nhiên thế giới', dates: '05 - 10 thg 7', price: '4.000.000 ₫', rating: '4.91', image: locationImages[3] },
    { id: 5, title: 'Vũng Tàu, Việt Nam', desc: 'View biển cực chill, ngắm sóng vỗ', dates: '22 - 27 thg 5', price: '1.500.000 ₫', rating: '4.75', image: locationImages[4] },
    // Nhân bản thêm để cho layout đẹp
    { id: 6, title: 'Đà Nẵng Riverside', desc: 'View sông Hàn xem bắn pháo hoa', dates: '01 - 06 thg 6', price: '2.900.000 ₫', rating: '4.85', image: locationImages[0] },
    { id: 7, title: 'Phú Quốc Resort', desc: 'Biệt thự biển riêng tư', dates: '18 - 23 thg 6', price: '5.500.000 ₫', rating: '4.98', image: locationImages[2] },
    { id: 8, title: 'Hạ Long Luxury Cruise', desc: 'Du thuyền ngủ đêm sang trọng', dates: '14 - 19 thg 5', price: '6.200.000 ₫', rating: '4.90', image: locationImages[3] },
  ]

  const toggleLike = (id: number) => {
    setLikedPlaces(prev =>
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    )
  }

  return (
    <div className="space-y-6 font-sans">
      {/* Category Bar */}
      <div className="flex items-center gap-8 overflow-x-auto pb-4 scrollbar-none border-b border-slate-100">
        {categories.map((cat) => (
          <button
            key={cat.id}
            onClick={() => setActiveCategory(cat.id)}
            className={`flex flex-col items-center gap-2 min-w-[60px] pb-2 border-b-2 transition-all duration-200 cursor-pointer ${
              activeCategory === cat.id
                ? 'border-slate-900 text-slate-900 font-semibold'
                : 'border-transparent text-slate-500 hover:text-slate-800 hover:border-slate-200'
            }`}
          >
            <span className="text-2xl">{cat.icon}</span>
            <span className="text-xs whitespace-nowrap">{cat.label}</span>
          </button>
        ))}
      </div>

      {/* Listing Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-x-6 gap-y-10">
        {places.map((place) => (
          <div key={place.id} className="group flex flex-col cursor-pointer">
            {/* Image Wrapper */}
            <div className="relative w-full aspect-square rounded-2xl overflow-hidden bg-slate-100 mb-3">
              {place.image ? (
                <img
                  src={place.image}
                  alt={place.title}
                  className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                />
              ) : (
                <div className="w-full h-full flex items-center justify-center text-slate-300">
                  No Image
                </div>
              )}
              
              {/* Heart Icon */}
              <button
                onClick={(e) => {
                  e.stopPropagation()
                  toggleLike(place.id)
                }}
                className="absolute top-3 right-3 p-2 rounded-full bg-transparent hover:scale-110 transition duration-200 z-10"
              >
                <Heart
                  className={`h-6 w-6 transition-colors duration-200 ${
                    likedPlaces.includes(place.id)
                      ? 'fill-rausch text-rausch'
                      : 'text-white drop-shadow-[0_2px_5px_rgba(0,0,0,0.5)]'
                  }`}
                />
              </button>
            </div>

            {/* Listing Details */}
            <div className="flex justify-between items-start">
              <h3 className="font-semibold text-slate-900 text-base">{place.title}</h3>
              <div className="flex items-center gap-1 text-sm">
                <Star className="h-4 w-4 fill-slate-900 text-slate-900" />
                <span className="font-medium text-slate-800">{place.rating}</span>
              </div>
            </div>
            <p className="text-sm text-slate-500 mt-0.5 font-light">{place.desc}</p>
            <p className="text-sm text-slate-500 mt-0.5 font-light">{place.dates}</p>
            <p className="text-sm text-slate-900 font-semibold mt-1.5">
              {place.price} <span className="font-normal text-slate-600">/ đêm</span>
            </p>
          </div>
        ))}
      </div>
    </div>
  )
}
