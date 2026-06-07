"use client";

import { use } from "react";
import { UserDetail } from "@/features/users/components/user-detail";

export default function UserDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  return <UserDetail userId={id} />;
}
