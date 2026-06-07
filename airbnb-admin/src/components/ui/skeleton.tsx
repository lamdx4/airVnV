import { cn } from "@/lib/utils"

function Skeleton({ className, ...props }: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="skeleton"
      className={cn("animate-pulse rounded-[8px] bg-[#f7f7f7]", className)}
      {...props}
    />
  )
}

export { Skeleton }
