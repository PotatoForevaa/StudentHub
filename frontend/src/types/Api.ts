export interface ApiErrorResponse {
  detail?: string;
  errors?: Record<string, string[]>; 
}

export interface ApiError {
  message?: string;
  field?: string;
}

export interface ApiResponse<T = unknown> {
  isSuccess: boolean;
  data?: T;
  errors?: ApiError[];
  errorType?: string;
}
