// features/properties/components/VietnamAddressSelector.tsx
import React, { useMemo } from "react";
import { useVietnamAddress } from "../hooks/useVietnamAddress";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { ProvinceResponse, WardResponse } from "../types/vietnam-address";
import { useTranslation } from "react-i18next";

interface VietnamAddressSelectorProps {
  selectedProvinceCode?: number;
  selectedWardCode?: number;
  onProvinceChange: (province: ProvinceResponse) => void;
  onWardChange: (ward: WardResponse) => void;
}

export const VietnamAddressSelector: React.FC<VietnamAddressSelectorProps> = ({
  selectedProvinceCode,
  selectedWardCode,
  onProvinceChange,
  onWardChange,
}) => {
  const { data: provinces, isLoading, error } = useVietnamAddress();
  const { t } = useTranslation();

  const selectedProvince = useMemo(
    () => provinces?.find((p) => p.code === selectedProvinceCode),
    [provinces, selectedProvinceCode]
  );

  const handleProvinceChange = (value: string) => {
    const code = parseInt(value, 10);
    const province = provinces?.find((p) => p.code === code);
    if (province) {
      onProvinceChange(province);
    }
  };

  const handleWardChange = (value: string) => {
    const code = parseInt(value, 10);
    const ward = selectedProvince?.wards?.find((w) => w.code === code);
    if (ward) {
      onWardChange(ward);
    }
  };

  if (error) {
    return <div className="text-sm text-destructive">Failed to load address data.</div>;
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col gap-2">
        <label className="text-sm font-medium">{t("location.province")}</label>
        <Select
          value={selectedProvinceCode?.toString() || ""}
          onValueChange={handleProvinceChange}
          disabled={isLoading}
        >
          <SelectTrigger className="w-full">
            <SelectValue placeholder={isLoading ? t("location.searching") : t("location.selectProvince")} />
          </SelectTrigger>
          <SelectContent>
            {provinces?.map((province) => (
              <SelectItem key={province.code} value={province.code.toString()}>
                {province.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="flex flex-col gap-2">
        <label className="text-sm font-medium">{t("location.ward")}</label>
        <Select
          value={selectedWardCode?.toString() || ""}
          onValueChange={handleWardChange}
          disabled={!selectedProvinceCode || isLoading}
        >
          <SelectTrigger className="w-full">
            <SelectValue placeholder={t("location.selectWard")} />
          </SelectTrigger>
          <SelectContent>
            {selectedProvince?.wards?.map((ward) => (
              <SelectItem key={ward.code} value={ward.code.toString()}>
                {ward.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
    </div>
  );
};
