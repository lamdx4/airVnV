export { propertiesApi } from "./api/properties";
export type { PropertyStatus, PropertyStatusValue, Property, PropertyListParams } from "./types";
export {
  useProperties,
  useProperty,
  useApproveProperty,
  useRejectProperty,
  useDeactivateProperty,
  useDeleteProperty,
} from "./hooks";
export { getPropertyStatusConfig } from "./utils/status";
