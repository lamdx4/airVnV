import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search01Icon } from 'hugeicons-react';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { Calendar } from '@/components/ui/calendar';
import { useQuery } from '@tanstack/react-query';
import { fetchProvinces } from '@/features/search/api/locations';
import { useSearchConsoleStore } from '@/store/searchConsoleStore';

export function SearchConsole({ onClose }: { onClose: () => void }) {
  const navigate = useNavigate();
  const [openWhere, setOpenWhere] = useState(false);
  
  const { location, setLocation, guestCount, dateRange } = useSearchConsoleStore();

  const { data: provinces, isLoading } = useQuery({
    queryKey: ['provinces'],
    queryFn: fetchProvinces,
    staleTime: Infinity, // Cache forever
  });

  const handleSearch = (e: React.MouseEvent) => {
    e.stopPropagation();
    onClose();
    
    const params = new URLSearchParams();
    if (location) params.append('province', location.code.toString());
    if (guestCount > 0) {
      params.append('guests', guestCount.toString());
    }
    
    navigate(`/search?${params.toString()}`);
  };

  return (
    <div className="bg-slate-100 rounded-full flex items-center border border-slate-200 shadow-lg mx-auto w-full max-w-[850px] relative mt-2">
      {/* Where Block */}
      <Popover open={openWhere} onOpenChange={setOpenWhere}>
        <PopoverTrigger asChild>
          <div 
            className={`flex-1 rounded-full px-8 py-3.5 cursor-pointer transition-colors group ${openWhere ? 'bg-white shadow-md' : 'hover:bg-white'}`}
            onClick={(e) => { e.stopPropagation(); setOpenWhere(true); }}
          >
            <div className="text-xs font-bold text-slate-800 tracking-wide mb-0.5">Where</div>
            <div className={`text-[15px] truncate ${location ? 'text-slate-900 font-medium' : 'text-slate-500'}`}>
              {location ? location.name : 'Search destinations'}
            </div>
          </div>
        </PopoverTrigger>
        <PopoverContent className="w-[400px] p-0 rounded-3xl mt-4 border-none shadow-2xl" align="start">
          <Command className="rounded-3xl border-0">
            <CommandInput placeholder="Search by region or city..." className="h-12 border-0" />
            <CommandList className="max-h-[300px] p-2">
              <CommandEmpty>No destinations found.</CommandEmpty>
              
              <CommandGroup heading="Options">
                <CommandItem
                  value="Nearby"
                  onSelect={() => {
                    setLocation({ name: 'Nearby', code: 'nearby', type: 'province' });
                    setOpenWhere(false);
                  }}
                  className="cursor-pointer py-3 px-4 rounded-xl hover:bg-slate-100 flex items-center gap-3"
                >
                  <div className="bg-slate-100 p-2 rounded-lg">
                    <Search01Icon className="h-4 w-4 text-[#FF5A5F]" />
                  </div>
                  <span className="font-bold text-slate-800">Nearby</span>
                </CommandItem>
              </CommandGroup>

              <CommandGroup heading="Destinations in Vietnam">
                {isLoading ? (
                  <div className="p-4 text-center text-sm text-slate-500">Loading...</div>
                ) : (
                  provinces?.map((province) => (
                    <CommandItem
                      key={province.code}
                      value={`${province.name} ${province.codename}`}
                      onSelect={() => {
                        setLocation({ name: province.name, code: province.code, type: 'province' });
                        setOpenWhere(false);
                      }}
                      className="cursor-pointer py-3 px-4 rounded-xl hover:bg-slate-100 flex items-center gap-3"
                    >
                      <div className="bg-slate-100 p-2 rounded-lg">
                        <Search01Icon className="h-4 w-4 text-slate-600" />
                      </div>
                      <span className="font-medium text-slate-700">{province.name}</span>
                    </CommandItem>
                  ))
                )}
              </CommandGroup>
            </CommandList>
          </Command>
        </PopoverContent>
      </Popover>

      <div className="w-[1px] h-10 bg-slate-200" />
      
    
      <div className="w-[1px] h-10 bg-slate-200" />
      
      {/* Who */}
      <Popover>
        <PopoverTrigger asChild>
          <div className="flex-1 hover:bg-white rounded-full pl-8 pr-2 py-2 cursor-pointer transition-colors flex items-center justify-between group">
            <div>
              <div className="text-xs font-bold text-slate-800 tracking-wide mb-0.5">Who</div>
              <div className={`text-[15px] ${guestCount > 0 ? 'text-slate-900 font-medium' : 'text-slate-500'}`}>
                {guestCount > 0 ? `${guestCount} guest${guestCount > 1 ? 's' : ''}` : 'Add guests'}
              </div>
            </div>
            <button 
              onClick={handleSearch}
              className="bg-[#FF5A5F] hover:bg-[#E31C5F] text-white px-6 py-3.5 rounded-full font-semibold flex items-center gap-2 transition-colors active:scale-95 z-10 relative"
            >
              <Search01Icon className="h-5 w-5 stroke-[2.5]" />
              <span>Search</span>
            </button>
          </div>
        </PopoverTrigger>
        <PopoverContent className="w-[350px] p-6 rounded-3xl mt-4 border-none shadow-2xl" align="end">
          <div className="flex items-center justify-between">
            <div>
              <div className="font-semibold text-slate-900">Guests</div>
              <div className="text-sm text-slate-500">How many guests?</div>
            </div>
            <div className="flex items-center gap-4">
              <button 
                onClick={(e) => { e.stopPropagation(); setGuestCount(guestCount - 1); }}
                disabled={guestCount <= 0}
                className="w-8 h-8 rounded-full border border-slate-300 flex items-center justify-center text-slate-500 hover:text-slate-800 hover:border-slate-800 disabled:opacity-30 disabled:hover:border-slate-300 disabled:hover:text-slate-500 transition-colors"
              >
                -
              </button>
              <span className="w-4 text-center font-medium text-slate-900">{guestCount}</span>
              <button 
                onClick={(e) => { e.stopPropagation(); setGuestCount(guestCount + 1); }}
                className="w-8 h-8 rounded-full border border-slate-300 flex items-center justify-center text-slate-500 hover:text-slate-800 hover:border-slate-800 transition-colors"
              >
                +
              </button>
            </div>
          </div>
        </PopoverContent>
      </Popover>
    </div>
  );
}
