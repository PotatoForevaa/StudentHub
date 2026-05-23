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

export type PaginatedResponse<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type User = {
    id: string;
    fullName: string;
    username: string;
    roles?: string[];
}

export type ActivityDto = {
    id: string;
    type: 'post' | 'comment' | 'rating';
    title?: string;
    content?: string;
    createdAt: string;
    projectId?: string;
    projectName?: string;
}

export type FieldErrors = Record<string, string>;
