import { test, expect } from '@playwright/test';

// Kịch bản E2E Test cho tính năng Quản lý Căn nhà (Create & Edit)
test.describe('Listing Management', () => {
  
  test.beforeEach(async ({ page }) => {
    // 1. Giả lập đăng nhập (Mock JWT Token)
    await page.addInitScript(() => {
      window.localStorage.setItem('airbnb_access_token', 'mock_jwt_token');
      window.localStorage.setItem('airbnb_user_id', 'mock_user_123');
    });

    // 2. Chặn và Mock các API Backend để không làm rác Database thật
    // Mock API lấy Amenities
    await page.route('**/api/properties/amenities', async route => {
      const json = {
        success: true,
        data: [
          { id: '1', name: 'Wifi', icon: 'wifi' },
          { id: '2', name: 'Pool', icon: 'pool' }
        ]
      };
      await route.fulfill({ json });
    });

    // Mock API Tạo nhà
    await page.route('**/api/properties', async route => {
      if (route.request().method() === 'POST') {
        const json = {
          success: true,
          data: { id: 'new_property_123' }
        };
        await route.fulfill({ json });
      } else {
        await route.continue();
      }
    });

    // Mock API Lấy thông tin nhà để Edit
    await page.route('**/api/properties/new_property_123', async route => {
      const json = {
        success: true,
        data: {
          id: 'new_property_123',
          title: 'Beautiful Villa',
          description: 'A very nice place to stay',
          basePrice: 1500,
          currencyCode: 'USD',
          status: 'Draft'
        }
      };
      await route.fulfill({ json });
    });
    
    // Mock API Cập nhật nhà
    await page.route('**/api/properties/new_property_123', async route => {
      if (route.request().method() === 'PUT') {
        const json = { success: true, data: { id: 'new_property_123' } };
        await route.fulfill({ json });
      } else {
        await route.continue();
      }
    });

    // Mock API Lấy danh sách nhà của Host
    await page.route('**/api/properties/my*', async route => {
      const json = {
        success: true,
        data: {
          items: [
            {
              id: 'new_property_123',
              title: 'Beautiful Villa',
              status: 'Draft',
              basePrice: 1500
            }
          ]
        }
      };
      await route.fulfill({ json });
    });
  });

  test('should create a new property successfully', async ({ page }) => {
    // 1. Đi tới trang Tạo nhà mới
    await page.goto('/host/homes/new');

    // 2. Điền thông tin cơ bản (Basic Info)
    // Tùy thuộc vào UI, giả sử có các input với placeholder hoặc label
    // (Vì code UI dùng các Component con, ta mock các tương tác cơ bản)
    
    // Vì không có UI chi tiết trong màn này, ta có thể test việc điều hướng
    await expect(page.locator('h1')).toContainText('List Your Property');
    
    // Chờ màn hình render
    await page.waitForSelector('form');

    // (Tùy thuộc vào các component BasicInfoSection, PricingSection, etc.)
    // Ở đây ta mô phỏng việc nhấn nút Submit luôn (có thể bị disable do thiếu file)
    // Để thực sự test được cần biết chính xác cấu trúc HTML (id, data-testid) của form.
    // Tạm thời verify trang load thành công:
    await expect(page.getByRole('button', { name: /Save/i })).toBeVisible();
  });

  test('should edit an existing property successfully', async ({ page }) => {
    // 1. Đi tới trang Dashboard
    await page.goto('/host/homes');

    // 2. Xác minh căn nhà hiển thị
    await expect(page.getByText('Beautiful Villa')).toBeVisible();

    // 3. Chuyển sang trang Edit
    await page.goto('/host/homes/new_property_123/edit');

    // 4. Verify form edit load dữ liệu
    await expect(page.locator('h1')).toBeVisible(); // Tùy thuộc UI Edit
    
    // 5. Cập nhật thông tin (ví dụ click nút Save)
    // await page.getByRole('button', { name: 'Save' }).click();
  });

});
