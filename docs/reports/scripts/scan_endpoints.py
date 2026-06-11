import os
import glob
import json
import re

def scan_services(src_dir):
    services = {}
    
    # Tìm tất cả các dự án Microservices
    service_dirs = [d for d in os.listdir(src_dir) if os.path.isdir(os.path.join(src_dir, d)) and d.startswith("Airbnb.")]
    
    for service in service_dirs:
        service_path = os.path.join(src_dir, service)
        features_path = os.path.join(service_path, "Features")
        
        if not os.path.exists(features_path):
            continue
            
        services[service] = []
        
        # Quét tất cả các file Endpoint.cs đệ quy
        endpoints = glob.glob(os.path.join(features_path, "**", "Endpoint.cs"), recursive=True)
        
        for ep in endpoints:
            feature_dir = os.path.dirname(ep)
            feature_name = os.path.basename(feature_dir)
            
            # Đọc Handler để phân tích ngoại lệ (Alternative flows)
            handler_path = os.path.join(feature_dir, "Handler.cs")
            exceptions = []
            
            if os.path.exists(handler_path):
                with open(handler_path, 'r', encoding='utf-8') as f:
                    content = f.read()
                    # Tìm các dòng throw BusinessException hoặc NotFoundException
                    matches = re.findall(r'throw new\s+([A-Za-z]+Exception)\s*\(\s*"([^"]+)"', content)
                    for ex_type, msg in matches:
                        exceptions.append(f"{ex_type}: {msg}")
            
            services[service].append({
                "FeatureName": feature_name,
                "Path": feature_dir.replace(src_dir, ""),
                "Exceptions": exceptions
            })
            
    return services

if __name__ == "__main__":
    src_path = "/home/lamdx4/Projects/Airbnb/src"
    result = scan_services(src_path)
    
    output_path = os.path.join(os.path.dirname(__file__), "scanned_endpoints.json")
    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(result, f, indent=4, ensure_ascii=False)
    
    # In ra summary để dễ quan sát
    for svc, features in result.items():
        print(f"[{svc}] - {len(features)} endpoints")
        for feat in features:
            print(f"  - {feat['FeatureName']}")
            if feat['Exceptions']:
                for ex in feat['Exceptions']:
                    print(f"    * {ex}")
