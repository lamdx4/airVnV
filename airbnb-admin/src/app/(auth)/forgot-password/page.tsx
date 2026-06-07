"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { ArrowLeft, Loader2, Mail } from "lucide-react";
import Link from "next/link";
import { z } from "zod";

import { api } from "@/lib/api";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";

const forgotSchema = z.object({
  email: z.email("Please enter a valid email address"),
});

type ForgotForm = z.infer<typeof forgotSchema>;

export default function ForgotPasswordPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [isSent, setIsSent] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ForgotForm>({
    defaultValues: { email: "" },
    mode: "onChange",
  });

  async function onSubmit(data: ForgotForm) {
    setIsLoading(true);
    try {
      await api.post("/users/forgot-password", data);
      setIsSent(true);
      toast.success("Reset link sent to your email");
    } catch {
      toast.error("Failed to send reset link. Please try again.");
    } finally {
      setIsLoading(false);
    }
  }

  if (isSent) {
    return (
      <Card className="border-[#dddddd] shadow-[rgba(0,0,0,0.02)_0_0_0_1px,rgba(0,0,0,0.04)_0_2px_6px,rgba(0,0,0,0.1)_0_4px_8px]">
        <CardHeader className="text-center">
          <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-[#ff385c]/10">
            <Mail className="h-6 w-6 text-[#ff385c]" />
          </div>
          <CardTitle className="text-xl font-semibold text-[#222222]">Check your email</CardTitle>
          <CardDescription className="text-[#6a6a6a]">
            We sent a password reset link to your email address.
          </CardDescription>
        </CardHeader>
        <CardContent className="text-center">
          <Link href="/login">
            <Button variant="outline" className="mt-2">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to login
            </Button>
          </Link>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="border-[#dddddd] shadow-[rgba(0,0,0,0.02)_0_0_0_1px,rgba(0,0,0,0.04)_0_2px_6px,rgba(0,0,0,0.1)_0_4px_8px]">
      <CardHeader className="text-center">
        <CardTitle className="text-[22px] font-semibold text-[#222222]">Forgot Password</CardTitle>
        <CardDescription className="text-[#6a6a6a]">
          Enter your email and we&apos;ll send you a reset link
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              type="email"
              placeholder="admin@airbnb.com"
              {...register("email", {
                validate: (value) => {
                  const result = forgotSchema.shape.email.safeParse(value);
                  return result.success || result.error?.issues[0]?.message;
                },
              })}
              aria-invalid={!!errors.email}
            />
            {errors.email && (
              <p className="text-xs text-[#c13515]">{errors.email.message}</p>
            )}
          </div>

          <Button type="submit" className="w-full h-12 rounded-[8px] text-base font-medium" disabled={isLoading}>
            {isLoading ? (
              <>
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                Sending...
              </>
            ) : (
              "Send Reset Link"
            )}
          </Button>

          <div className="text-center">
            <Link
              href="/login"
              className="text-sm text-[#6a6a6a] hover:text-[#222222] transition-colors"
            >
              <ArrowLeft className="h-3 w-3 inline mr-1" />
              Back to login
            </Link>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
