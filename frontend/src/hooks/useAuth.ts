import { useState } from "react";
import authService from "../services/api/authService";
import type { apiError } from "../types/auth.types";

export function useAuth() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const login = async (username: string, password: string) => {
    setLoading(true);
    setError(null);
    try {
      const res = await authService.login(username, password);
      return res.data;
    } catch (err: any) {
      const error = err.response.data as apiError;
      setError(error.detail || "ошибка юзауф");
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    setLoading(true);
    setError(null);
    try {
      await authService.logout();
    } catch (err: any) {
      const error = err.response.data as apiError;
      setError(error.detail || "ошибка юзауф");
    } finally {
      setLoading(false);
    }
  };

  return { login, logout, loading, error };
}
