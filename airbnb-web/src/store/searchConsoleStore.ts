import { create } from 'zustand';

interface DateRange {
  from?: Date;
  to?: Date;
}

interface LocationData {
  name: string;
  code: number | string; // Province or Ward code
  type: 'province' | 'ward';
}

interface SearchConsoleState {
  // State
  location: LocationData | null;
  dateRange: DateRange | null;
  guestCount: number;

  // Actions
  setLocation: (location: LocationData | null) => void;
  setDateRange: (range: DateRange | null) => void;
  setGuestCount: (count: number) => void;
  clearSearch: () => void;
}

export const useSearchConsoleStore = create<SearchConsoleState>((set) => ({
  location: null,
  dateRange: null,
  guestCount: 0,

  setLocation: (location) => set({ location }),
  
  setDateRange: (dateRange) => set({ dateRange }),
  
  setGuestCount: (count) => set({ guestCount: Math.max(0, count) }),
    
  clearSearch: () => 
    set({ 
      location: null, 
      dateRange: null, 
      guestCount: 0 
    }),
}));
