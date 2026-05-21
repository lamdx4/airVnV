import type { UseFormRegister, UseFormSetValue, UseFormWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import { Info } from 'lucide-react';

interface HouseRulesSectionProps {
  register: UseFormRegister<any>;
  setValue: UseFormSetValue<any>;
  watch: UseFormWatch<any>;
}

export function HouseRulesSection({
  register,
  setValue,
  watch,
}: HouseRulesSectionProps) {
  const { t } = useTranslation();

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">{t('rules.title')}</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">{t('rules.subtitle')}</p>
      </CardHeader>
      <CardContent className="px-6 py-6 space-y-8">
        <div className="grid gap-3 sm:grid-cols-2">
          {[
            { id: 'allowPets', label: t('rules.pets') },
            { id: 'allowSmoking', label: t('rules.smoking') },
            { id: 'allowEvents', label: t('rules.events') },
            { id: 'flexibleCheckOut', label: t('rules.flexible') },
          ].map((rule) => {
            const isChecked = watch(rule.id as any);
            return (
              <label key={rule.id} className={`flex items-center justify-between p-4 rounded-lg border transition-all cursor-pointer ${
                isChecked ? 'border-pink-500 bg-pink-50' : 'border-slate-200 hover:bg-slate-50'
              }`}>
                <span className="text-sm font-semibold text-slate-900">{rule.label}</span>
                <Checkbox
                  checked={isChecked}
                  onCheckedChange={(checked) => setValue(rule.id as any, checked === true)}
                  className="h-5 w-5 rounded-md border-slate-300 data-[state=checked]:bg-[#FF5A5F] data-[state=checked]:border-[#FF5A5F]"
                />
              </label>
            )
          })}
        </div>
        
        <div className="grid grid-cols-2 gap-8 pt-6 border-t border-slate-100">
            <div className="space-y-2">
                <Label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('rules.checkIn')}</Label>
                <Input type="time" {...register('checkInTime')} className="h-11 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100" />
            </div>
            <div className="space-y-2">
                <Label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">{t('rules.checkOut')}</Label>
                <Input type="time" {...register('checkOutTime')} className="h-11 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100" />
            </div>
        </div>

        <div className="p-4 bg-blue-50 border border-blue-100 rounded-lg flex gap-3">
            <Info className="h-5 w-5 text-blue-600 shrink-0 mt-0.5" />
            <p className="text-xs font-medium text-blue-700 leading-relaxed">
                By publishing, you agree to comply with Airbnb's community standards and local hosting laws.
            </p>
        </div>
      </CardContent>
    </Card>
  );
}
