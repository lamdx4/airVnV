import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"
import { Slot } from "radix-ui"

import { cn } from "@/lib/utils"

const buttonVariants = cva(
  "group/button inline-flex shrink-0 items-center justify-center rounded-[8px] border border-transparent bg-clip-padding text-sm font-medium whitespace-nowrap transition-all outline-none select-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 active:not-aria-[haspopup]:translate-y-px disabled:pointer-events-none disabled:bg-[#ffd1da] disabled:text-white disabled:opacity-100 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4",
  {
    variants: {
      variant: {
        default: "bg-[#ff385c] text-white hover:bg-[#e00b41] active:bg-[#e00b41]",
        outline:
          "border-[#dddddd] bg-white text-[#222222] hover:bg-[#f7f7f7] hover:text-[#222222] aria-expanded:bg-[#f7f7f7] aria-expanded:text-[#222222] dark:border-input dark:bg-input/30 dark:hover:bg-input/50",
        secondary:
          "bg-[#f7f7f7] text-[#222222] hover:bg-[color-mix(in_oklch,#f7f7f7,#222222_5%)] aria-expanded:bg-[#f7f7f7] aria-expanded:text-[#222222]",
        ghost:
          "hover:bg-[#f7f7f7] hover:text-[#222222] aria-expanded:bg-[#f7f7f7] aria-expanded:text-[#222222] dark:hover:bg-muted/50",
        destructive:
          "bg-[#c13515]/10 text-[#c13515] hover:bg-[#c13515]/20 focus-visible:border-[#c13515]/40 focus-visible:ring-[#c13515]/20 dark:bg-destructive/20 dark:hover:bg-destructive/30 dark:focus-visible:ring-destructive/40",
        link: "text-[#ff385c] underline-offset-4 hover:underline",
      },
      size: {
        default:
          "h-12 gap-2 px-6 text-base font-medium has-data-[icon=inline-end]:pr-4 has-data-[icon=inline-start]:pl-4",
        xs: "h-6 gap-1 rounded-[4px] px-2 text-xs in-data-[slot=button-group]:rounded-[8px] has-data-[icon=inline-end]:pr-1.5 has-data-[icon=inline-start]:pl-1.5 [&_svg:not([class*='size-'])]:size-3",
        sm: "h-8 gap-1.5 rounded-[8px] px-3 text-sm has-data-[icon=inline-end]:pr-2 has-data-[icon=inline-start]:pl-2 [&_svg:not([class*='size-'])]:size-3.5",
        lg: "h-12 gap-2 px-6 text-base font-medium has-data-[icon=inline-end]:pr-4 has-data-[icon=inline-start]:pl-4",
        icon: "size-10 rounded-full",
        "icon-xs":
          "size-6 rounded-full [&_svg:not([class*='size-'])]:size-3",
        "icon-sm":
          "size-8 rounded-full",
        "icon-lg": "size-10 rounded-full",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  }
)

function Button({
  className,
  variant = "default",
  size = "default",
  asChild = false,
  ...props
}: React.ComponentProps<"button"> &
  VariantProps<typeof buttonVariants> & {
    asChild?: boolean
  }) {
  const Comp = asChild ? Slot.Root : "button"

  return (
    <Comp
      data-slot="button"
      data-variant={variant}
      data-size={size}
      className={cn(buttonVariants({ variant, size, className }))}
      {...props}
    />
  )
}

export { Button, buttonVariants }
