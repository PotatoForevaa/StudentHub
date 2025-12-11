import { useState } from "react";
import type { FieldErrors } from "../../../shared/types";
import type { ProjectFormData } from "../types";
import { useFormErrors } from "../../../shared/hooks/useFormErrors";
import { projectService } from "../services/projectService";

export function useProjectForm(onSuccess?: () => void) {
  const [loading, setLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const { parseErrorResponse } = useFormErrors();

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
      const { formError: parsedFormError, fieldErrors: parsedFieldErrors } = parseErrorResponse(error);
      setFormError(parsedFormError);
      setFieldErrors(parsedFieldErrors);
      setLoading(false);
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
