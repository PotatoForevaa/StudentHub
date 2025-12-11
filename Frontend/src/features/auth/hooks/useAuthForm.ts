import { useState } from "react";
import type { FieldErrors } from "../../../shared/types";
import { useFormErrors } from "../../../shared/hooks/useFormErrors";

export function useAuthForm() {
  const [loading, setLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const { parseErrorResponse } = useFormErrors();

  const handleError = (err: unknown) => {
    setLoading(false);

    const { formError: parsedFormError, fieldErrors: parsedFieldErrors } = parseErrorResponse(err);

    setFormError(parsedFormError);
    setFieldErrors(parsedFieldErrors);
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
