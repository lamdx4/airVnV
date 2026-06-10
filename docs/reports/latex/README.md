# Hướng dẫn Build Báo cáo LaTeX

## Yêu cầu
- **TeX distribution**: TeX Live 2023+ hoặc MiKTeX
- **Engine**: XeLaTeX (bắt buộc – tiếng Việt Unicode)
- **Bibliography**: Biber

## Cài đặt (Ubuntu/Debian)
```bash
sudo apt install texlive-full biber
```

## Build
```bash
# Từ thư mục latex/
xelatex main.tex
biber main
xelatex main.tex
xelatex main.tex
```

## Hoặc dùng latexmk (khuyên dùng)
```bash
latexmk -xelatex -bibtex main.tex
```

## Cấu trúc thư mục
```
latex/
├── main.tex                          ← File gốc (entry point)
├── chapters/
│   ├── 01_tong_quan.tex              ← Chương I
│   ├── 02_thiet_ke.tex               ← Chương II
│   ├── 03_trien_khai.tex             ← Chương III
│   └── 04_ket_luan.tex               ← Chương IV
├── appendix/
│   ├── phan_cong_cong_viec.tex       ← Bảng phân công
│   └── tai_lieu_tham_khao.bib        ← Tài liệu tham khảo
├── figures/                          ← Ảnh, sơ đồ (đặt vào đây)
│   └── .gitkeep
└── README.md
```

## Thêm hình ảnh
Đặt file ảnh vào `figures/` rồi dùng:
```latex
\begin{figure}[H]
  \centering
  \includegraphics[width=0.9\textwidth]{figures/ten_file.png}
  \caption{Mô tả hình}
  \label{fig:nhan_hinh}
\end{figure}
```

## Placeholder conventions
- `\placeholder{text}` → Nội dung cần điền (in nghiêng, màu xám)
- `\todo{text}` → Việc cần làm (in đậm, màu đỏ)
- Comment `% REQUIREMENT:` → Gợi ý nội dung cần viết cho mỗi section
