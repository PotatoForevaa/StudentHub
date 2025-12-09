import { useState } from "react";
import type { AxiosError } from "axios";
import type { FieldErrors, ProjectFormData } from "../types";
import type { ApiErrorResponse, ApiResponse } from "../../../shared/types";
import { projectService } from "../services/projectService";

export function useProjectForm(onSuccess?: () => void) {
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

  const onSubmit = async (data: ProjectFormData, files: File[]) => {
    setLoading(true);
    resetErrors();

    try {
      const form = new FormData();
      form.append('name', data.name);
      form.append('description', data.description);
      if (data.externalUrl) {
        form.append('externalUrl', data.externalUrl);
      }
      files.forEach(file => {
        form.append('files', file);
      });

      const result = await projectService.addProject(form);

      if (result.isSuccess) {
        onSuccess?.();
      } else {
        const fields: FieldErrors = {};
        result.errors?.forEach(error => {
          const field = error.field?.toLowerCase() || 'general';
          if (field === 'general') {
            setFormError(error.message || 'Unknown error');
          } else {
            fields[field] = error.message || 'Unknown error';
          }
        });
        setFieldErrors(fields);
      }
    } catch (error) {
      handleError(error);
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    formError,
    fieldErrors,
    onSubmit,
    resetErrors,
  };
}
