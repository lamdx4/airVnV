import * as React from "react"

import { cn } from "@/lib/utils"

function Textarea({ className, ...props }: React.ComponentProps<"textarea">) {
  return (
    <textarea
      data-slot="textarea"
      className={cn(
        "flex field-sizing-content min-h-16 w-full rounded-[8px] border border-[#dddddd] bg-white px-3 py-3 text-base text-[#222222] transition-all outline-none placeholder:text-[#6a6a6a] focus-visible:border-[#222222] focus-visible:border-2 focus-visible:ring-0 disabled:cursor-not-allowed disabled:bg-[#f7f7f7] disabled:opacity-50 aria-invalid:border-[#c13515] aria-invalid:ring-3 aria-invalid:ring-[#c13515]/20 md:text-sm dark:bg-input/30 dark:disabled:bg-input/80 dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40",
        className
      )}
      {...props}
    />
  )
}

export { Textarea }
