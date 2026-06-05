import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"
import { Slot } from "radix-ui"

import { cn } from "@/lib/utils"

const badgeVariants = cva(
  "group/badge inline-flex h-5 w-fit shrink-0 items-center justify-center gap-1 overflow-hidden rounded-full border border-transparent px-2.5 py-0.5 text-xs font-semibold whitespace-nowrap transition-all focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 has-data-[icon=inline-end]:pr-1.5 has-data-[icon=inline-start]:pl-1.5 aria-invalid:border-destructive aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 [&>svg]:pointer-events-none [&>svg]:size-3!",
  {
    variants: {
      variant: {
        default: "bg-[#ff385c] text-white [a]:hover:bg-[#e00b41]",
        secondary:
          "bg-[#f7f7f7] text-[#222222] [a]:hover:bg-[#f2f2f2]",
        destructive:
          "bg-[#c13515]/10 text-[#c13515] focus-visible:ring-[#c13515]/20 dark:bg-destructive/20 dark:focus-visible:ring-destructive/40 [a]:hover:bg-[#c13515]/20",
        outline:
          "border-[#dddddd] text-[#222222] bg-white [a]:hover:bg-[#f7f7f7] [a]:hover:text-[#6a6a6a]",
        ghost:
          "hover:bg-[#f7f7f7] hover:text-[#6a6a6a] dark:hover:bg-muted/50",
        link: "text-[#ff385c] underline-offset-4 hover:underline",
        success: "bg-emerald-50 text-emerald-700",
        warning: "bg-amber-50 text-amber-700",
        info: "bg-blue-50 text-blue-700",
      },
    },
    defaultVariants: {
      variant: "default",
    },
  }
)

function Badge({
  className,
  variant = "default",
  asChild = false,
  ...props
}: React.ComponentProps<"span"> &
  VariantProps<typeof badgeVariants> & { asChild?: boolean }) {
  const Comp = asChild ? Slot.Root : "span"

  return (
    <Comp
      data-slot="badge"
      data-variant={variant}
      className={cn(badgeVariants({ variant }), className)}
      {...props}
    />
  )
}

export { Badge, badgeVariants }
