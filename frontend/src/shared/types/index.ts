export interface ApiErrorResponse {
  detail?: string;
  errors?: Record<string, string[]> | ApiError[]; 
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

export type User = {
    fullName: string;
    username: string;
}

