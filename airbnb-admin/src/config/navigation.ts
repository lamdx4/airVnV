import {
  Building2,
  Users,
  BarChart3,
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
  { label: "Reports", href: "/reports", icon: BarChart3 },
];
