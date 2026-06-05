"use client";

import { useState, useEffect } from "react";

import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";

interface DateRangePickerProps {
  from: string;
  to: string;
  onChange: (next: { from: string; to: string }) => void;
}

function todayIso(): string {
  return new Date().toISOString().slice(0, 10);
}

function daysAgoIso(days: number): string {
  const d = new Date();
  d.setDate(d.getDate() - days);
  return d.toISOString().slice(0, 10);
}

export function DateRangePicker({ from, to, onChange }: DateRangePickerProps) {
  const [localFrom, setLocalFrom] = useState(from);
  const [localTo, setLocalTo] = useState(to);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLocalFrom(from);
    setLocalTo(to);
  }, [from, to]);

  function validate(f: string, t: string): string | null {
    if (!f || !t) return "From and To are required.";
    if (f > t) return "From must be before To.";
    const fromD = new Date(f);
    const toD = new Date(t);
    const diff = Math.round((toD.getTime() - fromD.getTime()) / (1000 * 60 * 60 * 24)) + 1;
    if (diff > 365) return "Date range too large. Please use 365 days or less.";
    return null;
  }

  function apply() {
    const err = validate(localFrom, localTo);
    setError(err);
    if (!err) onChange({ from: localFrom, to: localTo });
  }

  function setPreset(days: number) {
    const newFrom = daysAgoIso(days);
    const newTo = todayIso();
    setLocalFrom(newFrom);
    setLocalTo(newTo);
    setError(null);
    onChange({ from: newFrom, to: newTo });
  }

  return (
    <div className="flex flex-col gap-2 sm:flex-row sm:items-end sm:gap-3">
      <div className="flex flex-col gap-1">
        <Label htmlFor="reports-from" className="text-xs text-[#6a6a6a]">From</Label>
        <Input
          id="reports-from"
          type="date"
          value={localFrom}
          onChange={(e) => setLocalFrom(e.target.value)}
          className="h-9 w-[160px]"
        />
      </div>
      <div className="flex flex-col gap-1">
        <Label htmlFor="reports-to" className="text-xs text-[#6a6a6a]">To</Label>
        <Input
          id="reports-to"
          type="date"
          value={localTo}
          onChange={(e) => setLocalTo(e.target.value)}
          className="h-9 w-[160px]"
        />
      </div>
      <Button onClick={apply} className="h-9 bg-[#222222] text-white hover:bg-[#000000]">Apply</Button>
      <div className="flex gap-1 sm:ml-2">
        <Button variant="ghost" size="sm" className="h-9 text-xs" onClick={() => setPreset(7)}>7d</Button>
        <Button variant="ghost" size="sm" className="h-9 text-xs" onClick={() => setPreset(30)}>30d</Button>
        <Button variant="ghost" size="sm" className="h-9 text-xs" onClick={() => setPreset(90)}>90d</Button>
      </div>
      {error && (
        <p className="text-xs text-[#ff385c] sm:ml-2">{error}</p>
      )}
    </div>
  );
}
