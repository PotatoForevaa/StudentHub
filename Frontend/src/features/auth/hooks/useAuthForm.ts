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

    console.log('Error response data:', data);
    console.log('Error response status:', axiosError.response?.status);

    if (!data) {
      setFormError("Произошла неизвестная ошибка");
      return;
    }

    if ('errors' in data && Array.isArray(data.errors)) {
      const fields: FieldErrors = {};
      let hasFieldErrors = false;
      let generalError: string | null = null;
      
      for (const error of data.errors) {
        if (error.field && error.message) {
          const fieldName = error.field.toLowerCase();
          
          fields[fieldName] = fields[fieldName] 
            ? `${fields[fieldName]}\n${error.message}`
            : error.message;
          hasFieldErrors = true;
        } else if (error.message) {
          generalError = generalError 
            ? `${generalError}\n${error.message}`
            : error.message;
        }
      }
      
      if (hasFieldErrors) {
        setFieldErrors(fields);
        if (generalError && Object.keys(fields).length === 0) {
          setFormError(generalError);
        }
        return;
      }

      if (generalError) {
        setFormError(generalError);
        return;
      }
    }
    
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
    
    if ('detail' in data && data.detail) {
      setFormError(data.detail);
      return;
    }
    
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
