import * as React from "react"

import { cn } from "@/lib/utils"

function Input({ className, type, ...props }: React.ComponentProps<"input">) {
  return (
    <input
      type={type}
      data-slot="input"
      className={cn(
        "h-12 w-full min-w-0 rounded-[8px] border border-[#dddddd] bg-white px-3 py-[14px] text-base text-[#222222] transition-all outline-none file:inline-flex file:h-6 file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-[#222222] placeholder:text-[#6a6a6a] focus-visible:border-[#222222] focus-visible:border-2 focus-visible:ring-0 disabled:pointer-events-none disabled:cursor-not-allowed disabled:bg-[#f7f7f7] disabled:opacity-50 aria-invalid:border-[#c13515] aria-invalid:ring-3 aria-invalid:ring-[#c13515]/20 md:text-sm dark:bg-input/30 dark:disabled:bg-input/80 dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40",
        className
      )}
      {...props}
    />
  )
}

export { Input }
