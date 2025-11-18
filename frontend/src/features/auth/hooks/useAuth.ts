import { useState, useEffect } from "react";
import type { AxiosError } from "axios";
import authService from "../../../services/api/authService";
import type { FieldErrors } from "../types/AuthForm.types";
import type { ApiErrorResponse } from "../../../services/api/api.types";


export function useAuth() {
  const [loading, setLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [user, setUser] = useState(null);

  useEffect(() => {
    const fetchUser = async () => {
      setLoading(true);
      try {
        const res = await authService.getCurrentUser();
        setUser(res.data);   
      } catch (err) {
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, []);

  const handleError = (err: unknown) => {
    setFormError(null);
    setFieldErrors({});

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
        fields[key] = Array.isArray(value) ? value.join(", ") : String(value);
      }
      setFieldErrors(fields);

    } else if (data.detail) {
      setFormError(data.detail);
    } else {
      setFormError("Произошла неизвестная ошибка");
    }
  };

  const login = async (username: string, password: string) => {
    setLoading(true);
    try {
      const res = await authService.login(username, password);
      return res.data;
    } catch (err: unknown) {
      handleError(err);
    } finally {
      setLoading(false);
    }
  };

  const register = async (fullName: string, username: string, password: string) => {
    setLoading(true);
    try {
      const res = await authService.register(username, password, fullName);
      return res.data;
    } catch (err: unknown) {
      handleError(err);
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    setLoading(true);
    try {
      await authService.logout();
    } catch (err: unknown) {
      handleError(err);
    } finally {
      setLoading(false);
    }
  };

  const getUser = async () => {
    setLoading(true);
    try {
      const res = await authService.getCurrentUser();
      setUser(res.data); 
    } catch (err) {
      setUser(null);
    } finally {
      setLoading(false);
    }
  };

  return { login, register, logout, getUser, user, loading, formError, fieldErrors };
}
