#!/usr/bin/env bash
# ============================================================
#  Auto-compile LaTeX with inotifywait (inotify-tools)
#  Usage: ./comp.sh
#
#  Cài đặt (Fedora – minimal, ~400MB thay vì 9GB):
#    sudo dnf install inotify-tools biber \
#      texlive-xetex texlive-xetex-bin \
#      texlive-fontspec texlive-polyglossia \
#      texlive-geometry texlive-fancyhdr texlive-titlesec \
#      texlive-setspace texlive-indentfirst \
#      texlive-caption texlive-subcaption \
#      texlive-booktabs texlive-multirow texlive-longtable \
#      texlive-float texlive-listings texlive-xcolor \
#      texlive-amsmath texlive-hyperref texlive-bookmark \
#      texlive-biblatex texlive-enumitem texlive-microtype \
#      texlive-tocloft texlive-tocbibind texlive-chngcntr \
#      texlive-collection-fontsrecommended
# ============================================================

set -euo pipefail

MAIN="main.tex"
OUT_DIR="build"
WATCH_DIRS=("." "chapters" "appendix")

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
NC='\033[0m'

# ── Kiểm tra dependency ──────────────────────────────────────
check_deps() {
  local missing=()
  command -v inotifywait &>/dev/null || missing+=("inotify-tools")
  command -v xelatex     &>/dev/null || missing+=("texlive-xetex")
  command -v biber       &>/dev/null || missing+=("biber")

  if [[ ${#missing[@]} -gt 0 ]]; then
    echo -e "${RED}[ERROR] Thiếu: ${missing[*]}${NC}"
    echo -e "  sudo dnf install ${missing[*]}"
    exit 1
  fi
}

# ── Compile ──────────────────────────────────────────────────
compile() {
  mkdir -p "$OUT_DIR"

  echo -e "\n${CYAN}[$(date '+%H:%M:%S')] Đang compile...${NC}"

  # Pass 1
  xelatex -interaction=nonstopmode \
          -output-directory="$OUT_DIR" \
          "$MAIN" > "$OUT_DIR/compile.log" 2>&1 || true

  # Biber (bibliography)
  biber --input-directory="$OUT_DIR" \
        --output-directory="$OUT_DIR" \
        "${MAIN%.tex}" >> "$OUT_DIR/compile.log" 2>&1 || true

  # Pass 2 + 3 (resolve refs)
  xelatex -interaction=nonstopmode \
          -output-directory="$OUT_DIR" \
          "$MAIN" >> "$OUT_DIR/compile.log" 2>&1 || true

  xelatex -interaction=nonstopmode \
          -output-directory="$OUT_DIR" \
          "$MAIN" >> "$OUT_DIR/compile.log" 2>&1 || true

  # Kiểm tra kết quả
  if grep -q "^! " "$OUT_DIR/compile.log"; then
    echo -e "${RED}[FAIL] Có lỗi LaTeX:${NC}"
    grep -A 2 "^! " "$OUT_DIR/compile.log" | head -20
  else
    local warnings
    warnings=$(grep -c "Warning" "$OUT_DIR/compile.log" 2>/dev/null || true)
    echo -e "${GREEN}[OK] Compile xong → ${OUT_DIR}/main.pdf${NC} (${warnings} warning)"
  fi
}

# ── Main ─────────────────────────────────────────────────────
cd "$(dirname "$0")"

check_deps

echo -e "${YELLOW}╔══════════════════════════════════════════╗${NC}"
echo -e "${YELLOW}║   LaTeX Auto-Compiler  (Ctrl+C để dừng) ║${NC}"
echo -e "${YELLOW}╚══════════════════════════════════════════╝${NC}"
echo -e "  Watch: ${WATCH_DIRS[*]}"
echo -e "  Out  : ${OUT_DIR}/main.pdf"

# Compile lần đầu ngay khi chạy
compile

# Watch & auto-compile
while true; do
  inotifywait \
    --quiet \
    --event modify,create,delete \
    --recursive \
    "${WATCH_DIRS[@]}" \
    --include '\.(tex|bib|cls|sty)$' \
    2>/dev/null

  compile
done
