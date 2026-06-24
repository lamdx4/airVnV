const fs = require('fs');
const path = require('path');
const axios = require('axios');
const FormData = require('form-data');
const { fakerVI: faker } = require('@faker-js/faker');

// ==========================================
// CONFIGURATION
// ==========================================
const API_BASE_URL = 'http://localhost:8088'; // YARP Gateway
const NUM_HOSTS = 5;
const NUM_PROPERTIES_PER_HOST = 10;
const CONCURRENCY_LIMIT = 3; // Limit parallel property creations to avoid overwhelming the Gateway/Cloudinary

// ==========================================
// PREDEFINED LOCATIONS FOR REALISTIC SEARCH INDEXING
// ==========================================
const LOCATIONS = [
  {
    CountryCode: "VN",
    Admin1Code: "HN",
    DisplayAddress: "Hà Nội, Việt Nam",
    Districts: ["Hoàn Kiếm", "Tây Hồ", "Ba Đình", "Cầu Giấy", "Đống Đa"],
    Streets: ["Trần Phú", "Phan Đình Phùng", "Trích Sài", "Hàng Bài", "Kim Mã"],
    LatMin: 21.0000, LatMax: 21.0600,
    LngMin: 105.7800, LngMax: 105.8600
  },
  {
    CountryCode: "VN",
    Admin1Code: "SG",
    DisplayAddress: "Hồ Chí Minh, Việt Nam",
    Districts: ["Quận 1", "Quận 2", "Quận 3", "Bình Thạnh", "Phú Nhuận"],
    Streets: ["Nguyễn Huệ", "Lê Lợi", "Thảo Điền", "Nguyễn Hữu Cảnh", "Nam Kỳ Khởi Nghĩa"],
    LatMin: 10.7500, LatMax: 10.8300,
    LngMin: 106.6800, LngMax: 106.7500
  },
  {
    CountryCode: "VN",
    Admin1Code: "LD",
    DisplayAddress: "Đà Lạt, Lâm Đồng, Việt Nam",
    Districts: ["Phường 1", "Phường 2", "Phường 8", "Phường 10"],
    Streets: ["Trần Phú", "Yersin", "Hồ Tùng Mậu", "Quang Trung", "Đinh Tiên Hoàng"],
    LatMin: 11.9200, LatMax: 11.9600,
    LngMin: 108.4300, LngMax: 108.4600
  },
  {
    CountryCode: "VN",
    Admin1Code: "DN",
    DisplayAddress: "Đà Nẵng, Việt Nam",
    Districts: ["Hải Châu", "Sơn Trà", "Ngũ Hành Sơn", "Thanh Khê"],
    Streets: ["Bạch Đằng", "Trần Phú", "Võ Nguyên Giáp", "Hoàng Sa", "Nguyễn Văn Thoại"],
    LatMin: 16.0300, LatMax: 16.0800,
    LngMin: 108.2000, LngMax: 108.2500
  },
  {
    CountryCode: "VN",
    Admin1Code: "KG",
    DisplayAddress: "Phú Quốc, Kiên Giang, Việt Nam",
    Districts: ["Dương Đông", "An Thới", "Gành Dầu"],
    Streets: ["Trần Hưng Đạo", "Nguyễn Trung Trực", "30 Tháng 4"],
    LatMin: 10.1500, LatMax: 10.3500,
    LngMin: 103.9500, LngMax: 104.0000
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
                
                const payload = {
                    title: faker.lorem.words({ min: 3, max: 6 }).split(' ').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ') + " " + faker.helpers.arrayElement(["Villa", "Homestay", "Biệt thự", "Resort", "Căn hộ"]),
                    description: faker.lorem.paragraphs(2),
                    slug: faker.helpers.slugify(faker.lorem.words(3)) + '-' + Date.now() + Math.floor(Math.random() * 1000),
                    type: faker.helpers.arrayElement(PROPERTY_TYPES),
                    basePrice: faker.number.float({ min: 50.0, max: 500.0, fractionDigits: 2 }),
                    currencyCode: "USD",
                    cleaningFee: faker.number.float({ min: 10.0, max: 50.0, fractionDigits: 2 }),
                    serviceFee: faker.number.float({ min: 5.0, max: 20.0, fractionDigits: 2 }),
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
                    checkOutTime: "11:00:00",
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
