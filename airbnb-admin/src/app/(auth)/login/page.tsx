"use client";

import { Suspense, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Eye, EyeOff, Loader2 } from "lucide-react";
import { z } from "zod";
import Link from "next/link";

import { authApi } from "@/lib/auth";
import { useAuthStore } from "@/store";
import { ROUTES } from "@/config/constants";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";

const loginSchema = z.object({
  email: z.email("Please enter a valid email address"),
  password: z.string().min(6, "Password must be at least 6 characters"),
});

type LoginForm = z.infer<typeof loginSchema>;

function LoginForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const setAuth = useAuthStore((s) => s.setAuth);
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const redirectTo = searchParams.get("redirect") ?? ROUTES.DASHBOARD;

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({
    defaultValues: { email: "", password: "" },
    mode: "onChange",
  });

  async function onSubmit(data: LoginForm) {
    setIsLoading(true);
    try {
      const response = await authApi.login(data);
      const { accessToken, refreshToken, fullName, email, role } = response.data;
      const user = {
        id: email,
        email,
        fullName,
        role,
      };
      setAuth(user, accessToken, refreshToken);
      toast.success("Welcome back!");
      router.push(redirectTo);
    } catch (error: unknown) {
      const message =
        error && typeof error === "object" && "response" in error
          ? (error as { response?: { data?: { message?: string } } }).response?.data?.message
          : "Invalid email or password. Please try again.";
      toast.error(message ?? "Invalid email or password. Please try again.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <Card className="border-[#dddddd] shadow-[rgba(0,0,0,0.02)_0_0_0_1px,rgba(0,0,0,0.04)_0_2px_6px,rgba(0,0,0,0.1)_0_4px_8px]">
      <CardHeader className="text-center pb-2">
        <div className="mx-auto mb-2">
          <svg viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg" aria-hidden="true" role="presentation" focusable="false" className="h-12 w-12">
            <path d="M16 1c2.008 0 3.463.963 4.751 3.269l.533 1.022c1.158 2.244 2.944 4.67 5.224 7.263l.654.727.835.916c2.085 2.3 3.179 4.186 3.332 5.72l.012.214.006.288c0 4.062-2.877 6.831-7.027 6.831-1.586 0-3.163-.465-4.638-1.322l-.227-.14-.277-.178-.246-.165-.136-.097-.276-.203-.166-.13c-.198-.156-.327-.272-.393-.348l-.035-.042-.035.042c-.066.076-.195.192-.393.348l-.166.13-.276.203-.136.097-.246.165-.277.178-.227.14c-1.475.857-3.052 1.322-4.638 1.322-4.15 0-7.027-2.769-7.027-6.831 0-1.694.864-3.536 2.477-5.59l.378-.465.835-.916.654-.727c2.28-2.593 4.066-5.02 5.224-7.263l.533-1.022C12.537 1.963 13.992 1 16 1z" fill="#ff385c"/>
          </svg>
        </div>
        <CardTitle className="text-[22px] font-semibold text-[#222222]">Admin Login</CardTitle>
        <CardDescription className="text-[#6a6a6a]">
          Sign in to manage the Airbnb platform
        </CardDescription>
      </CardHeader>
      <CardContent className="pt-4">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              type="email"
              placeholder="admin@airbnb.com"
              {...register("email", {
                validate: (value) => {
                  const result = loginSchema.shape.email.safeParse(value);
                  return result.success || result.error?.issues[0]?.message;
                },
              })}
              aria-invalid={!!errors.email}
            />
            {errors.email && (
              <p className="text-xs text-[#c13515]">{errors.email.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="password">Password</Label>
            <div className="relative">
              <Input
                id="password"
                type={showPassword ? "text" : "password"}
                placeholder="Enter your password"
                {...register("password", {
                  validate: (value) => {
                    const result = loginSchema.shape.password.safeParse(value);
                    return result.success || result.error?.issues[0]?.message;
                  },
                })}
                aria-invalid={!!errors.password}
                className="pr-10"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-[#6a6a6a] hover:text-[#222222] transition-colors"
                tabIndex={-1}
              >
                {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>
            {errors.password && (
              <p className="text-xs text-[#c13515]">{errors.password.message}</p>
            )}
          </div>

          <Button type="submit" className="w-full h-12 rounded-[8px] text-base font-medium" disabled={isLoading}>
            {isLoading ? (
              <>
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                Signing in...
              </>
            ) : (
              "Sign in"
            )}
          </Button>

          <div className="text-center">
            <Link
              href="/forgot-password"
              className="text-sm text-[#6a6a6a] hover:text-[#222222] transition-colors"
            >
              Forgot your password?
            </Link>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}

export default function LoginPage() {
  return (
    <Suspense>
      <LoginForm />
    </Suspense>
  );
}
