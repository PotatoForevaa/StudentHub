import { createContext, useState, useEffect, type ReactNode } from "react";
import type { User } from "../types/User";
import authService from "../../services/api/authService";

export type AuthContextType = {
  isAuthenticated: boolean;
  user: User | null;
  loading: boolean;

  login: (username: string, password: string) => Promise<boolean>;
  register: (fullName: string, username: string, password: string) => Promise<boolean>;
  logout: () => Promise<void>;
  getUser: () => Promise<void>;
};

export const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  user: null,
  loading: false,
  login: async () => false,
  register: async () => false,
  logout: async () => {},
  getUser: async () => {},
});

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [isAuthenticated, setAuth] = useState(false);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  const login = async (username: string, password: string) => {
    try {
      await authService.login(username, password);
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
      setUser(null);
      setAuth(false);
    }
  };

  const getUser = async () => {
    try {
      const res = await authService.getCurrentUser();
      setUser(res.data);
      setAuth(true);
    } catch {
      setUser(null);
      setAuth(false);
    }
  };

  useEffect(() => {
    (async () => {
      await getUser(); 
      setLoading(false);
    })();
  }, []);

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        user,
        loading,
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
