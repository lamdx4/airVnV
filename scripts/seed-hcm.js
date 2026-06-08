const fs = require('fs');
const path = require('path');
const axios = require('axios');
const FormData = require('form-data');
const { fakerVI: faker } = require('@faker-js/faker');

// ==========================================
// CONFIGURATION
// ==========================================
const API_BASE_URL = 'http://localhost:5136'; // YARP Gateway
const NUM_HOSTS = 5;
const NUM_PROPERTIES_PER_HOST = 20;
const CONCURRENCY_LIMIT = 3; // Limit parallel property creations to avoid overwhelming the Gateway/Cloudinary

// ==========================================
// PREDEFINED LOCATIONS FOR REALISTIC SEARCH INDEXING
// ==========================================
const LOCATIONS = [
  {
    CountryCode: "VN",
    Admin1Code: "Hồ Chí Minh",
    DisplayAddress: "Hồ Chí Minh, Việt Nam",
    Districts: ["Quận 1", "Quận 2", "Quận 3", "Bình Thạnh", "Phú Nhuận", "Quận 4", "Quận 7", "Thành phố Thủ Đức"],
    Streets: ["Nguyễn Huệ", "Lê Lợi", "Thảo Điền", "Nguyễn Hữu Cảnh", "Nam Kỳ Khởi Nghĩa", "Võ Văn Kiệt", "Mai Chí Thọ", "Tôn Đức Thắng"],
    LatMin: 10.7000, LatMax: 10.8500,
    LngMin: 106.6000, LngMax: 106.8000
  }
];

const PROPERTY_TYPES = [1, 2, 3, 4, 5]; // House, Apartment, Villa, Cabin, Resort

// ==========================================
// HELPERS
// ==========================================
function getRandomLocation() {
    return faker.helpers.arrayElement(LOCATIONS);
}

function getRandomInRange(min, max, decimals = 4) {
    const str = (Math.random() * (max - min) + min).toFixed(decimals);
    return parseFloat(str);
}

