import { useState } from "react";
import type { AxiosError } from "axios";
import type { FieldErrors } from "../types/AuthForm.types";
import type { ApiErrorResponse } from "../../../services/api/api.types";

export function useAuthForm() {
  const [loading, setLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  const handleError = (err: unknown) => {
    setLoading(false);

    const axiosError = err as AxiosError<ApiErrorResponse>;
    const data = axiosError.response?.data;

    if (!data) {
      setFormError("Произошла неизвестная ошибка");
      return;
    }

    if (data.errors) {
      const fields: FieldErrors = {};
      for (const key in data.errors) {
        const value = data.errors[key];
        fields[key.toLowerCase()] = Array.isArray(value)
          ? value.join("\n")
          : String(value);
      }
      setFieldErrors(fields);
    } else if (data.detail) {
      setFormError(data.detail);
    } else {
      setFormError("Произошла неизвестная ошибка");
    }
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
