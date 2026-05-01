---
trigger: always_on
---

# 🎨 FRONTEND RULES – REACT (SHADCN/UI + ZUSTAND)

Quy chuẩn viết code Frontend để đảm bảo tính nhất quán, module hóa và chống vỡ hệ thống.

---

## 🏛️ 1. Architecture Layers (Bắt buộc)

Dự án phân chia thành 5 lớp trách nhiệm rõ ràng:

1.  **API Layer (`features/{feature}/api/`):** 
    - Chỉ chứa các hàm gọi Axios thuần túy. Trả về đúng kiểu dữ liệu (DTO) từ Server.
2.  **Types & Mapper Layer (`features/{feature}/types/` & `features/{feature}/utils/`):**
    - `types/`: Định nghĩa `DTO` (từ API) và `Model` (cho UI).
    - `utils/`: Chứa các hàm **Mapper/Transformer** chuyển đổi từ DTO sang Model. Bắt buộc mapping để UI không phụ thuộc trực tiếp vào Schema của DB.
3.  **Hooks Layer (`features/{feature}/hooks/`):**
    - Chỉ dùng **TanStack Query** để quản lý server state.
    - **Không** chứa logic điều hướng (`useNavigate`) hay logic nghiệp vụ phức tạp.
4.  **Components Layer (`features/{feature}/components/`):**
    - Nhận dữ liệu từ Hooks và hiển thị.
    - Xử lý side-effects giao diện (Navigation, Toast) tại đây.
5.  **Shared Layer (`src/components/`, `src/hooks/`, `src/utils/`):**
    - Chứa các thành phần dùng chung toàn dự án (UI nguyên tử, hàm format date, auth provider).

---

## 🧠 2. State Management (Quy tắc 3 tầng)

1.  **Server State (TanStack Query):** Quản lý 100% dữ liệu từ API. Không bao giờ đưa dữ liệu này vào Zustand.
2.  **Global State (Zustand):** Chỉ lưu các trạng thái "xuyên trang" thực sự: `AuthSession`, `Theme`, `GlobalSettings`.
3.  **Local State (`useState`, `useReducer`):** Dùng cho logic nội bộ Component: `isOpen`, `isLoading`, `activeTab`. Không lạm dụng Zustand cho các trạng thái cục bộ.

---

## ⚙️ 3. Business Logic Boundary

- **Validation:** Sử dụng **Zod** schema (đặt trong `features/{feature}/utils/validation.ts`).
- **Data Transformation:** Mọi logic xử lý dữ liệu (tính toán, lọc, gộp) phải nằm trong các hàm **Transformer/Helper** thuần túy (Pure Functions), không viết trực tiếp trong Hook hay UI.
- **Side Effects:** Logic điều hướng (`useNavigate`) phải nằm ở tầng UI (Component) sau khi Hook báo trạng thái `isSuccess`.

---

## 🚨 4. Anti-Pattern Guard (Cấm)

- **Cấm gọi Axios trong UI:** Tuyệt đối không.
- **Cấm share Hook giữa các Feature:** Nếu logic giống nhau, hãy đưa vào `src/hooks/` (Shared).
- **Cấm dùng `any`:** Mọi biến phải có type/interface.
- **Cấm hardcode URL:** Phải dùng tệp cấu hình hoặc hằng số.

---

## 🤖 5. AI Rule

- Luôn kiểm tra xem tệp `types` đã có Mapper từ DTO sang Model chưa trước khi viết UI.
- Luôn ưu tiên viết logic xử lý dữ liệu thành Pure Functions trong thư mục `utils`.
- Luôn giữ Component "mỏng" nhất có thể.
