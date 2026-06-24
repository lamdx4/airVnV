const fs = require('fs');
const path = require('path');
const axios = require('axios');
const FormData = require('form-data');

const API_BASE_URL = 'http://localhost:8088'; // YARP Gateway
const EMAIL = 'seeder_host@airbnb.local';
const PASSWORD = 'Password123!';

async function runSeeder() {
    try {
        let accessToken = null;
        
        console.log('1. Attempting to log in as existing Seeder Host...');
        try {
            const loginRes = await axios.post(`${API_BASE_URL}/api/users/login`, {
                email: EMAIL,
                password: PASSWORD
            });
            accessToken = loginRes.data.data.accessToken;
            console.log('=> Logged in successfully. JWT Token acquired.');
        } catch (loginErr) {
            console.log('=> User does not exist yet. Proceeding to register...');
            
            console.log('\n2. Registering user...');
            const regRes = await axios.post(`${API_BASE_URL}/api/users/register`, {
                email: EMAIL,
                password: PASSWORD,
                fullName: "Seeder Host"
            });
            
            const message = regRes.data.data.message;
            const code = message.split('Code: ')[1].trim();
            console.log(`=> Registration initiated. Extracted Code: ${code}`);

            console.log('\n3. Verifying email...');
            await axios.post(`${API_BASE_URL}/api/users/verify-email`, {
                email: EMAIL,
                code: code
            });
            console.log('=> Email verified successfully.');

            console.log('\n4. Logging in...');
            const loginRes = await axios.post(`${API_BASE_URL}/api/users/login`, {
                email: EMAIL,
                password: PASSWORD
            });
            accessToken = loginRes.data.data.accessToken;
            console.log('=> Logged in successfully. JWT Token acquired.');
        }

        console.log('\n5. Creating property with beautiful generated images...');
        const payload = {
            title: "Luxury Villa with Pool",
            description: "A beautiful dummy villa for testing purposes. Enjoy the pool and the great view.",
            slug: `luxury-villa-${Date.now()}`,
            type: 1, // House
            basePrice: 150.0,
            currencyCode: "USD",
            cleaningFee: 30.0,
            serviceFee: 15.0,
            weekendPremiumPercent: 10.0,
            latitude: 21.0285,
            longitude: 105.8542,
            countryCode: "VN",
            admin1Code: "HN",
            displayAddress: "Hoan Kiem, Hanoi, Vietnam",
            streetAddress: "123 Test Street",
            guestCount: 4,
            bedroomCount: 2,
            bedCount: 3,
            bathroomCount: 2,
            allowPets: true,
            allowSmoking: false,
            allowEvents: false,
            checkInTime: "14:00:00",
            checkOutTime: "11:00:00",
            flexibleCheckOut: true,
            bookingMode: 1 // InstantBook
        };

        const form = new FormData();
        form.append('Payload', JSON.stringify(payload));
        
        // Đính kèm 5 ảnh
        for (let i = 1; i <= 5; i++) {
            const imgPath = path.join(__dirname, 'dummy_images', `img${i}.jpg`);
            form.append('Images', fs.createReadStream(imgPath), `img${i}.jpg`);
        }

        const propRes = await axios.post(`${API_BASE_URL}/api/properties`, form, {
            headers: {
                ...form.getHeaders(),
                'Authorization': `Bearer ${accessToken}`
            }
        });

        console.log('=> Property created successfully!');
        console.log(propRes.data);
        console.log('\n✅ SEEDING COMPLETE!');

    } catch (error) {
        console.error('\n❌ SEEDING FAILED!');
        if (error.response) {
            console.error('API Error Response:', error.response.data);
        } else {
            console.error(error.message);
        }
    }
}

runSeeder();
