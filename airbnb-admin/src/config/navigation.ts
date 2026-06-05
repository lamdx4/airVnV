import {
  LayoutDashboard,
  Building2,
  CalendarCheck,
  Users,
  CreditCard,
  Banknote,
  Star,
  BarChart3,
  Settings,
} from "lucide-react";

export interface NavItem {
  label: string;
  href: string;
  icon: React.ComponentType<{ className?: string }>;
  badge?: string;
  children?: NavItem[];
}

export const sidebarNav: NavItem[] = [
  { label: "Dashboard", href: "/dashboard", icon: LayoutDashboard },
  { label: "Properties", href: "/properties", icon: Building2 },
  { label: "Bookings", href: "/bookings", icon: CalendarCheck },
  { label: "Users", href: "/users", icon: Users },
  {
    label: "Payments",
    href: "/payments",
    icon: CreditCard,
    children: [
      { label: "Transactions", href: "/payments", icon: CreditCard },
      { label: "Payouts", href: "/payments/payouts", icon: Banknote },
    ],
  },
  { label: "Reviews", href: "/reviews", icon: Star },
  { label: "Reports", href: "/reports", icon: BarChart3 },
  { label: "Settings", href: "/settings", icon: Settings },
];
