export interface InitiatePaymentRequest {
  bookingId: string;
}

export interface InitiatePaymentResponse {
  paymentUrl: string;
}
