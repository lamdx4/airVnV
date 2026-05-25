import { useQuery } from '@tanstack/react-query';
import { propertiesApi } from '@/features/properties/api/properties';
import { formatCurrency } from '../utils/mappers';
import { Button } from '@/components/ui/button';
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet';
import { Skeleton } from '@/components/ui/skeleton';
import { Badge } from '@/components/ui/badge';
import { ScrollArea } from '@/components/ui/scroll-area';

interface PropertyDetailDrawerProps {
  propertyId: string | null;
  isOpen: boolean;
  onClose: () => void;
  onApprove: (propertyId: string) => void;
  onReject: (propertyId: string) => void;
}

export function PropertyDetailDrawer({
  propertyId,
  isOpen,
  onClose,
  onApprove,
  onReject,
}: PropertyDetailDrawerProps) {
  const { data: property, isLoading } = useQuery({
    queryKey: ['property', propertyId],
    queryFn: () => propertiesApi.getProperty(propertyId!),
    enabled: !!propertyId,
    staleTime: 30_000,
  });

  // Normalize status to handle both string and number types from API
  const status = Number(property?.status ?? 0);
  const statusStr = String(property?.status ?? '');

  return (
    <Sheet open={isOpen} onOpenChange={(open: boolean) => !open && onClose()}>
      <SheetContent className="w-full sm:max-w-xl md:max-w-2xl p-0 overflow-hidden">
        <SheetHeader className="p-6 pb-0">
          <SheetTitle className="text-xl font-semibold">Property Details</SheetTitle>
        </SheetHeader>

        <ScrollArea className="h-[calc(100vh-120px)]">
          {isLoading ? (
            <div className="p-6 space-y-4">
              <Skeleton className="w-full h-48 rounded-lg" />
              <Skeleton className="w-3/4 h-8" />
              <Skeleton className="w-1/2 h-6" />
              <Skeleton className="w-full h-32" />
            </div>
          ) : property ? (
            <div className="p-6 space-y-6">
              {/* Image Gallery */}
              <div className="grid grid-cols-2 gap-2">
                {property.images.slice(0, 4).map((image, index) => (
                  <div
                    key={image.id}
                    className={`aspect-video rounded-lg bg-cover bg-center ${
                      index === 0 ? 'col-span-2 row-span-2' : ''
                    }`}
                    style={{ backgroundImage: `url(${image.url})` }}
                  />
                ))}
              </div>

              {/* Title & Status */}
              <div>
                <div className="flex items-start justify-between gap-2 mb-2">
                  <h2 className="text-2xl font-semibold text-[#222222]">{property.title}</h2>
                  <Badge
                    variant="secondary"
                    className={
                      statusStr === 'Published' || status === 2
                        ? 'bg-[#008A05] text-white'
                        : statusStr === 'PendingReview' || status === 1
                        ? 'bg-[#FFB400] text-white'
                        : 'bg-[#717171] text-white'
                    }
                  >
                    {property.status}
                  </Badge>
                </div>
                <p className="text-[#717171]">{property.displayAddress}</p>
              </div>

              {/* Host Info */}
              <div className="p-4 bg-[#F7F7F7] rounded-xl">
                <p className="text-sm text-[#717171]">Host ID</p>
                <p className="font-medium">{property.hostId}</p>
              </div>

              {/* Description */}
              <div>
                <h3 className="font-semibold text-[#222222] mb-2">Description</h3>
                <p className="text-[#717171] whitespace-pre-wrap">{property.description}</p>
              </div>

              {/* Capacity */}
              <div>
                <h3 className="font-semibold text-[#222222] mb-2">Capacity</h3>
                <div className="grid grid-cols-4 gap-2">
                  <div className="p-3 bg-[#F7F7F7] rounded-lg text-center">
                    <p className="text-2xl font-semibold">{property.capacity.guestCount}</p>
                    <p className="text-xs text-[#717171]">Guests</p>
                  </div>
                  <div className="p-3 bg-[#F7F7F7] rounded-lg text-center">
                    <p className="text-2xl font-semibold">{property.capacity.bedroomCount}</p>
                    <p className="text-xs text-[#717171]">Bedrooms</p>
                  </div>
                  <div className="p-3 bg-[#F7F7F7] rounded-lg text-center">
                    <p className="text-2xl font-semibold">{property.capacity.bedCount}</p>
                    <p className="text-xs text-[#717171]">Beds</p>
                  </div>
                  <div className="p-3 bg-[#F7F7F7] rounded-lg text-center">
                    <p className="text-2xl font-semibold">{property.capacity.bathroomCount}</p>
                    <p className="text-xs text-[#717171]">Bathrooms</p>
                  </div>
                </div>
              </div>

              {/* Pricing */}
              <div>
                <h3 className="font-semibold text-[#222222] mb-2">Pricing</h3>
                <div className="space-y-2">
                  <div className="flex justify-between p-3 bg-[#F7F7F7] rounded-lg">
                    <span className="text-[#717171]">Base Price</span>
                    <span className="font-medium">
                      {formatCurrency(property.pricing.basePrice, property.pricing.currencyCode)}
                    </span>
                  </div>
                  <div className="flex justify-between p-3 bg-[#F7F7F7] rounded-lg">
                    <span className="text-[#717171]">Cleaning Fee</span>
                    <span className="font-medium">
                      {formatCurrency(property.pricing.cleaningFee, property.pricing.currencyCode)}
                    </span>
                  </div>
                  <div className="flex justify-between p-3 bg-[#F7F7F7] rounded-lg">
                    <span className="text-[#717171]">Service Fee</span>
                    <span className="font-medium">
                      {formatCurrency(property.pricing.serviceFee, property.pricing.currencyCode)}
                    </span>
                  </div>
                </div>
              </div>

              {/* House Rules */}
              <div>
                <h3 className="font-semibold text-[#222222] mb-2">House Rules</h3>
                <div className="grid grid-cols-3 gap-2">
                  <div className={`p-3 rounded-lg text-center ${property.houseRules.allowPets ? 'bg-[#008A05]/10' : 'bg-[#F7F7F7]'}`}>
                    <p className="text-lg">{property.houseRules.allowPets ? '✓' : '✗'}</p>
                    <p className="text-xs text-[#717171]">Pets</p>
                  </div>
                  <div className={`p-3 rounded-lg text-center ${property.houseRules.allowSmoking ? 'bg-[#008A05]/10' : 'bg-[#F7F7F7]'}`}>
                    <p className="text-lg">{property.houseRules.allowSmoking ? '✓' : '✗'}</p>
                    <p className="text-xs text-[#717171]">Smoking</p>
                  </div>
                  <div className={`p-3 rounded-lg text-center ${property.houseRules.allowEvents ? 'bg-[#008A05]/10' : 'bg-[#F7F7F7]'}`}>
                    <p className="text-lg">{property.houseRules.allowEvents ? '✓' : '✗'}</p>
                    <p className="text-xs text-[#717171]">Events</p>
                  </div>
                </div>
                <div className="mt-2 p-3 bg-[#F7F7F7] rounded-lg">
                  <p className="text-sm text-[#717171]">Check-in: {property.houseRules.checkInTime} / Check-out: {property.houseRules.checkOutTime}</p>
                </div>
              </div>

              {/* Amenities */}
              {property.propertyAmenities.length > 0 && (
                <div>
                  <h3 className="font-semibold text-[#222222] mb-2">Amenities</h3>
                  <div className="flex flex-wrap gap-2">
                    {property.propertyAmenities.map((amenity) => (
                      <Badge key={amenity.amenityId} variant="secondary" className="px-3 py-1">
                        {amenity.name}
                      </Badge>
                    ))}
                  </div>
                </div>
              )}

              {/* Actions */}
              {(statusStr === 'PendingReview' || status === 1) && (
                <div className="flex gap-3 pt-4 border-t">
                  <Button
                    className="flex-1 h-12 bg-[#FF5A5F] hover:bg-[#E31C5F] text-white rounded-xl"
                    onClick={() => propertyId && onApprove(propertyId)}
                  >
                    Approve Property
                  </Button>
                  <Button
                    variant="outline"
                    className="flex-1 h-12 border-[#FF5A5F] text-[#FF5A5F] hover:bg-[#FFF5F5] rounded-xl"
                    onClick={() => propertyId && onReject(propertyId)}
                  >
                    Reject Property
                  </Button>
                </div>
              )}
            </div>
          ) : null}
        </ScrollArea>
      </SheetContent>
    </Sheet>
  );
}