// ==========================================
// MAIN SCRIPT
// ==========================================
async function runSeeder() {
    const { default: pLimit } = await import('p-limit');
    const limit = pLimit(CONCURRENCY_LIMIT);
    
    console.log(`🚀 Starting Bulk Seeder...`);
    console.log(`Config: ${NUM_HOSTS} Hosts, ${NUM_PROPERTIES_PER_HOST} Properties/Host`);

    const hosts = [];

    // 1. Create Hosts
    console.log(`\n--- 1. Creating ${NUM_HOSTS} Hosts ---`);
    for (let i = 1; i <= NUM_HOSTS; i++) {
        const email = `host${i}_${Date.now()}@airbnb.local`;
        const password = 'Password123!';
        const fullName = faker.person.fullName();
        
        try {
            console.log(`[Host ${i}] Registering ${email}...`);
            const regRes = await axios.post(`${API_BASE_URL}/api/users/register`, { email, password, fullName });
            
            const code = regRes.data.data.message.split('Code: ')[1].trim();
            await axios.post(`${API_BASE_URL}/api/users/verify-email`, { email, code });
            
            const loginRes = await axios.post(`${API_BASE_URL}/api/users/login`, { email, password });
            hosts.push({ email, password, token: loginRes.data.data.accessToken });
            console.log(`✅ Host ${i} verified & logged in.`);
        } catch (err) {
            console.error(`❌ Failed to create Host ${i}:`, err.response?.data || err.message);
        }
    }

    // 2. Create Properties
    console.log(`\n--- 2. Creating Properties (via p-limit concurrency: ${CONCURRENCY_LIMIT}) ---`);
    const propertyTasks = [];

    for (let h = 0; h < hosts.length; h++) {
        const host = hosts[h];
        
        for (let p = 1; p <= NUM_PROPERTIES_PER_HOST; p++) {
            propertyTasks.push(limit(async () => {
                const loc = getRandomLocation();
                const guestCount = faker.number.int({ min: 2, max: 12 });
                const bedroomCount = Math.ceil(guestCount / 2);
                const bedCount = bedroomCount + faker.number.int({ min: 0, max: 2 });
                const bathroomCount = faker.number.int({ min: 1, max: bedroomCount });
                const district = faker.helpers.arrayElement(loc.Districts);
                const street = faker.helpers.arrayElement(loc.Streets);
                const streetAddress = `${faker.number.int({ min: 1, max: 999 })} ${street}`;
                
                const houseAdjectives = ["Sang trọng", "Hiện đại", "Cao cấp", "View siêu đẹp", "Trung tâm", "Ấm cúng", "Vintage", "Gần sân bay", "Yên tĩnh", "Rộng rãi", "Cổ điển", "Tối giản", "View thành phố", "Đẳng cấp", "Ngập tràn ánh sáng", "Nghỉ dưỡng", "Xanh mát", "Phá cách", "Retro", "Boutique", "Phong cách Nhật Bản", "Phong cách Hàn Quốc", "Bình yên"];
                const houseTypes = ["Căn hộ", "Studio", "Biệt thự", "Homestay", "Penthouse", "Duplex", "Nhà nguyên căn", "Loft", "Condotel", "Villa", "Phòng Master", "Căn hộ dịch vụ"];
                const landmarks = ["Landmark 81", "Phố đi bộ Nguyễn Huệ", "Chợ Bến Thành", "Bitexco", "Thảo Điền", "Vinhomes Central Park", "Bùi Viện", "Quận 1", "Sông Sài Gòn", "Dinh Độc Lập", "Nhà thờ Đức Bà", "Khu đô thị Sala", "Sân bay Tân Sơn Nhất", "Thảo Cầm Viên", "Hồ Con Rùa", "Chợ Lớn", "Cầu Ánh Sao", "Phú Mỹ Hưng", "Masteri Thảo Điền", "Công viên Tao Đàn", "Chợ Tân Định"];
                
                const title = `${faker.helpers.arrayElement(houseTypes)} ${faker.helpers.arrayElement(houseAdjectives)} gần ${faker.helpers.arrayElement(landmarks)} - ${streetAddress}`;
                const description = `Chào mừng bạn đến với ${title}. Chỗ ở của chúng tôi được trang bị đầy đủ tiện nghi hiện đại, thiết kế tinh tế mang lại cảm giác thoải mái nhất cho chuyến đi của bạn. Nằm ngay vị trí đắc địa tại ${district}, dễ dàng di chuyển đến các khu vực trung tâm và địa điểm du lịch nổi tiếng. Phù hợp cho nhóm ${guestCount} khách với ${bedroomCount} phòng ngủ và ${bedCount} giường êm ái. Wi-Fi tốc độ cao, bếp đầy đủ đồ dùng, và hỗ trợ nhận/trả phòng linh hoạt.`;
                
                const payload = {
                    title: title,
                    description: description,
                    slug: faker.helpers.slugify(title) + '-' + Date.now() + Math.floor(Math.random() * 1000),
                    type: faker.helpers.arrayElement(PROPERTY_TYPES),
                    basePrice: faker.number.float({ min: 300000.0, max: 5000000.0, fractionDigits: 0 }), // VND realistic price
                    currencyCode: "VND",
                    cleaningFee: faker.number.float({ min: 50000.0, max: 300000.0, fractionDigits: 0 }),
                    serviceFee: faker.number.float({ min: 20000.0, max: 100000.0, fractionDigits: 0 }),
                    weekendPremiumPercent: faker.number.float({ min: 0, max: 20, fractionDigits: 1 }),
                    latitude: getRandomInRange(loc.LatMin, loc.LatMax, 6),
                    longitude: getRandomInRange(loc.LngMin, loc.LngMax, 6),
                    countryCode: loc.CountryCode,
                    admin1Code: loc.Admin1Code,
                    admin2Code: district,
                    displayAddress: loc.DisplayAddress,
                    streetAddress: streetAddress,
                    guestCount,
                    bedroomCount,
                    bedCount,
                    bathroomCount,
                    allowPets: faker.datatype.boolean(),
                    allowSmoking: false,
                    allowEvents: faker.datatype.boolean(),
                    checkInTime: "14:00:00",
                    checkOutTime: "12:00:00",
                    flexibleCheckOut: faker.datatype.boolean(),
                    bookingMode: faker.helpers.arrayElement([0, 1]) // Manual or Instant
                };

                const form = new FormData();
                form.append('Payload', JSON.stringify(payload));
                
                // Pick 5 unique random images from 1 to 18
                const pickedImages = faker.helpers.arrayElements(Array.from({length: 18}, (_, i) => i + 1), 5);
                
                // Attach 5 beautiful images
                pickedImages.forEach((imgIndex, i) => {
                    const imgPath = path.join(__dirname, 'dummy_images', `img${imgIndex}.jpg`);
                    form.append('Images', fs.createReadStream(imgPath), `img${i + 1}.jpg`);
                });

                try {
                    const propRes = await axios.post(`${API_BASE_URL}/api/properties`, form, {
                        headers: {
                            ...form.getHeaders(),
                            'Authorization': `Bearer ${host.token}`
                        }
                    });
                    
                    const propertyId = propRes.data.data.id;
                    const slug = propRes.data.data.slug;

                    // Automatically publish the property (Status = 2)
                    await axios.patch(`${API_BASE_URL}/api/properties/${propertyId}/status`, { status: 2 }, {
                        headers: { 'Authorization': `Bearer ${host.token}` }
                    });

                    console.log(`✅ [Host ${h+1} | Prop ${p}] Published: ${payload.title} (Slug: ${slug})`);
                } catch (err) {
                    console.error(`❌ [Host ${h+1} | Prop ${p}] Failed:`, err.response?.data?.message || err.message);
                }
            }));
        }
    }

    await Promise.all(propertyTasks);

    // Summary
    console.log('\n=============================================');
    console.log('✅ BULK SEEDING COMPLETE!');
    
    // Export hosts to JSON file
    const exportPath = path.join(__dirname, 'seeded-hosts.json');
    fs.writeFileSync(exportPath, JSON.stringify(hosts, null, 2));
    
    console.log('Here are the Host accounts created. Use them to login and test:');
    hosts.forEach((h, i) => {
        console.log(`- Host ${i + 1}: ${h.email} | Pass: ${h.password}`);
    });
    console.log(`\n📁 Hosts list exported to: ${exportPath}`);
    console.log('=============================================');
}

runSeeder();
