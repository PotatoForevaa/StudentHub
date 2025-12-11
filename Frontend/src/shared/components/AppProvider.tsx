import { ReactNode } from 'react';
import { AuthProvider } from '../../features/auth/context/AuthContext';
import { ProjectProvider } from '../../features/projects/context/ProjectContext';
import { UserProvider } from '../../features/users/context/UserContext';

interface AppProviderProps {
  children: ReactNode;
}

export const AppProvider = ({ children }: AppProviderProps) => {
  return (
    <AuthProvider>
      <ProjectProvider>
        <UserProvider>
          {children}
        </UserProvider>
      </ProjectProvider>
    </AuthProvider>
  );
};
