import {
  Building2,
  Users,
  BarChart3,
  CreditCard,
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
  { label: "Properties", href: "/properties", icon: Building2 },
  { label: "Users", href: "/users", icon: Users },
  { label: "Payments", href: "/payments", icon: CreditCard },
  { label: "Reports", href: "/reports", icon: BarChart3 },
  { label: "Settings", href: "/settings", icon: Settings },
];
