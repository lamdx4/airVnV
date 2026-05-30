import React, { useState } from 'react';
import { DayPicker } from 'react-day-picker';
import 'react-day-picker/dist/style.css';
import { format, parseISO } from 'date-fns';
import { 
  Calendar03Icon, 
  InformationCircleIcon, 
  Loading03Icon,
  Delete02Icon,
  Add01Icon,
  Tick02Icon
} from 'hugeicons-react';
import type { PropertyAvailability } from '../types';
import { useBlockDates, useRemoveAvailability } from '../hooks/useProperties';
import { toast } from 'sonner';
import { useTranslation } from 'react-i18next';

interface AvailabilityCalendarProps {
  propertyId: string;
  availabilities: PropertyAvailability[];
}

export const AvailabilityCalendar: React.FC<AvailabilityCalendarProps> = ({ propertyId, availabilities }) => {
  const { t } = useTranslation();
  const [selectedRange, setSelectedRange] = useState<any>(undefined);
  const [note, setNote] = useState('');
  
  const blockDatesMutation = useBlockDates();
  const removeAvailabilityMutation = useRemoveAvailability();

  const blockedDays = availabilities.map(a => ({
    from: parseISO(a.startDate),
    to: parseISO(a.endDate)
  }));

  const handleBlock = async () => {
    if (!selectedRange?.from || !selectedRange?.to) {
        toast.error(t('calendar.selectRange'));
        return;
    }

    try {
      await blockDatesMutation.mutateAsync({
        propertyId,
        data: {
          startDate: format(selectedRange.from, 'yyyy-MM-dd'),
          endDate: format(selectedRange.to, 'yyyy-MM-dd'),
          note: note
        }
      });
      setSelectedRange(undefined);
      setNote('');
      toast.success(t('calendar.blockSuccess'));
    } catch (err: any) {
      toast.error(err.response?.data?.message || t('calendar.blockFailed'));
    }
  };

  const handleRemove = async (id: string) => {
    try {
      await removeAvailabilityMutation.mutateAsync({ propertyId, availabilityId: id });
      toast.success(t('calendar.unblockSuccess'));
    } catch (err) {
      toast.error(t('calendar.unblockFailed'));
    }
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
      {/* Calendar Section */}
      <div className="lg:col-span-7 bg-white p-6 rounded-3xl border shadow-sm">
        <div className="flex items-center justify-between mb-6">
            <h3 className="text-lg font-bold text-hof flex items-center gap-2">
                <Calendar03Icon className="h-5 w-5 text-rausch" />
                {t('calendar.title')}
            </h3>
            <div className="flex items-center gap-4 text-xs font-semibold">
                <div className="flex items-center gap-2">
                    <div className="w-3 h-3 bg-rausch/10 border border-rausch/30 rounded-sm"></div>
                    <span className="text-slate-500">{t('calendar.blocked')}</span>
                </div>
                <div className="flex items-center gap-2">
                    <div className="w-3 h-3 bg-white border rounded-sm"></div>
                    <span className="text-slate-500">{t('calendar.available')}</span>
                </div>
            </div>
        </div>

        <div className="flex justify-center calendar-custom">
            <style>{`
                .rdp-day_selected { background-color: #FF385C !important; color: white !important; }
                .rdp-button:hover:not([disabled]):not(.rdp-day_selected) { background-color: #FFF1F2 !important; color: #FF385C !important; }
            `}</style>
            <DayPicker
                mode="range"
                selected={selectedRange}
                onSelect={setSelectedRange}
                disabled={[{ before: new Date() }, ...blockedDays]}
                modifiers={{ blocked: blockedDays }}
                modifiersClassNames={{ blocked: 'bg-rausch/10 text-rausch font-bold line-through' }}
                numberOfMonths={2}
                className="border-none"
            />
        </div>
      </div>

      {/* Controls & List */}
      <div className="lg:col-span-5 space-y-6">
        {/* Block Dates Form */}
        <div className="bg-slate-900 text-white p-6 rounded-3xl space-y-4 shadow-xl">
            <h4 className="font-bold flex items-center gap-2">
                <Add01Icon className="h-5 w-5 text-rausch" />
                {t('calendar.blockNew')}
            </h4>
            
            <div className="space-y-3">
                <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-1">
                        <label className="text-[10px] font-bold uppercase text-slate-400">{t('calendar.from')}</label>
                        <div className="bg-white/10 p-3 rounded-xl border border-white/10 text-sm">
                            {selectedRange?.from ? format(selectedRange.from, 'MMM dd, yyyy') : t('calendar.selectDate')}
                        </div>
                    </div>
                    <div className="space-y-1">
                        <label className="text-[10px] font-bold uppercase text-slate-400">{t('calendar.to')}</label>
                        <div className="bg-white/10 p-3 rounded-xl border border-white/10 text-sm">
                            {selectedRange?.to ? format(selectedRange.to, 'MMM dd, yyyy') : t('calendar.selectDate')}
                        </div>
                    </div>
                </div>

                <div className="space-y-1">
                    <label className="text-[10px] font-bold uppercase text-slate-400">{t('calendar.noteInternal')}</label>
                    <input 
                        value={note}
                        onChange={(e) => setNote(e.target.value)}
                        placeholder={t('calendar.notePlaceholder')}
                        className="w-full bg-white/10 p-3 rounded-xl border border-white/10 text-sm outline-none focus:border-rausch transition-all"
                    />
                </div>

                <button
                    onClick={handleBlock}
                    disabled={!selectedRange?.from || blockDatesMutation.isPending}
                    className="w-full bg-rausch hover:bg-rausch-dark h-12 rounded-xl font-bold transition-all disabled:bg-slate-700 disabled:text-slate-500 mt-2 flex items-center justify-center gap-2"
                >
                    {blockDatesMutation.isPending ? <Loading03Icon className="h-5 w-5 animate-spin" /> : <Tick02Icon className="h-5 w-5" />}
                    {t('calendar.blockSelection')}
                </button>
            </div>
        </div>

        {/* Blocked List */}
        <div className="space-y-4">
            <h4 className="text-sm font-bold text-hof flex items-center gap-2">
                {t('calendar.managedBlocks')}
                <span className="text-[10px] bg-slate-100 px-2 py-0.5 rounded-full text-slate-500">{availabilities.length}</span>
            </h4>
            
            <div className="space-y-2 max-h-[300px] overflow-y-auto pr-2 custom-scrollbar">
                {availabilities.map(item => (
                    <div key={item.id} className="flex items-center justify-between p-4 bg-white border rounded-2xl hover:border-rausch/30 transition-all group">
                        <div className="flex items-center gap-3">
                            <div className="p-2 bg-slate-50 rounded-xl group-hover:bg-rausch/5 text-slate-400 group-hover:text-rausch transition-colors">
                                <Calendar03Icon className="h-5 w-5" />
                            </div>
                            <div>
                                <p className="text-sm font-bold text-hof">
                                    {format(parseISO(item.startDate), 'MMM dd')} - {format(parseISO(item.endDate), 'MMM dd, yyyy')}
                                </p>
                                {item.note && <p className="text-[10px] text-slate-400 italic">"{item.note}"</p>}
                            </div>
                        </div>
                        <button 
                            onClick={() => handleRemove(item.id)}
                            className="p-2 text-slate-300 hover:text-rausch hover:bg-rausch/10 rounded-lg transition-all"
                        >
                            <Delete02Icon className="h-5 w-5" />
                        </button>
                    </div>
                ))}

                {availabilities.length === 0 && (
                    <div className="py-12 text-center border-2 border-dashed rounded-3xl border-slate-100 flex flex-col items-center gap-2">
                        <InformationCircleIcon className="h-8 w-8 text-slate-200" />
                        <p className="text-xs text-slate-400">{t('calendar.noBlockedYet')}</p>
                    </div>
                )}
            </div>
        </div>
      </div>
    </div>
  );
};
