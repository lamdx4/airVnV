import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

const resources = {
  en: {
    translation: {
      "header": {
        "yourHome": "Airbnb your home",
        "messages": "Messages",
        "trips": "Trips",
        "profile": "Profile",
        "manageListings": "Manage listings",
        "logout": "Log out",
        "login": "Log in",
        "signup": "Sign up"
      },
      "app": {
        "title": "List Your Property",
        "saveExit": "Save & Exit",
        "step": "Step {{step}} of 5"
      },
      "basic": {
        "title": "The Basics",
        "subtitle": "Share some basic info about your place",
        "propertyTitle": "Property Title",
        "propertyTitlePlaceholder": "e.g., Cozy Beachfront Villa with Private Pool",
        "propertyDescription": "Description",
        "propertyDescriptionPlaceholder": "Describe the unique features, ambiance, and surroundings of your property...",
        "capacity": "Capacities & Spaces",
        "guests": "Guests",
        "bedrooms": "Bedrooms",
        "beds": "Beds",
        "bathrooms": "Bathrooms",
        "continue": "Continue"
      },
      "pricing": {
        "title": "Pricing & Fees",
        "subtitle": "Set your prices and dynamic service charges",
        "basePrice": "Base Price (per night)",
        "cleaningFee": "Cleaning Fee",
        "continue": "Continue"
      },
      "location": {
        "title": "Location",
        "subtitle": "Where is your place located?",
        "country": "Country / Region",
        "search": "Search Address",
        "searchPlaceholder": "Search for your city, street, or building...",
        "searching": "Searching suggestions...",
        "useCurrent": "Use Current Location",
        "locating": "Locating you on the map...",
        "locateSuccess": "Found your location!",
        "autoMapSuccess": "Address auto-mapped to fields below!",
        "errorFetchCountries": "Failed to load supported countries from server. Please try refreshing.",
        "errorFetchConfig": "Failed to load address field structures for {{countryCode}} from server.",
        "province": "Province / City",
        "selectProvince": "Select Province/City",
        "ward": "Ward / Commune",
        "selectWard": "Select Ward/Commune",
        "continue": "Continue"
      },
      "amenities": {
        "title": "Amenities",
        "subtitle": "What amenities does your property offer?",
        "errorFetch": "Failed to load amenities catalog list from the server.",
        "continue": "Continue"
      },
      "rules": {
        "title": "House Rules & Booking Times",
        "subtitle": "Define booking check-in details and guest policies",
        "checkIn": "Check-in Time",
        "checkOut": "Check-out Time",
        "pets": "Allow Pets",
        "smoking": "Allow Smoking",
        "events": "Allow Social Events",
        "flexible": "Flexible Check-out",
        "publish": "Publish Listing",
        "publishing": "Publishing Listing...",
        "lockTitle": "Publishing is Temporarily Locked",
        "lockSub": "We cannot publish your property because some required backend services are offline:",
        "lockCountries": "Supported Countries Catalog: Failed to fetch the master list of active supported countries.",
        "lockAmenities": "Amenities Catalog: The server failed to load the available amenities list.",
        "lockConfig": "Address Dynamic Config ({{countryCode}}): Failed to load the structural form layouts for this country.",
        "lockIntegrity": "Layout Integrity: The address form structure returned from the database is invalid or empty.",
        "lockFooter": "Please check your connection, refresh the page, or select a different country."
      }
    }
  },
  vi: {
    translation: {
      "header": {
        "yourHome": "Đăng tin chỗ nghỉ",
        "messages": "Tin nhắn",
        "trips": "Chuyến đi",
        "profile": "Cá nhân",
        "manageListings": "Quản lý danh sách",
        "logout": "Đăng xuất",
        "login": "Đăng nhập",
        "signup": "Đăng ký"
      },
      "app": {
        "title": "Đăng tin nhà / phòng của bạn",
        "saveExit": "Lưu & Thoát",
        "step": "Bước {{step}} trên 5"
      },
      "basic": {
        "title": "Thông tin cơ bản",
        "subtitle": "Chia sẻ một vài thông tin cơ bản về chỗ nghỉ của bạn",
        "propertyTitle": "Tiêu đề chỗ nghỉ",
        "propertyTitlePlaceholder": "Ví dụ: Biệt thự ven biển ấm cúng với hồ bơi riêng",
        "propertyDescription": "Mô tả",
        "propertyDescriptionPlaceholder": "Mô tả những điểm độc đáo, không gian, và khu vực xung quanh chỗ nghỉ của bạn...",
        "capacity": "Sức chứa & Không gian",
        "guests": "Khách tối đa",
        "bedrooms": "Phòng ngủ",
        "beds": "Giường",
        "bathrooms": "Phòng tắm",
        "continue": "Tiếp tục"
      },
      "pricing": {
        "title": "Giá cả & Phí dịch vụ",
        "subtitle": "Thiết lập giá và các khoản phí dịch vụ đi kèm",
        "basePrice": "Giá cơ bản (mỗi đêm)",
        "cleaningFee": "Phí dọn dẹp",
        "continue": "Tiếp tục"
      },
      "location": {
        "title": "Vị trí",
        "subtitle": "Chỗ nghỉ của bạn nằm ở đâu?",
        "country": "Quốc gia / Khu vực",
        "search": "Tìm kiếm địa chỉ",
        "searchPlaceholder": "Tìm kiếm thành phố, đường phố hoặc tòa nhà của bạn...",
        "searching": "Đang tìm kiếm gợi ý...",
        "useCurrent": "Sử dụng vị trí hiện tại",
        "locating": "Đang xác định vị trí của bạn trên bản đồ...",
        "locateSuccess": "Đã tìm thấy vị trí của bạn!",
        "autoMapSuccess": "Đã tự động điền địa chỉ vào các trường bên dưới!",
        "errorFetchCountries": "Không thể tải danh sách quốc gia được hỗ trợ từ máy chủ. Vui lòng làm mới lại.",
        "errorFetchConfig": "Không thể tải cấu trúc biểu mẫu địa chỉ của {{countryCode}} từ máy chủ.",
        "province": "Tỉnh / Thành phố",
        "selectProvince": "Chọn Tỉnh/Thành phố",
        "ward": "Phường / Xã",
        "selectWard": "Chọn Phường/Xã",
        "continue": "Tiếp tục"
      },
      "amenities": {
        "title": "Tiện ích",
        "subtitle": "Chỗ nghỉ của bạn cung cấp những tiện ích gì?",
        "errorFetch": "Không thể tải danh mục tiện ích từ máy chủ.",
        "continue": "Tiếp tục"
      },
      "rules": {
        "title": "Quy tắc chung & Thời gian",
        "subtitle": "Định nghĩa thời gian nhận/trả phòng và quy định chỗ nghỉ",
        "checkIn": "Giờ nhận phòng (Check-in)",
        "checkOut": "Giờ trả phòng (Check-out)",
        "pets": "Cho phép thú cưng",
        "smoking": "Cho phép hút thuốc",
        "events": "Cho phép tổ chức sự kiện",
        "flexible": "Trả phòng linh hoạt",
        "publish": "Đăng tin chỗ nghỉ",
        "publishing": "Đang đăng tin...",
        "lockTitle": "Tính năng đăng tin tạm thời bị khóa",
        "lockSub": "Chúng tôi không thể đăng tin chỗ nghỉ vì một số dịch vụ máy chủ đang ngoại tuyến:",
        "lockCountries": "Danh mục quốc gia hỗ trợ: Không thể tải danh sách các quốc gia đang được hỗ trợ.",
        "lockAmenities": "Danh mục tiện ích: Máy chủ không thể tải danh sách các tiện ích sẵn có.",
        "lockConfig": "Cấu hình địa chỉ động ({{countryCode}}): Không thể tải cấu trúc biểu mẫu cho quốc gia này.",
        "lockIntegrity": "Tính toàn vẹn biểu mẫu: Cấu trúc địa chỉ nhận về từ cơ sở dữ liệu bị trống hoặc không hợp lệ.",
        "lockFooter": "Vui lòng kiểm tra kết nối mạng, tải lại trang hoặc chọn một quốc gia khác."
      }
    }
  }
};

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: 'en',
    interpolation: {
      escapeValue: false
    }
  });

export default i18n;
