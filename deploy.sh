#!/bin/bash

# Đảm bảo script dừng ngay lập tức nếu có lỗi xảy ra
set -e

# Hiển thị hướng dẫn nếu không truyền tham số
if [ -z "$1" ]; then
    echo "Sử dụng: ./deploy.sh [command]"
    echo ""
    echo "Các commands có sẵn:"
    echo "  local          : Chạy AppHost ở môi trường Development (Local)"
    echo "  gen-aspirate   : Gen file docker-compose bằng công cụ Aspirate (Bỏ qua build để né lỗi SSL/Registry)"
    echo "  gen-aspire9    : Gen file docker-compose bằng tính năng native của Aspire 9 (Có bao gồm fix lỗi SSL UntrustedRoot)"
    echo ""
    exit 1
fi

COMMAND=$1

case $COMMAND in
    "local")
        echo "🚀 Đang khởi động môi trường Development Local..."
        dotnet run --project Airbnb.AppHost/Airbnb.AppHost.csproj
        ;;
        
    "gen-aspirate")
        echo "⚙️ Đang generate docker-compose bằng Aspirate (môi trường Production)..."
        export DOTNET_ROLL_FORWARD=Major
        export ASPIRATE_SECRET_PASSWORD="AirVnV2026!"
        cd Airbnb.AppHost && DOTNET_ENVIRONMENT=Production aspirate generate --output-format compose --non-interactive --skip-build --include-dashboard false
        echo "✅ Generate hoàn tất! File nằm trong thư mục Airbnb.AppHost/aspirate-output/"
        ;;
        
    "gen-aspire9")
        echo "⚙️ Đang generate docker-compose bằng Aspire 9 Publisher (môi trường Production)..."

        # Fix 1: Ép .NET dùng system TLS handler (không phải custom handler bỏ qua cert)
        export DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=1

        # Fix 2: Trỏ SSL_CERT_FILE đến cacert.pem đã có Microsoft TLS RSA Root G2
        # (cacert.pem = Mozilla bundle + MS TLS Root G2 được append thủ công)
        export SSL_CERT_FILE="$(pwd)/cacert.pem"
        export SSL_CERT_DIR=/etc/pki/tls/certs

        echo "🔐 SSL_CERT_FILE=$SSL_CERT_FILE"

        # Chạy Aspire 9 native publisher
        # --no-launch-profile: ngăn launchSettings.json override DOTNET_ENVIRONMENT về Development
        DOTNET_ENVIRONMENT=Production dotnet run \
            --project Airbnb.AppHost/Airbnb.AppHost.csproj \
            --no-launch-profile \
            --publisher docker-compose \
            --output-path ./compose
        
        echo "✅ Generate hoàn tất! File nằm trong thư mục compose/"
        ;;
        
    *)
        echo "❌ Lệnh không hợp lệ: $COMMAND"
        echo "Vui lòng chạy './deploy.sh' để xem danh sách lệnh."
        exit 1
        ;;
esac
