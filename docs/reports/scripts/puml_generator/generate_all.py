import os
import sys

# Import các file data chứa cấu hình PlantUML
from data_01_10 import PUML_DATA_01_10
from data_11_20 import PUML_DATA_11_20
from data_21_30 import PUML_DATA_21_30
from data_31_45 import PUML_DATA_31_45

def generate_puml_files():
    # Gom tất cả các dictionary lại thành một dictionary lớn
    all_pumls = {}
    all_pumls.update(PUML_DATA_01_10)
    all_pumls.update(PUML_DATA_11_20)
    all_pumls.update(PUML_DATA_21_30)
    all_pumls.update(PUML_DATA_31_45)
    # Đã đủ 45 UC

    out_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", "latex", "figures", "outputs", "seq"))
    os.makedirs(out_dir, exist_ok=True)
    
    print(f"Đang ghi các file .puml ra thư mục: {out_dir}")
    
    count = 0
    for uc_id, puml_content in all_pumls.items():
        file_path = os.path.join(out_dir, f"{uc_id}.puml")
        with open(file_path, "w", encoding="utf-8") as f:
            f.write(puml_content)
        count += 1
        
    print(f"Hoàn tất! Đã ghi thành công {count} file .puml.")

if __name__ == "__main__":
    generate_puml_files()
