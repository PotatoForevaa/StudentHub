import type { AxiosError } from "axios";
import type { FieldErrors } from "../types";

export function useFormErrors() {
  const parseErrorResponse = (error: unknown) => {
    const axiosError = error as AxiosError<Record<string, any>>;
    const data = axiosError.response?.data;

    if (!data) {
      return { formError: "Произошла неизвестная ошибка", fieldErrors: {} };
    }

    // Handle array of errors
    if (data.errors && Array.isArray(data.errors)) {
      const fieldErrors: FieldErrors = {};
      let generalError: string | null = null;

      for (const err of data.errors) {
        if (err.field && err.message) {
          const fieldName = err.field.toLowerCase();
          fieldErrors[fieldName] = fieldErrors[fieldName]
            ? `${fieldErrors[fieldName]}\n${err.message}`
            : err.message;
        } else if (err.message) {
          generalError = generalError
            ? `${generalError}\n${err.message}`
            : err.message;
        }
      }

      return {
        formError: generalError,
        fieldErrors: Object.keys(fieldErrors).length > 0 ? fieldErrors : {}
      };
    }

    // Handle object of errors
    if (data.errors && typeof data.errors === 'object') {
      const fieldErrors: FieldErrors = {};
      for (const [key, value] of Object.entries(data.errors)) {
        fieldErrors[key.toLowerCase()] = Array.isArray(value)
          ? value.join("\n")
          : String(value);
      }
      return { formError: null, fieldErrors };
    }

    // Handle single detail error
    if (data.detail) {
      return { formError: data.detail, fieldErrors: {} };
    }

    return { formError: "Произошла неизвестная ошибка", fieldErrors: {} };
  };

  return { parseErrorResponse };
}
