import { createContext, useState, useEffect, type ReactNode } from "react";
import type { User } from "../../../shared/types";
import { authService } from "../services/authService";
import { baseUrl } from "../../../shared/services/base";

export type AuthContextType = {
  isAuthenticated: boolean;
  user: User | null;
  loading: boolean;
  picture: string | undefined;

  login: (username: string, password: string) => Promise<boolean>;
  register: (fullName: string, username: string, password: string) => Promise<boolean>;
  logout: () => Promise<void>;
  getUser: () => Promise<void>;
};

export const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  user: null,
  loading: false,
  picture: "",
  login: async () => false,
  register: async () => false,
  logout: async () => {},
  getUser: async () => {},
});

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [isAuthenticated, setAuth] = useState(false);
  const [user, setUser] = useState<User | null>(null);
  const [picture, setPicture] = useState("");
  const [loading, setLoading] = useState(true);

  const login = async (username: string, password: string) => {
    try {
      const res = await authService.login(username, password);
      // Store token if provided
      if (res?.data?.token) {
        localStorage.setItem('token', res.data.token);
      }
      await getUser();
      
      return true;
    } catch (err) {
      throw err;
    }
  };

  const register = async (fullName: string, username: string, password: string) => {
    try {
      await authService.register(username, password, fullName);
      await getUser();
      return true;
    } catch (err) {
      throw err;
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } finally {
      localStorage.removeItem('token');
      setUser(null);
      setAuth(false);
    }
  };

  const getUser = async () => {
    try {      
      const res = await authService.getCurrentUser();
      if (res && res.isSuccess && res.data) {
        setUser(res.data);
        setAuth(true);
        if (res.data.username) {
          setPicture(`${baseUrl}/api/Users/ProfilePicture/${res.data.username}`);
        }
      } else {
        setUser(null);
        setAuth(false);
      }
    } catch {
      setUser(null);
      setAuth(false);
    }
  };

  useEffect(() => {
    (async () => {
      // Check if token exists in localStorage (for Bearer token auth)
      const token = localStorage.getItem('token');
      if (token) {
        // Token exists, try to get user to verify it's still valid
        await getUser();
      } else {
        // No token, but still try getUser in case backend uses cookies
        await getUser();
      }
      setLoading(false);
    })();
  }, []);

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        user,
        loading,
        picture,
        login,
        register,
        logout,
        getUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

