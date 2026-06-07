namespace Airbnb.AppHost.Extensions;

public static class ResourceBuilderExtensions
{
    /// <summary>
    /// Áp dụng các cấu hình mặc định bắt buộc cho tất cả microservices:
    /// - Sử dụng Workstation GC để tiết kiệm RAM.
    /// - Tự động nạp Linux OS CA bundle để tránh lỗi Aspire DCP ghi đè SSL_CERT_DIR.
    /// </summary>
    public static IResourceBuilder<T> WithDefaultServiceConfig<T>(this IResourceBuilder<T> builder) where T : IResourceWithEnvironment
    {
        // 1. Tối ưu RAM cho môi trường dev
        builder.WithEnvironment("DOTNET_gcServer", "0");

        // 2. Fix lỗi SSL_CERT_DIR bị ghi đè trên Linux (Aspire DCP bug)
        if (OperatingSystem.IsLinux())
        {
            var certPaths = new[] {
                "/etc/pki/tls/certs/ca-bundle.crt",      // Fedora/RHEL/CentOS
                "/etc/ssl/certs/ca-certificates.crt",    // Ubuntu/Debian/Alpine
                "/etc/ssl/ca-bundle.pem",                // SUSE
                "/etc/pki/ca-trust/extracted/pem/tls-ca-bundle.pem" // Một số distro khác
            };
            var validCertPath = certPaths.FirstOrDefault(File.Exists);
            
            if (validCertPath != null)
            {
                builder.WithEnvironment(context =>
                {
                    // Chỉ inject chứng chỉ hệ thống khi chạy local (manifest/deploy thì bỏ qua)
                    if (context.ExecutionContext.IsRunMode)
                    {
                        context.EnvironmentVariables["SSL_CERT_FILE"] = validCertPath;
                    }
                });
            }
        }
        
        return builder;
    }
}
