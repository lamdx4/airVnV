import os

def escape_latex(text):
    if not isinstance(text, str):
        return text
    return text.replace('_', '\\_').replace('&', '\\&').replace('%', '\\%')

def create_latex_table(uc_id, name, actors, desc, precond, postcond, basic_flow, alt_flows, frs):
    uc_id = escape_latex(uc_id)
    name = escape_latex(name)
    actors = escape_latex(actors)
    desc = escape_latex(desc)
    precond = escape_latex(precond)
    postcond = escape_latex(postcond)
    
    basic_flow_str = "\\newline ".join([f"{i+1}. {escape_latex(step)}" for i, step in enumerate(basic_flow)])
    
    alt_str = "\\newline ".join([f"\\textbf{{{escape_latex(alt['name'])}}}: {escape_latex(alt['desc'])}" for alt in alt_flows])
    if not alt_str: alt_str = "Không có."
    
    fr_str = "\\newline ".join([f"$\\diamond$ \\textbf{{{escape_latex(fr['id'])}}}: {escape_latex(fr['desc'])}" for fr in frs])
    
    cc = "\\cellcolor{gray!15}\\bfseries"
    return f"""
\\begin{{table}}[H]
\\renewcommand{{\\arraystretch}}{{1.3}}
\\centering
\\begin{{tabularx}}{{\\textwidth}}{{|p{{4.5cm}}|X|}}
\\hline
{cc} Mã Usecase & \\textbf{{{uc_id}}} \\\\ \\hline
{cc} Tên Usecase & \\textbf{{{name}}} \\\\ \\hline
{cc} Tác nhân (Actors) & {actors} \\\\ \\hline
{cc} Mô tả ngắn & {desc} \\\\ \\hline
{cc} Điều kiện tiên quyết & {precond} \\\\ \\hline
{cc} Điều kiện đảm bảo & {postcond} \\\\ \\hline
{cc} Luồng sự kiện chính & {basic_flow_str} \\\\ \\hline
{cc} Luồng rẽ nhánh & {alt_str} \\\\ \\hline
{cc} Yêu cầu chức năng (FR) & {fr_str} \\\\ \\hline
\\end{{tabularx}}
\\end{{table}}
"""

