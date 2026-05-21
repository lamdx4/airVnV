import { useState } from 'react';
import type { UseFormRegister, UseFormSetValue, UseFormWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Info, Plus, Trash2 } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';

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
  const [newRule, setNewRule] = useState('');

  // Watch the dynamic custom rules list from react-hook-form
  const customRules: string[] = watch('customRules') || [];

  const handleAddRule = (e: React.MouseEvent | React.KeyboardEvent) => {
    e.preventDefault();
    const trimmed = newRule.trim();
    if (!trimmed) return;

    // Check for duplicates to prevent clean data issues
    if (customRules.includes(trimmed)) {
      setNewRule('');
      return;
    }

    const updated = [...customRules, trimmed];
    setValue('customRules', updated, { shouldValidate: true });
    setNewRule('');
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      handleAddRule(e);
    }
  };

  const handleRemoveRule = (indexToRemove: number) => {
    const updated = customRules.filter((_, idx) => idx !== indexToRemove);
    setValue('customRules', updated, { shouldValidate: true });
  };

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">{t('rules.title', 'House Rules')}</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">
          {t('rules.subtitle', 'Guests must agree to your house rules before booking.')}
        </p>
      </CardHeader>
      <CardContent className="px-6 py-6 space-y-8">
        {/* Standard Rules Configuration */}
        <div className="grid gap-3 sm:grid-cols-2">
          {[
            { id: 'allowPets', label: t('rules.pets', 'Pets allowed') },
            { id: 'allowSmoking', label: t('rules.smoking', 'Smoking allowed') },
            { id: 'allowEvents', label: t('rules.events', 'Events allowed') },
            { id: 'flexibleCheckOut', label: t('rules.flexible', 'Flexible checkout') },
          ].map((rule) => {
            const isChecked = watch(rule.id as any);
            return (
              <label 
                key={rule.id} 
                className={`flex items-center justify-between p-4 rounded-lg border transition-all cursor-pointer ${
                  isChecked ? 'border-pink-500 bg-pink-50/50' : 'border-slate-200 hover:bg-slate-50'
                }`}
              >
                <span className="text-sm font-semibold text-slate-900">{rule.label}</span>
                <Checkbox
                  checked={isChecked}
                  onCheckedChange={(checked) => setValue(rule.id as any, checked === true)}
                  className="h-5 w-5 rounded-md border-slate-300 data-[state=checked]:bg-[#FF5A5F] data-[state=checked]:border-[#FF5A5F]"
                />
              </label>
            );
          })}
        </div>
        
        {/* Check-in and Check-out Times */}
        <div className="grid grid-cols-2 gap-8 pt-6 border-t border-slate-100">
          <div className="space-y-2">
            <Label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">
              {t('rules.checkIn', 'Check-in time')}
            </Label>
            <Input 
              type="time" 
              {...register('checkInTime')} 
              className="h-11 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100" 
            />
          </div>
          <div className="space-y-2">
            <Label className="text-xs font-semibold text-slate-500 uppercase tracking-wider">
              {t('rules.checkOut', 'Check-out time')}
            </Label>
            <Input 
              type="time" 
              {...register('checkOutTime')} 
              className="h-11 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100" 
            />
          </div>
        </div>

        {/* Custom House Rules Dynamic Section */}
        <div className="pt-6 border-t border-slate-100 space-y-4">
          <div className="space-y-1">
            <Label className="text-sm font-bold text-slate-900">
              {t('rules.customTitle', 'Additional rules')}
            </Label>
            <p className="text-xs font-normal text-slate-500">
              {t('rules.customSubtitle', 'Add rules that guests must follow during their stay (e.g. Quiet hours).')}
            </p>
          </div>

          <div className="flex gap-2">
            <Input
              type="text"
              value={newRule}
              onChange={(e) => setNewRule(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder={t('rules.customPlaceholder', 'e.g. No shoes on carpet, Quiet after 10 PM...')}
              className="h-11 rounded-lg border-slate-300 focus:ring-3 focus:ring-pink-100"
            />
            <Button
              type="button"
              onClick={handleAddRule}
              className="h-11 px-4 rounded-lg bg-slate-900 hover:bg-slate-800 text-white font-medium flex items-center gap-1.5 shrink-0 transition-colors"
            >
              <Plus className="h-4 w-4" />
              {t('rules.customAdd', 'Add')}
            </Button>
          </div>

          {/* Render Animated Dynamic Custom Rules List */}
          <div className="space-y-2">
            <AnimatePresence initial={false}>
              {customRules.map((ruleText, idx) => (
                <motion.div
                  key={`${ruleText}-${idx}`}
                  initial={{ opacity: 0, height: 0, y: -10 }}
                  animate={{ opacity: 1, height: 'auto', y: 0 }}
                  exit={{ opacity: 0, height: 0, y: -10 }}
                  transition={{ duration: 0.2 }}
                  className="flex items-center justify-between p-3.5 bg-slate-50/70 border border-slate-200/80 rounded-lg group hover:border-slate-300 transition-colors overflow-hidden"
                >
                  <span className="text-sm font-medium text-slate-700 leading-relaxed pr-4">
                    {ruleText}
                  </span>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    onClick={() => handleRemoveRule(idx)}
                    className="h-8 w-8 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-full shrink-0 transition-colors"
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </motion.div>
              ))}
            </AnimatePresence>
          </div>
        </div>

        {/* Community Info box */}
        <div className="p-4 bg-blue-50 border border-blue-100 rounded-lg flex gap-3">
          <Info className="h-5 w-5 text-blue-600 shrink-0 mt-0.5" />
          <p className="text-xs font-medium text-blue-700 leading-relaxed">
            {t('rules.communityDisclaimer', "By publishing, you agree to comply with Airbnb's community standards and local hosting laws.")}
          </p>
        </div>
      </CardContent>
    </Card>
  );
}
