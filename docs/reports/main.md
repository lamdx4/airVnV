**BỘ KHOA HỌC VÀ CÔNG NGHỆ HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG** ----------------------------i. 

# **BÁO CÁO ĐỒ ÁN MÔN HỌC** 

**ĐỀ TÀI:** 

Môn học: Giảng viên hướng dẫn Thực hiện bởi nhóm sinh viên, bao gồm: 

- **TP.HCM - 2026** 

- ——— 

**Hệ thống khử nhiễu** 



## **MỤC LỤC** 

|**MỤC LỤC ................................................................................................................................ 1**|**MỤC LỤC ................................................................................................................................ 1**|
|---|---|
|**Chương I: TỔNG QUAN........................................................................................................ 4**||
|1.|Giới thiệu đề tài. ............................................................................................................4|
||_1.1._<br>Bối cảnh ..................................................................................................................4|
||_1.2._<br>Mục tiêu ..................................................................................................................4|
|2.|Cơ sở lý thuyết. .............................................................................................................4|
|**Chương II: THIẾT KẾ HỆ THỐNG .................................................................................... 4**||
|1.|Phân tích yêu cầu hệ thống ............................................................................................4|
||_1.1._<br>Yêu cầu chức năng (Functional Requirements) ......................................................4|
||_1.2._<br>Yêu cầu phi chức năng (Non-Functional Requirements) .......................................4|
|2.|Thiết kế lược đồ dữ liệu tích hợp ..................................................................................4|
|3.|Kiến trúc tổng thể ..........................................................................................................4|
||_3.1._<br>Sơ đồ Kiến trúc Tổng thể ........................................................................................4|
||_3.2._<br>Áp dụng Mẫu thiết kế (Design Patterns / Architectural Patterns) ...........................5|
|4.|Thiết kế cấu hình các node và mạng .............................................................................5|
|**Chương III: TRIỂN KHAI HỆ THỐNG .............................................................................. 5**||
|1.|Môi trường và công nghệ triển khai ..............................................................................5|
|2.|Kết quả triển khai các chức năng chính ........................................................................5|
|3.|Đánh giá kết quả thực nghiệm .......................................................................................5|
|**Chương IV: KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN ........................................................ 5**||
|1.|Những kết quả đã đạt được. ..........................................................................................5|
|2.|Hạn chế và hướng phát triển. ........................................................................................5|
|**TÀI**|**LIỆU THAM KHẢO ...................................................................................................... 6**|
|**BẢNG PHÂN CÔNG CÔNG VIỆC ...................................................................................... 6**||



1 

**Hệ thống khử nhiễu** 



2 

**Hệ thống khử nhiễu** 



No table of figures entries found. 

3 

**Hệ thống khử nhiễu** 



## **Chương I: TỔNG QUAN** 

## **1. Giới thiệu đề tài.** 

- _**1.1.**_ **Bối cảnh** 

- _**1.2.**_ **Mục tiêu** 

**2. Cơ sở lý thuyết.** 

4 

**Hệ thống khử nhiễu** 



## **Chương II: THIẾT KẾ HỆ THỐNG** 

## **1. Phân tích yêu cầu hệ thống** 

## _**1.1.**_ **Yêu cầu chức năng (Functional Requirements)** 

- Vẽ **Use Case Diagram** tổng thể và đặc tả các luồng nghiệp vụ chính (Đặt hàng, Xử lý thanh toán, Pre-order...). 

## _**1.2.**_ **Yêu cầu phi chức năng (Non-Functional Requirements)** 

- Nêu rõ các chỉ số về hiệu năng, khả năng mở rộng (Scalability), tính toàn vẹn dữ liệu (Consistency) hoặc tính sẵn sàng (Availability). 

## **2. Thiết kế lược đồ dữ liệu tích hợp** 

- Vẽ sơ đồ ERD (Entity-Relationship Diagram) tổng thể của hệ thống. Nếu làm Microservices/Database phân tán: Giải thích cách phân rã Database theo từng Service (Database-per-service), cách thiết kế các bảng tích hợp/quan hệ giữa các thành phần dữ liệu, hoặc cấu trúc lưu trữ đặc biệt (như hệ thống danh mục nhiều cấp, quản lý trạng thái). 

## **3. Kiến trúc tổng thể** 

## _**3.1.**_ **Sơ đồ Kiến trúc Tổng thể** 

- Bạn dùng Monolith (Đơn khối), Clean/Hexagonal Architecture, hay Microservices? **Tại sao lại chọn nó?** (Hãy so sánh ưu/nhược điểm dựa trên yêu cầu phi chức năng ở Chương 1). **Sơ đồ Kiến trúc Hệ thống (System Architecture Diagram):** Vẽ luồng đi từ Client $\rightarrow$ API Gateway $\rightarrow$ các Service/Module $\rightarrow$ Message Broker (Kafka/RabbitMQ) $\rightarrow$ Databases. 

## _**3.2.**_ **Áp dụng Mẫu thiết kế (Design Patterns / Architectural Patterns)** 

- Áp dụng Mẫu thiết kế (Design Patterns): Liệt kê và vẽ Class Diagram / Sequence Diagram minh họa các pattern đã cài đặt trong code (ví dụ: CQRS, Outbox Pattern để đồng bộ dữ liệu, Repository & Unit of Work, Factory Pattern cho Payment Service...). Giải thích rõ các thành phần này phối hợp với nhau như thế nào. 

## **4. Thiết kế cấu hình các node và mạng** 

- Đây là phần thiết kế vật lý (Deployment View): Vẽ sơ đồ triển khai hệ thống (Deployment Diagram). Mô tả cấu hình các Node chạy dịch vụ (Ví dụ: Node chạy 

5 

**Hệ thống khử nhiễu** 



API Gateway, các Node chạy các Service độc lập, Node chạy Database Cluster, Node chạy Message Broker). Mô tả phân vùng mạng (Cấu hình Docker Compose, cách các service giao tiếp nội bộ qua mạng ảo, cách mở port ra ngoài internet, hoặc cấu hình định tuyến qua API Gateway). 

6 

**Hệ thống khử nhiễu** 



## **Chương III: TRIỂN KHAI HỆ THỐNG** 

## **1. Môi trường và công nghệ triển khai** 

- Chi tiết phiên bản ngôn ngữ, hệ điều hành (như Fedora/Ubuntu Server), các công cụ quản lý container (Docker). 

## **2. Kết quả triển khai các chức năng chính** 

- Chụp màn hình giao diện demo sản phẩm chạy thực tế (Frontend, kết quả trả về từ API, hoặc log xử lý bất đồng bộ của các Service trên Terminal). 

## **3. Đánh giá kết quả thực nghiệm** 

- Đánh giá xem hệ thống chạy có đúng theo kịch bản thiết kế ở Chương II không, khả năng xử lý dữ liệu và phản hồi của các node ra sao. 

7 

**Hệ thống khử nhiễu** 



## **Chương IV: KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN** 

## **1. Những kết quả đã đạt được.** 

- Đối chiếu lại với mục tiêu ban đầu ở Chương I và các tiêu chí thiết kế ở Chương II xem đã hoàn thành được bao nhiêu %. 

## **2. Hạn chế và hướng phát triển.** 

- Thẳng thắn chỉ ra các điểm chưa tối ưu (Technical Debt) và đề xuất hướng nâng cấp (ví dụ: Cấu hình Auto-scaling, áp dụng Saga Pattern cho các transaction phức tạp hơn). 

8 

## **BẢNG PHÂN CÔNG CÔNG VIỆC** 

10 

