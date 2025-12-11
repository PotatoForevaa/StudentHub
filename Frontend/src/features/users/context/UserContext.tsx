import { createContext, useEffect, useState, useContext, useCallback, type ReactNode } from "react";
import { userService } from "../../../shared/services/userService";
import type { User } from "../types";
import { AuthContext } from "../../auth/context/AuthContext";

export type UserContextType = {
    users: User[];
    loading: boolean;
    getUsers: () => Promise<void>;
};

export const UserContext = createContext<UserContextType>({
    users: [],
    loading: true,
    getUsers: async () => {}
});

export const UserProvider = ({ children }: { children: ReactNode }) => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const { isAuthenticated, loading: authLoading } = useContext(AuthContext);

  const getUsers = useCallback(async () => {
    setLoading(true);
    try {
        const res = await userService.getAllUsers();
        if (res && res.isSuccess && res.data) {
          setUsers(res.data as User[]);
        } else {
          console.warn('Failed to load users');
          setUsers([]);
        }
    } catch (err) {
        console.error('Error fetching users:', err);
        setUsers([]);
    } finally {
        setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (!authLoading && isAuthenticated) {
      getUsers();
    }
  }, [isAuthenticated, authLoading, getUsers]);

  return (
    <UserContext.Provider
      value={{
        users,
        loading,
        getUsers
      }}
    >
      {children}
    </UserContext.Provider>
  );
};
