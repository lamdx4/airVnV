import React, { useState, useEffect, useCallback } from 'react';
import { MapContainer, TileLayer, Marker, useMapEvents, useMap } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import { Location01Icon, PinIcon, Loading03Icon, SentIcon } from 'hugeicons-react';
import { useTranslation } from 'react-i18next';
import { useUpdateLocation } from '../hooks/useProperties';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

// Fix leaflet icon issue in React
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

let DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41]
});
L.Marker.prototype.options.icon = DefaultIcon;

// --- Open API v2 Interfaces ---
interface ProvinceResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  phone_code: number;
  wards: WardResponse[];
}

interface WardResponse {
  name: string;
  code: number;
  division_type: string;
  codename: string;
  province_code: number;
}

interface LocationManagerProps {
  propertyId: string;
  initialLat: number;
  initialLng: number;
  initialAddress: string;
  initialSubDivisions?: Record<string, string>;
}

// ─────────────────────────────────────────────────────────────
// FIX 1: Tách LocationMarker ra ngoài component cha
// Trước đây khai báo TRONG render function → React tạo ra type mới
// mỗi lần re-render → unmount/remount marker (flicker).
// ─────────────────────────────────────────────────────────────
interface LocationMarkerProps {
  position: [number, number];
  onPositionChange: (lat: number, lng: number) => void;
}

const LocationMarker: React.FC<LocationMarkerProps> = ({ position, onPositionChange }) => {
  useMapEvents({
    click(e) {
      onPositionChange(e.latlng.lat, e.latlng.lng);
    },
  });

  return (
    <Marker
      position={position}
      draggable={true}
      eventHandlers={{
        dragend: (e) => {
          const marker = e.target;
          const pos = marker.getLatLng();
          onPositionChange(pos.lat, pos.lng);
        }
      }}
    />
  );
};

// ─────────────────────────────────────────────────────────────
// FIX 2: FlyToPosition – map tự re-center khi position đổi
// MapContainer.center chỉ đọc lần đầu, không reactive.
// ─────────────────────────────────────────────────────────────
const FlyToPosition: React.FC<{ position: [number, number] }> = ({ position }) => {
  const map = useMap();
  useEffect(() => {
    map.flyTo(position, map.getZoom(), { animate: true, duration: 0.8 });
  }, [position, map]);
  return null;
};

