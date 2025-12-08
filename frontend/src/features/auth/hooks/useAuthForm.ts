import { useState } from "react";
import type { AxiosError } from "axios";
import type { FieldErrors } from "../types";
import type { ApiErrorResponse, ApiResponse } from "../../../shared/types";

export function useAuthForm() {
  const [loading, setLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  const handleError = (err: unknown) => {
    setLoading(false);

    const axiosError = err as AxiosError<ApiResponse | ApiErrorResponse>;
    const data = axiosError.response?.data;

    // Debug logging
    console.log('Error response data:', data);
    console.log('Error response status:', axiosError.response?.status);

    if (!data) {
      setFormError("Произошла неизвестная ошибка");
      return;
    }

    // Handle API response errors array (from ApiResponse.errors - ApiError[])
    if ('errors' in data && Array.isArray(data.errors)) {
      const fields: FieldErrors = {};
      let hasFieldErrors = false;
      let generalError: string | null = null;
      
      for (const error of data.errors) {
        if (error.field && error.message) {
          // Normalize field name: convert to lowercase and handle camelCase/PascalCase
          // e.g., "FullName" -> "fullname", "fullName" -> "fullname", "Username" -> "username"
          const fieldName = error.field.toLowerCase();
          
          // If field already has an error, append with newline
          fields[fieldName] = fields[fieldName] 
            ? `${fields[fieldName]}\n${error.message}`
            : error.message;
          hasFieldErrors = true;
        } else if (error.message) {
          // If no field specified, collect as general error
          generalError = generalError 
            ? `${generalError}\n${error.message}`
            : error.message;
        }
      }
      
      if (hasFieldErrors) {
        setFieldErrors(fields);
        // Also set general error if there are any (but don't override field errors)
        if (generalError && Object.keys(fields).length === 0) {
          setFormError(generalError);
        }
        return;
      }
      
      // If we only have general errors, set form error
      if (generalError) {
        setFormError(generalError);
        return;
      }
    }
    
    // Handle validation errors as Record<string, string[]> (ASP.NET Core ModelState format)
    if ('errors' in data && !Array.isArray(data.errors) && typeof data.errors === 'object') {
      const fields: FieldErrors = {};
      for (const key in data.errors) {
        const value = data.errors[key];
        fields[key.toLowerCase()] = Array.isArray(value)
          ? value.join("\n")
          : String(value);
      }
      setFieldErrors(fields);
      return;
    }
    
    // Handle detail field (if present in ApiErrorResponse)
    if ('detail' in data && data.detail) {
      setFormError(data.detail);
      return;
    }
    
    // Fallback
    setFormError("Произошла неизвестная ошибка");
  };

  const resetErrors = () => {
    setFieldErrors({});
    setFormError(null);
  };

  return {
    loading,
    formError,
    fieldErrors,
    setLoading,
    handleError,
    resetErrors,
  };
}