usecases = [
    # Nhóm 1: User (01-08)
    {
        "id": "UC-01", "name": "Đăng ký tài khoản", "actors": "Khách (Guest), Chủ nhà (Host)",
        "desc": "Người dùng đăng ký tài khoản mới trên hệ thống AirVnV.",
        "precond": "Người dùng chưa đăng nhập.",
        "postcond": "Tài khoản được tạo ở trạng thái Inactive, OTP được gửi qua email.",
        "basic_flow": ["Người dùng nhập thông định cơ bản.", "Hệ thống kiểm tra email.", "Hệ thống tạo user và gửi OTP."],
        "alt_flows": [{"name": "Email đã tồn tại", "desc": "Hệ thống báo lỗi Email already exists."}],
        "frs": [{"id": "FR_01.1", "desc": "Hệ thống từ chối nếu email trùng lặp."}]
    },
    {
        "id": "UC-02", "name": "Xác thực Email (Verify)", "actors": "Người dùng",
        "desc": "Người dùng nhập mã OTP để kích hoạt tài khoản.",
        "precond": "Tài khoản ở trạng thái Inactive, có mã OTP hợp lệ.",
        "postcond": "Tài khoản chuyển sang trạng thái Active.",
        "basic_flow": ["Người dùng nhập OTP.", "Hệ thống kiểm tra tính hợp lệ.", "Cập nhật trạng thái Active."],
        "alt_flows": [{"name": "Mã hết hạn", "desc": "Hệ thống báo lỗi Verification code expired."}],
        "frs": [{"id": "FR_02.1", "desc": "Mã OTP chỉ có hiệu lực trong 15 phút."}]
    },
    {
        "id": "UC-03", "name": "Đăng nhập hệ thống", "actors": "Người dùng",
        "desc": "Người dùng đăng nhập bằng mật khẩu hoặc Google SSO.",
        "precond": "Tài khoản đã kích hoạt.",
        "postcond": "Nhận được JWT Token để truy cập hệ thống.",
        "basic_flow": ["Người dùng nhập email/mật khẩu.", "Hệ thống kiểm tra hash.", "Hệ thống cấp phát JWT Token."],
        "alt_flows": [{"name": "Sai mật khẩu", "desc": "Báo lỗi Invalid email or password."}],
        "frs": [{"id": "FR_03.1", "desc": "Mật khẩu phải được mã hóa trước khi so sánh."}]
    },
    {
        "id": "UC-04", "name": "Cập nhật hồ sơ cá nhân", "actors": "Người dùng",
        "desc": "Thay đổi thông tin như tên, ảnh đại diện, số điện thoại.",
        "precond": "Người dùng đã đăng nhập.",
        "postcond": "Thông tin mới được lưu trữ trong CSDL.",
        "basic_flow": ["Người dùng sửa thông tin.", "Hệ thống validate.", "Cập nhật CSDL."],
        "alt_flows": [{"name": "Lỗi định dạng", "desc": "Báo lỗi Validation Failed (400)."}],
        "frs": [{"id": "FR_04.1", "desc": "Số điện thoại phải đúng định dạng quốc gia."}]
    },
    {
        "id": "UC-05", "name": "Đổi mật khẩu", "actors": "Người dùng",
        "desc": "Người dùng đang đăng nhập muốn đổi mật khẩu mới.",
        "precond": "Đã đăng nhập.",
        "postcond": "Mật khẩu được cập nhật, các phiên đăng nhập khác bị đăng xuất.",
        "basic_flow": ["Nhập mật khẩu cũ và mới.", "Hệ thống kiểm tra.", "Cập nhật mật khẩu."],
        "alt_flows": [{"name": "Sai mật khẩu cũ", "desc": "Báo lỗi và từ chối cập nhật."}],
        "frs": [{"id": "FR_05.1", "desc": "Mật khẩu mới phải mạnh (có ký tự đặc biệt, hoa, thường)."}]
    },
    {
        "id": "UC-06", "name": "Quên mật khẩu", "actors": "Người dùng",
        "desc": "Yêu cầu khôi phục mật khẩu qua email.",
        "precond": "Không yêu cầu đăng nhập.",
        "postcond": "Gửi link reset kèm token vào email.",
        "basic_flow": ["Người dùng nhập email.", "Hệ thống tạo Reset Token.", "Gửi email chứa link khôi phục."],
        "alt_flows": [{"name": "Email không tồn tại", "desc": "Trả về 200 OK nhưng không gửi mail để tránh lộ thông tin."}],
        "frs": [{"id": "FR_06.1", "desc": "Token reset chỉ có hiệu lực 1 lần duy nhất trong 30 phút."}]
    },
    {
        "id": "UC-07", "name": "Làm mới Token (Refresh Token)", "actors": "Người dùng",
        "desc": "Lấy JWT mới khi token cũ hết hạn.",
        "precond": "Sở hữu Refresh Token hợp lệ.",
        "postcond": "Cấp JWT mới và Refresh Token mới.",
        "basic_flow": ["Client gửi Refresh Token.", "Hệ thống kiểm tra.", "Trả về bộ token mới."],
        "alt_flows": [{"name": "Token bị thu hồi", "desc": "Báo lỗi Token is revoked và yêu cầu đăng nhập lại."}],
        "frs": [{"id": "FR_07.1", "desc": "Cơ chế xoay vòng (Rotation) Refresh Token để chống đánh cắp."}]
    },
    {
        "id": "UC-08", "name": "Nhận Thông báo (Notifications)", "actors": "Người dùng",
        "desc": "Xem danh sách thông báo in-app và đánh dấu đã đọc.",
        "precond": "Đã đăng nhập.",
        "postcond": "Danh sách thông báo được cập nhật trạng thái IsRead.",
        "basic_flow": ["Lấy danh sách thông báo qua API.", "Hiển thị lên chuông thông báo.", "Đánh dấu là đã đọc."],
        "alt_flows": [],
        "frs": [{"id": "FR_08.1", "desc": "Hệ thống đẩy thông báo realtime qua SignalR."}]
    },

    # Nhóm 2: Bất động sản (09-17)
    {
        "id": "UC-09", "name": "Đăng tin Bất động sản mới", "actors": "Chủ nhà (Host)",
        "desc": "Khởi tạo một Bất động sản mới dưới dạng Draft.",
        "precond": "Chủ nhà đã đăng nhập.",
        "postcond": "Bất động sản được tạo với trạng thái Draft.",
        "basic_flow": ["Host nhập tiêu đề, mô tả.", "Hệ thống validate.", "Tạo bản ghi trong propdb."],
        "alt_flows": [{"name": "Dữ liệu thiếu", "desc": "Báo lỗi Validation Failed."}],
        "frs": [{"id": "FR_09.1", "desc": "Tiêu đề không được để trống và tối đa 100 ký tự."}]
    },
    {
        "id": "UC-10", "name": "Quản lý hình ảnh & tiện ích", "actors": "Chủ nhà (Host)",
        "desc": "Host tải lên hình ảnh và chọn tiện ích (Wifi, Bể bơi).",
        "precond": "Bất động sản thuộc sở hữu của Host.",
        "postcond": "Hình ảnh được lưu (Cloudinary/S3), tiện ích được map.",
        "basic_flow": ["Host chọn ảnh/tiện ích.", "Upload lên Cloud.", "Lưu URL vào CSDL."],
        "alt_flows": [{"name": "Sai định dạng ảnh", "desc": "Từ chối upload nếu không phải JPG/PNG."}],
        "frs": [{"id": "FR_10.1", "desc": "Mỗi phòng phải có ít nhất 5 ảnh trước khi submit."}]
    },
    {
        "id": "UC-11", "name": "Thiết lập Giá theo ngày (Smart Pricing)", "actors": "Chủ nhà (Host)",
        "desc": "Cấu hình giá cao hơn vào cuối tuần hoặc mùa cao điểm.",
        "precond": "Phòng đã được tạo.",
        "postcond": "Cập nhật bảng giá tùy chỉnh.",
        "basic_flow": ["Host chọn các ngày đặc biệt.", "Nhập giá mới.", "Lưu xuống cơ sở dữ liệu."],
        "alt_flows": [{"name": "Giá không hợp lệ", "desc": "Báo lỗi nếu giá nhập < 0."}],
        "frs": [{"id": "FR_11.1", "desc": "Hệ thống phải ưu tiên giá tùy chỉnh thay vì giá mặc định."}]
    },
    {
        "id": "UC-12", "name": "Khóa lịch rảnh (Block Dates)", "actors": "Chủ nhà (Host)",
        "desc": "Host chủ động khóa một khoảng thời gian không cho khách đặt.",
        "precond": "Phòng đã được Publish.",
        "postcond": "Khoảng thời gian bị chặn trong lịch Booking.",
        "basic_flow": ["Chọn khoảng ngày.", "Hệ thống kiểm tra xem có ai đã đặt chưa.", "Tạo BlockAvailability."],
        "alt_flows": [{"name": "Trùng lịch", "desc": "Báo lỗi không thể khóa ngày đã có khách đặt."}],
        "frs": [{"id": "FR_12.1", "desc": "Từ chối block nếu ngày đó đang có Booking Pending/Confirmed."}]
    },
    {
        "id": "UC-13", "name": "Xóa Bất động sản", "actors": "Chủ nhà (Host)",
        "desc": "Host xóa vĩnh viễn bài đăng đang lưu nháp.",
        "precond": "Phòng phải ở trạng thái Draft.",
        "postcond": "Dữ liệu bị xóa mềm (Soft Delete).",
        "basic_flow": ["Host chọn phòng nháp.", "Bấm Xóa.", "Cập nhật IsDeleted = true."],
        "alt_flows": [{"name": "Đã publish", "desc": "Báo lỗi Cannot delete published property. Must suspend first."}],
        "frs": [{"id": "FR_13.1", "desc": "Xóa toàn bộ hình ảnh trên Cloudinary tương ứng."}]
    },
    {
        "id": "UC-14", "name": "Gửi duyệt Bất động sản (Submit)", "actors": "Chủ nhà (Host)",
        "desc": "Chuyển trạng thái từ Draft sang Pending Approval.",
        "precond": "Phòng có đủ ảnh, địa chỉ, tiện ích, giá.",
        "postcond": "Trạng thái thành Pending Approval, gửi thông báo cho Admin.",
        "basic_flow": ["Host nhấn Submit.", "Hệ thống validate toàn diện.", "Đổi trạng thái."],
        "alt_flows": [{"name": "Thiếu dữ liệu", "desc": "Từ chối Submit nếu thiếu giá tiền hoặc địa chỉ."}],
        "frs": [{"id": "FR_14.1", "desc": "Trạng thái chỉ chuyển được từ Draft sang Pending Approval."}]
    },
    {
        "id": "UC-15", "name": "Cập nhật trạng thái phòng", "actors": "Chủ nhà (Host)",
        "desc": "Tạm ngưng (Suspend) hoặc Mở lại (Reinstate) phòng.",
        "precond": "Phòng đang ở trạng thái Published hoặc Suspended.",
        "postcond": "Trạng thái phòng được cập nhật.",
        "basic_flow": ["Host chọn thay đổi trạng thái.", "Cập nhật DB.", "Gửi Event sang SearchService."],
        "alt_flows": [{"name": "Từ Draft", "desc": "Báo lỗi Cannot revert to Draft from current status."}],
        "frs": [{"id": "FR_15.1", "desc": "Đồng bộ trạng thái này sang Elasticsearch qua Kafka."}]
    },
    {
        "id": "UC-16", "name": "Xem Dashboard Chủ nhà", "actors": "Chủ nhà (Host)",
        "desc": "Host xem tổng quan thu nhập và lượt xem.",
        "precond": "Là Host.",
        "postcond": "Trả về dữ liệu Analytics.",
        "basic_flow": ["Lấy danh sách booking tháng này.", "Lấy lượt view từ SearchService.", "Hiển thị biểu đồ."],
        "alt_flows": [],
        "frs": [{"id": "FR_16.1", "desc": "Dữ liệu Dashboard phải được cache trên Redis để tối ưu tốc độ."}]
    },
    {
        "id": "UC-17", "name": "Phản hồi đánh giá (Host Reply)", "actors": "Chủ nhà (Host)",
        "desc": "Host bình luận lại bên dưới đánh giá của khách.",
        "precond": "Phòng có đánh giá hợp lệ.",
        "postcond": "Lưu phản hồi và hiển thị công khai.",
        "basic_flow": ["Host nhập nội dung reply.", "Lưu vào DB.", "Gửi thông báo cho khách."],
        "alt_flows": [{"name": "Đã phản hồi", "desc": "Báo lỗi Host already replied to this review."}],
        "frs": [{"id": "FR_17.1", "desc": "Nội dung phản hồi không chứa từ ngữ thô tục."}]
    },

    # Nhóm 3: Tìm kiếm (18-20)
    {
        "id": "UC-18", "name": "Tìm kiếm Bất động sản (Elasticsearch)", "actors": "Khách (Guest)",
        "desc": "Tìm kiếm phòng trống theo địa điểm, giá, ngày.",
        "precond": "Không yêu cầu đăng nhập.",
        "postcond": "Trả về danh sách phòng trống.",
        "basic_flow": ["Khách nhập tiêu chí tìm kiếm.", "SearchService truy vấn Elasticsearch.", "Trả về kết quả phân trang."],
        "alt_flows": [{"name": "Lỗi Elasticsearch", "desc": "Fallback về PostgreSQL (nếu có) hoặc báo lỗi 500."}],
        "frs": [{"id": "FR_18.1", "desc": "Chỉ trả về các phòng có trạng thái Published."}]
    },
    {
        "id": "UC-19", "name": "Tìm kiếm gợi ý (Autocomplete)", "actors": "Khách (Guest)",
        "desc": "Gợi ý tên thành phố hoặc địa danh khi khách gõ phím.",
        "precond": "Không yêu cầu đăng nhập.",
        "postcond": "Trả về mảng chuỗi địa danh.",
        "basic_flow": ["Khách gõ chữ cái đầu.", "SearchService tìm gần đúng (Fuzzy Match).", "Trả về danh sách top 5."],
        "alt_flows": [],
        "frs": [{"id": "FR_19.1", "desc": "Thời gian phản hồi autocomplete phải < 50ms."}]
    },
    {
        "id": "UC-20", "name": "Xem chi tiết Bất động sản", "actors": "Khách (Guest)",
        "desc": "Xem chi tiết phòng, giá, tiện ích và các review.",
        "precond": "Không yêu cầu.",
        "postcond": "Dữ liệu chi tiết được hiển thị.",
        "basic_flow": ["Chọn phòng.", "Lấy dữ liệu từ PropertyService và ReviewService.", "Hiển thị cho khách."],
        "alt_flows": [{"name": "Phòng không tồn tại", "desc": "Báo lỗi Property not found (404)."}],
        "frs": [{"id": "FR_20.1", "desc": "Nếu phòng bị ẩn, hệ thống trả về 404 Not Found."}]
    },

    # Nhóm 4: Booking (21-25)
    {
        "id": "UC-21", "name": "Đặt phòng (Create Booking)", "actors": "Khách (Guest)",
        "desc": "Tạo yêu cầu đặt phòng cho một khoảng thời gian.",
        "precond": "Đã đăng nhập.",
        "postcond": "Tạo đơn Pending, phát sự kiện sang PaymentService.",
        "basic_flow": ["Khách chọn ngày và số người.", "Hệ thống khóa lịch (Idempotent).", "Tạo Booking Pending và bắn Event."],
        "alt_flows": [{"name": "Ngày đã bị đặt", "desc": "Báo lỗi These dates are already booked."}],
        "frs": [{"id": "FR_21.1", "desc": "Tổng chi phí = Số đêm x Giá đêm + Phí dịch vụ."}]
    },
    {
        "id": "UC-22", "name": "Chủ nhà Duyệt/Từ chối đặt phòng", "actors": "Chủ nhà (Host)",
        "desc": "Host quyết định chấp nhận hoặc từ chối khách.",
        "precond": "Có đơn Booking ở trạng thái Pending Host Approval.",
        "postcond": "Booking chuyển sang Approved hoặc Rejected.",
        "basic_flow": ["Host xem đơn.", "Chọn Duyệt.", "Cập nhật trạng thái và thông báo Khách."],
        "alt_flows": [{"name": "Đơn không tồn tại", "desc": "Báo lỗi Booking not found."}],
        "frs": [{"id": "FR_22.1", "desc": "Host chỉ được thao tác trên các đơn thuộc Property của mình."}]
    },
    {
        "id": "UC-23", "name": "Khách Hủy đặt phòng", "actors": "Khách (Guest)",
        "desc": "Khách chủ động hủy đơn.",
        "precond": "Đơn đang ở trạng thái Pending hoặc Confirmed.",
        "postcond": "Booking chuyển sang Cancelled, kích hoạt hoàn tiền nếu cần.",
        "basic_flow": ["Khách nhấn Hủy.", "Kiểm tra chính sách hủy.", "Cập nhật trạng thái và bắn RefundEvent."],
        "alt_flows": [{"name": "Quá hạn hủy", "desc": "Từ chối hủy nếu vi phạm chính sách (ví dụ sát giờ check-in)."}],
        "frs": [{"id": "FR_23.1", "desc": "Hệ thống tự động tính số tiền hoàn lại dựa trên Policy."}]
    },
    {
        "id": "UC-24", "name": "Chủ nhà Hủy đặt phòng", "actors": "Chủ nhà (Host)",
        "desc": "Host hủy đơn trong trường hợp khẩn cấp.",
        "precond": "Đơn đang ở trạng thái Confirmed.",
        "postcond": "Hủy đơn, hoàn 100% tiền cho khách và phạt Host.",
        "basic_flow": ["Host chọn hủy.", "Ghi nhận lý do.", "Bắn Event phạt Host và Refund cho khách."],
        "alt_flows": [{"name": "Đã check-in", "desc": "Không thể hủy khi đã qua ngày Check-in."}],
        "frs": [{"id": "FR_24.1", "desc": "Trừ tự động phí phạt (Penalty) vào tài khoản Host."}]
    },
    {
        "id": "UC-25", "name": "Xem Lịch sử Đặt phòng", "actors": "Khách (Guest)",
        "desc": "Khách xem danh sách chuyến đi sắp tới và quá khứ.",
        "precond": "Đã đăng nhập.",
        "postcond": "Trả về danh sách Booking.",
        "basic_flow": ["Khách vào Trips.", "Query BookingService.", "Trả về dữ liệu phân trang."],
        "alt_flows": [],
        "frs": [{"id": "FR_25.1", "desc": "Các đơn Upcoming phải hiển thị trên cùng."}]
    },

    # Nhóm 5: Payment (26-33)
    {
        "id": "UC-26", "name": "Khởi tạo Thanh toán", "actors": "Hệ thống, Khách",
        "desc": "Tạo Session thanh toán VNPay/Stripe.",
        "precond": "Đã có Booking ID.",
        "postcond": "Cấp URL thanh toán cho khách.",
        "basic_flow": ["PaymentService nhận BookingEvent.", "Khởi tạo Session qua VNPay API.", "Trả URL cho Client."],
        "alt_flows": [{"name": "Đang xử lý", "desc": "Báo lỗi Payment is already being processed."}],
        "frs": [{"id": "FR_26.1", "desc": "Phải gắn Idempotency Key để chống tạo trùng hóa đơn."}]
    },
    {
        "id": "UC-27", "name": "Xử lý Webhook Thanh toán", "actors": "Cổng thanh toán (VNPay)",
        "desc": "Hệ thống nhận kết quả từ bên thứ ba.",
        "precond": "Giao dịch đã được khách thực hiện.",
        "postcond": "Cập nhật trạng thái hóa đơn và báo cho BookingService.",
        "basic_flow": ["Nhận payload IPN từ VNPay.", "Xác thực chữ ký số.", "Phát PaymentCompletedEvent."],
        "alt_flows": [{"name": "Sai chữ ký", "desc": "Ghi log cảnh báo bảo mật và bỏ qua payload."}],
        "frs": [{"id": "FR_27.1", "desc": "Bắt buộc kiểm tra Checksum (Secure Hash) trước khi xử lý."}]
    },
    {
        "id": "UC-28", "name": "Tự động Hoàn tiền (Refund)", "actors": "Hệ thống",
        "desc": "Tự động hoàn tiền khi có lỗi hoặc khách hủy đúng luật.",
        "precond": "Giao dịch gốc đã thành công.",
        "postcond": "Tiền được hoàn lại tài khoản khách.",
        "basic_flow": ["Nhận RefundEvent.", "Gọi API Hoàn tiền của VNPay.", "Cập nhật trạng thái Refunded."],
        "alt_flows": [{"name": "Cổng VNPay lỗi", "desc": "Lưu vào Outbox để retry sau."}],
        "frs": [{"id": "FR_28.1", "desc": "Lưu trữ đầy đủ Transaction ID đối soát."}]
    },
    {
        "id": "UC-29", "name": "Xem số dư ví Host", "actors": "Chủ nhà (Host)",
        "desc": "Host xem doanh thu và số tiền có thể rút.",
        "precond": "Đã đăng nhập.",
        "postcond": "Trả về dữ liệu ví.",
        "basic_flow": ["Truy vấn bảng Balance.", "Hiển thị tổng thu nhập và Available Payout.", "Trả về Client."],
        "alt_flows": [],
        "frs": [{"id": "FR_29.1", "desc": "Chỉ cộng tiền vào ví sau khi khách Check-out thành công."}]
    },
    {
        "id": "UC-30", "name": "Cập nhật Thông tin Ngân hàng", "actors": "Chủ nhà (Host)",
        "desc": "Host thêm số tài khoản ngân hàng để nhận Payout.",
        "precond": "Đã đăng nhập.",
        "postcond": "Lưu thông tin thanh toán (đã mã hóa).",
        "basic_flow": ["Host nhập STK, Tên ngân hàng.", "Validate format.", "Lưu vào DB."],
        "alt_flows": [{"name": "Số tài khoản sai", "desc": "Báo lỗi Invalid Bank Account Format."}],
        "frs": [{"id": "FR_30.1", "desc": "Số tài khoản phải được mã hóa mã hóa chuẩn AES256."}]
    },
    {
        "id": "UC-31", "name": "Yêu cầu Rút tiền (Payout)", "actors": "Chủ nhà, Admin",
        "desc": "Host yêu cầu rút tiền và Admin duyệt chuyển khoản.",
        "precond": "Số dư lớn hơn mức tối thiểu.",
        "postcond": "Tiền bị trừ khỏi ví, lệnh chuyển khoản được tạo.",
        "basic_flow": ["Host tạo lệnh Payout.", "Admin xem và duyệt.", "Hệ thống MarkPayoutCompleted và trừ ví."],
        "alt_flows": [{"name": "Số dư không đủ", "desc": "Hệ thống từ chối tạo lệnh."}],
        "frs": [{"id": "FR_31.1", "desc": "Lệnh rút tiền phải sinh ra mã PayoutReference để đối soát."}]
    },
    {
        "id": "UC-32", "name": "Xem Lịch sử Giao dịch", "actors": "Chủ nhà (Host)",
        "desc": "Hiển thị lịch sử cộng/trừ tiền trong ví.",
        "precond": "Đã đăng nhập.",
        "postcond": "Trả về List Transaction.",
        "basic_flow": ["Host mở Transaction History.", "Query PaymentService DB.", "Hiển thị phân trang."],
        "alt_flows": [],
        "frs": [{"id": "FR_32.1", "desc": "Lịch sử phải không thể xóa hoặc sửa đổi (Immutable)."}]
    },

    # Nhóm 6: Tin nhắn & Review (33-38)
    {
        "id": "UC-33", "name": "Tạo cuộc hội thoại", "actors": "Khách, Chủ nhà",
        "desc": "Khách gửi yêu cầu chat với Host về một căn phòng.",
        "precond": "Khách đã đăng nhập.",
        "postcond": "Tạo Conversation_Id trong MongoDB/Postgres.",
        "basic_flow": ["Khách bấm Liên hệ Chủ nhà.", "Tạo Conversation map với PropertyId.", "Chuyển sang màn hình Chat."],
        "alt_flows": [{"name": "Chat với chính mình", "desc": "Host cannot start a conversation as a guest for their own property."}],
        "frs": [{"id": "FR_33.1", "desc": "Nếu Conversation giữa 2 user cho Property này đã có, trả về Id cũ."}]
    },
    {
        "id": "UC-34", "name": "Gửi tin nhắn & File", "actors": "Người dùng",
        "desc": "Gửi text hoặc ảnh qua WebSocket.",
        "precond": "Nằm trong Conversation.",
        "postcond": "Tin nhắn được lưu và push realtime qua SignalR/Socket.IO.",
        "basic_flow": ["Nhập tin nhắn.", "Lưu DB.", "Broadcast tới người kia."],
        "alt_flows": [{"name": "Không có quyền", "desc": "Báo lỗi You are not a participant in this conversation."}],
        "frs": [{"id": "FR_34.1", "desc": "Tin nhắn hệ thống (System message) không thể được gửi bởi user."}]
    },
    {
        "id": "UC-35", "name": "Đọc tin nhắn (Mark As Read)", "actors": "Người dùng",
        "desc": "Đánh dấu tin nhắn đã được xem.",
        "precond": "Có tin nhắn chưa đọc.",
        "postcond": "Huy hiệu Unread bị xóa.",
        "basic_flow": ["Người dùng mở hộp thoại.", "Gọi API MarkAsRead.", "Cập nhật DB."],
        "alt_flows": [],
        "frs": [{"id": "FR_35.1", "desc": "Phải cập nhật LastReadTimestamp."}]
    },
    {
        "id": "UC-36", "name": "Lấy Lịch sử Tin nhắn", "actors": "Người dùng",
        "desc": "Tải các tin nhắn cũ theo cơ chế Infinite Scroll.",
        "precond": "Đang trong Conversation.",
        "postcond": "Trả về phân trang.",
        "basic_flow": ["Cuộn lên trên.", "Gửi request LastMessageId.", "Lấy 20 tin nhắn tiếp theo."],
        "alt_flows": [],
        "frs": [{"id": "FR_36.1", "desc": "Sử dụng Keyset Pagination (Cursor) để tránh trôi tin nhắn."}]
    },
    {
        "id": "UC-37", "name": "Viết Đánh giá (Review)", "actors": "Khách (Guest)",
        "desc": "Khách đánh giá phòng sau khi ở.",
        "precond": "Booking ở trạng thái Completed.",
        "postcond": "Lưu Review và cập nhật Rating tổng của phòng.",
        "basic_flow": ["Nhập số sao và bình luận.", "Lưu Review.", "Phát Event cập nhật PropertyService."],
        "alt_flows": [{"name": "Chưa hoàn tất chuyến đi", "desc": "Báo lỗi Booking must be Confirmed or Completed to review."}],
        "frs": [{"id": "FR_37.1", "desc": "Mỗi Booking chỉ được phép viết 1 Review duy nhất."}]
    },
    {
        "id": "UC-38", "name": "Sửa/Xóa Đánh giá", "actors": "Khách (Guest)",
        "desc": "Sửa hoặc xóa review đã viết.",
        "precond": "Review do chính khách viết.",
        "postcond": "Rating tổng của phòng được tính toán lại.",
        "basic_flow": ["Chọn Sửa/Xóa.", "Cập nhật DB.", "Phát Event để tính lại Rating trung bình."],
        "alt_flows": [{"name": "Phòng bị xóa", "desc": "Báo lỗi Property not found."}],
        "frs": [{"id": "FR_38.1", "desc": "Khi xóa Review, hệ thống phải cập nhật lại sao trung bình của phòng."}]
    },

    # Nhóm 7: Admin (39-45)
    {
        "id": "UC-39", "name": "Đăng nhập Admin", "actors": "Admin",
        "desc": "Đăng nhập vào hệ thống quản trị.",
        "precond": "Tài khoản có Role = Admin/Moderator.",
        "postcond": "Cấp JWT Token quyền quản trị.",
        "basic_flow": ["Nhập credentials.", "Kiểm tra quyền.", "Cấp Token."],
        "alt_flows": [{"name": "Thiếu quyền", "desc": "Báo lỗi Access denied. Admin or Moderator role required."}],
        "frs": [{"id": "FR_39.1", "desc": "Mật khẩu Admin phải được mã hóa theo chuẩn bcrypt."}]
    },
    {
        "id": "UC-40", "name": "Duyệt Bất động sản", "actors": "Admin",
        "desc": "Duyệt tin đăng của Host trước khi hiển thị công khai.",
        "precond": "Phòng ở trạng thái Pending Approval.",
        "postcond": "Phòng được duyệt thành Published hoặc bị Reject.",
        "basic_flow": ["Admin xem tin.", "Bấm Approve.", "Cập nhật trạng thái và bắn event lên Kafka."],
        "alt_flows": [{"name": "Thiếu thông tin", "desc": "Admin chọn Reject kèm lý do."}],
        "frs": [{"id": "FR_40.1", "desc": "Sau khi Approve, dữ liệu phải xuất hiện trên Elasticsearch qua luồng CDC."}]
    },
    {
        "id": "UC-41", "name": "Khóa/Mở tài khoản", "actors": "Admin",
        "desc": "Khóa user vi phạm chính sách.",
        "precond": "User tồn tại.",
        "postcond": "Trạng thái user = Suspended, vô hiệu hóa mọi JWT đang có.",
        "basic_flow": ["Admin chọn User.", "Bấm Suspend.", "Cập nhật DB và phát Event."],
        "alt_flows": [{"name": "User không tồn tại", "desc": "Báo lỗi User not found."}],
        "frs": [{"id": "FR_41.1", "desc": "Hệ thống tự động hủy các phiên đăng nhập (RevokeSessions) của user bị khóa."}]
    },
    {
        "id": "UC-42", "name": "Quản lý Danh mục Tiện ích", "actors": "Admin",
        "desc": "Thêm, Sửa, Xóa các danh mục tiện ích cho toàn hệ thống.",
        "precond": "Quyền Admin.",
        "postcond": "Danh mục tiện ích được cập nhật.",
        "basic_flow": ["Admin nhập tên tiện ích mới.", "Upload icon.", "Lưu vào Amenity DB."],
        "alt_flows": [{"name": "Trùng tên", "desc": "Báo lỗi Amenity already exists."}],
        "frs": [{"id": "FR_42.1", "desc": "Không cho phép xóa tiện ích nếu đang có Property sử dụng nó."}]
    },
    {
        "id": "UC-43", "name": "Xử lý Khiếu nại (Disputes)", "actors": "Admin",
        "desc": "Admin tham gia giải quyết tranh chấp giữa Host và Guest.",
        "precond": "Booking có tranh chấp.",
        "postcond": "Tranh chấp được giải quyết (Refund hoặc phạt).",
        "basic_flow": ["Xem bằng chứng của 2 bên.", "Ra quyết định Refund.", "Đóng Dispute."],
        "alt_flows": [],
        "frs": [{"id": "FR_43.1", "desc": "Quyết định của Admin là cuối cùng và đóng băng Booking đó."}]
    },
    {
        "id": "UC-44", "name": "Xem Báo cáo Thống kê", "actors": "Admin",
        "desc": "Xem biểu đồ doanh thu và lượng người dùng tăng trưởng.",
        "precond": "Đã đăng nhập Admin.",
        "postcond": "Trả về dữ liệu mảng để vẽ Chart.",
        "basic_flow": ["Chọn khoảng thời gian.", "Truy vấn Analytics DB.", "Trả về JSON."],
        "alt_flows": [],
        "frs": [{"id": "FR_44.1", "desc": "Dữ liệu báo cáo phải được gom nhóm theo Ngày/Tháng/Năm (Group By)."}]
    },
    {
        "id": "UC-45", "name": "Cập nhật Cấu hình Nền tảng", "actors": "Admin",
        "desc": "Thay đổi phần trăm phí dịch vụ hoặc phí hoa hồng.",
        "precond": "Quyền Root Admin.",
        "postcond": "Cấu hình mới được áp dụng ngay lập tức cho các giao dịch sau.",
        "basic_flow": ["Admin nhập cấu hình mới.", "Lưu vào bảng PlatformSettings.", "Xóa Cache."],
        "alt_flows": [],
        "frs": [{"id": "FR_45.1", "desc": "Các giao dịch tạo trước thời điểm cập nhật vẫn giữ nguyên mức phí cũ."}]
    }
]

def main():
    latex_content = ""
    for uc in usecases:
        latex_content += create_latex_table(
            uc["id"], uc["name"], uc["actors"], uc["desc"],
            uc["precond"], uc["postcond"], uc["basic_flow"],
            uc.get("alt_flows", []), uc.get("frs", [])
        )
    
    # Ghi ra file
    output_path = os.path.join(os.path.dirname(__file__), "latex/chapters/02_1_usecases.tex")
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    with open(output_path, "w", encoding="utf-8") as f:
        f.write(latex_content)
        
    print(f"Đã tạo thành công {len(usecases)} Use Cases vào {output_path}")

if __name__ == "__main__":
    main()