// ─────────────────────────────────────────────────────────────
// Main Component
// ─────────────────────────────────────────────────────────────
export const LocationManager: React.FC<LocationManagerProps> = ({
  propertyId,
  initialLat,
  initialLng,
  initialAddress,
  initialSubDivisions
}) => {
  const { t } = useTranslation();

  // Map State
  const [position, setPosition] = useState<[number, number]>([initialLat, initialLng]);
  const [isReverseGeocoding, setIsReverseGeocoding] = useState(false);

  // Address State
  const [streetAddress, setStreetAddress] = useState(initialAddress || '');
  const [selectedProvinceCode, setSelectedProvinceCode] = useState<string>('');
  const [selectedWardCode, setSelectedWardCode] = useState<string>('');

  // API Data State
  const [provinces, setProvinces] = useState<ProvinceResponse[]>([]);
  const [wards, setWards] = useState<WardResponse[]>([]);
  const [isLoadingProvinces, setIsLoadingProvinces] = useState(false);
  const [isLoadingWards, setIsLoadingWards] = useState(false);

  const updateLocationMutation = useUpdateLocation();

  // Fetch all provinces on mount
  useEffect(() => {
    const fetchProvinces = async () => {
      setIsLoadingProvinces(true);
      try {
        const res = await fetch('https://provinces.open-api.vn/api/v2/p/');
        const data: ProvinceResponse[] = await res.json();
        setProvinces(data);
      } catch (err) {
        console.error("Failed to fetch provinces", err);
      } finally {
        setIsLoadingProvinces(false);
      }
    };
    fetchProvinces();
  }, []);

  // ─────────────────────────────────────────────────────────────
  // FIX 3: Pre-select province/ward CHỈ SAU KHI provinces đã fetch xong
  // Trước đây set trong useState initial → provinces list chưa có
  // → Select có value nhưng options rỗng → hiển thị placeholder.
  // ─────────────────────────────────────────────────────────────
  useEffect(() => {
    if (provinces.length === 0) return;
    const savedProvinceCode = initialSubDivisions?.provinceCode;
    if (savedProvinceCode) {
      // Kiểm tra province có thực sự tồn tại trong danh sách không
      const exists = provinces.some(p => p.code.toString() === savedProvinceCode);
      if (exists) {
        setSelectedProvinceCode(savedProvinceCode);
      }
    }
  }, [provinces, initialSubDivisions?.provinceCode]);

  // Fetch wards when province selected (depth=2)
  useEffect(() => {
    if (!selectedProvinceCode) {
      setWards([]);
      return;
    }

    const fetchWards = async () => {
      setIsLoadingWards(true);
      try {
        const res = await fetch(`https://provinces.open-api.vn/api/v2/p/${selectedProvinceCode}?depth=2`);
        const data: ProvinceResponse = await res.json();
        setWards(data.wards || []);
      } catch (err) {
        console.error("Failed to fetch wards", err);
      } finally {
        setIsLoadingWards(false);
      }
    };

    fetchWards();
  }, [selectedProvinceCode]);

  // Pre-select ward CHỈ SAU KHI wards của province đã fetch xong
  useEffect(() => {
    if (wards.length === 0) return;
    const savedWardCode = initialSubDivisions?.wardCode;
    if (savedWardCode) {
      const exists = wards.some(w => w.code.toString() === savedWardCode);
      if (exists) {
        setSelectedWardCode(savedWardCode);
      }
    }
  }, [wards, initialSubDivisions?.wardCode]);

  // ─────────────────────────────────────────────────────────────
  // FIX 4: Reverse geocode chỉ lấy đường phố/số nhà ngắn gọn,
  // không dump toàn bộ display_name (quá dài, lẫn thành phố, quận...)
  // ─────────────────────────────────────────────────────────────
  const reverseGeocode = useCallback(async (lat: number, lng: number) => {
    setIsReverseGeocoding(true);
    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}&zoom=18&addressdetails=1`
      );
      const data = await response.json();
      if (data?.address) {
        const addr = data.address;
        // Ưu tiên: house_number + road → ngắn gọn, đúng nghĩa "street address"
        const streetParts = [addr.house_number, addr.road].filter(Boolean);
        if (streetParts.length > 0) {
          setStreetAddress(streetParts.join(' '));
        }
        // Nếu không có road (khu vực ngoại thành), fallback sang suburb/quarter
        else if (addr.suburb || addr.quarter || addr.neighbourhood) {
          setStreetAddress(addr.suburb || addr.quarter || addr.neighbourhood);
        }
        // Không set gì cả nếu không có thông tin hữu ích – giữ nguyên input của user
      }
    } catch (err) {
      console.error('Failed to reverse geocode');
    } finally {
      setIsReverseGeocoding(false);
    }
  }, []);

  const handlePositionChange = useCallback((lat: number, lng: number) => {
    setPosition([lat, lng]);
    reverseGeocode(lat, lng);
  }, [reverseGeocode]);

  const handleSave = async () => {
    const selectedProvince = provinces.find(p => p.code.toString() === selectedProvinceCode);
    const selectedWard = wards.find(w => w.code.toString() === selectedWardCode);

    const parts = [streetAddress];
    if (selectedWard) parts.push(selectedWard.name);
    if (selectedProvince) parts.push(selectedProvince.name);
    const fullAddress = parts.filter(Boolean).join(', ');

    try {
      await updateLocationMutation.mutateAsync({
        propertyId,
        data: {
          latitude: position[0],
          longitude: position[1],
          displayAddress: fullAddress,
          countryCode: 'VN',
          streetAddress: streetAddress,
          admin1Code: selectedProvinceCode,
          subDivisions: {
            province: selectedProvince?.name || '',
            provinceCode: selectedProvinceCode,
            ward: selectedWard?.name || '',
            wardCode: selectedWardCode
          }
        }
      });
      toast.success(t('location.updateSuccess'));
    } catch (err) {
      toast.error(t('location.updateFailed'));
    }
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
      {/* Sidebar Controls */}
      <div className="space-y-6">
        <div>
          <h3 className="text-lg font-bold text-hof flex items-center gap-2">
            <Location01Icon className="h-5 w-5 text-rausch" />
            {t('location.propertyAddress')}
          </h3>
          <p className="text-sm text-slate-500 mt-1">
            {t('location.addressSubtitle')}
          </p>
        </div>

        <div className="bg-slate-50 p-4 rounded-2xl border space-y-4">

          {/* Province Selection */}
          <div className="space-y-1">
            <Label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">
              {t('location.province')}
            </Label>
            <Select
              value={selectedProvinceCode}
              onValueChange={(val) => {
                setSelectedProvinceCode(val);
                setSelectedWardCode('');
              }}
              disabled={isLoadingProvinces}
            >
              <SelectTrigger className="w-full bg-white h-11 border-slate-200">
                <SelectValue
                  placeholder={
                    isLoadingProvinces
                      ? t('location.loadingProvinces')
                      : t('location.selectProvincePlaceholder')
                  }
                />
              </SelectTrigger>
              <SelectContent>
                {provinces.map((prov) => (
                  <SelectItem key={prov.code} value={prov.code.toString()}>
                    {prov.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Ward Selection */}
          <div className="space-y-1">
            <Label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">
              {t('location.ward')}
            </Label>
            <Select
              value={selectedWardCode}
              onValueChange={setSelectedWardCode}
              disabled={!selectedProvinceCode || isLoadingWards}
            >
              <SelectTrigger className="w-full bg-white h-11 border-slate-200">
                <SelectValue
                  placeholder={
                    !selectedProvinceCode
                      ? t('location.selectProvinceFirst')
                      : isLoadingWards
                        ? t('location.loadingWards')
                        : t('location.selectWardPlaceholder')
                  }
                />
              </SelectTrigger>
              <SelectContent>
                {wards.map((ward) => (
                  <SelectItem key={ward.code} value={ward.code.toString()}>
                    {ward.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* Street Address */}
          <div className="space-y-1">
            <Label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">
              {t('location.streetAddress')}
            </Label>
            <Textarea
              value={streetAddress}
              onChange={(e) => setStreetAddress(e.target.value)}
              className={`min-h-[80px] rounded-xl border-slate-200 focus-visible:ring-rausch focus-visible:border-rausch text-sm text-hof resize-none ${isReverseGeocoding ? 'opacity-50' : ''}`}
              placeholder={t('location.streetAddressPlaceholder')}
            />
            {isReverseGeocoding && (
              <p className="text-[10px] text-slate-400 flex items-center gap-1">
                <Loading03Icon className="h-3 w-3 animate-spin" />
                {t('location.detectingAddress')}
              </p>
            )}
          </div>

          {/* Coordinates */}
          <div className="space-y-1 pt-2 border-t border-slate-200">
            <Label className="text-[10px] font-bold uppercase text-slate-400 tracking-wider">
              {t('location.coordinates')}
            </Label>
            <div className="text-sm font-mono text-hof bg-white p-2 rounded-lg border">
              {position[0].toFixed(6)}, {position[1].toFixed(6)}
            </div>
          </div>
        </div>

        <Button
          onClick={handleSave}
          disabled={
            updateLocationMutation.isPending ||
            isReverseGeocoding ||
            !selectedProvinceCode ||
            !selectedWardCode
          }
          className="w-full bg-hof text-white h-12 rounded-xl font-bold hover:bg-slate-800"
        >
          {updateLocationMutation.isPending
            ? <Loading03Icon className="h-5 w-5 animate-spin mr-2" />
            : <SentIcon className="h-5 w-5 mr-2" />
          }
          {t('location.saveLocation')}
        </Button>
      </div>

      {/* Map Display */}
      <div className="lg:col-span-2 aspect-video lg:aspect-auto h-[500px] rounded-3xl overflow-hidden border shadow-inner relative">
        <MapContainer
          center={position}
          zoom={16}
          scrollWheelZoom={true}
          className="h-full w-full"
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          {/* FIX 2: Map tự fly đến position mới */}
          <FlyToPosition position={position} />
          {/* FIX 1: LocationMarker tách ngoài, stable reference */}
          <LocationMarker position={position} onPositionChange={handlePositionChange} />
        </MapContainer>

        <div className="absolute top-4 left-4 z-[1000] bg-white/95 backdrop-blur px-4 py-2 rounded-full shadow-lg border flex items-center gap-2 pointer-events-none">
          <PinIcon className="h-4 w-4 text-rausch" />
          <span className="text-xs font-bold text-hof">{t('location.clickOrDrag')}</span>
        </div>
      </div>
    </div>
  );
};
