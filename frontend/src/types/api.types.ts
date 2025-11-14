export interface ApiErrorResponse {
  detail?: string;
  errors?: Record<string, string[]>; 
}

export type FieldErrors = Record<string, string>;
