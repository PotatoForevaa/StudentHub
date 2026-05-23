import { useContext } from "react";
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { AuthContext } from "../../features/auth/context/AuthContext";
import { hasRole } from "../utils/roles";

export const PrivateRoute = ({ roles }: { roles?: string[] }) => {
  const { isAuthenticated, loading, user } = useContext(AuthContext);
  const location = useLocation();

  if (loading) return <div>Loading...</div>;

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/login"
        replace
        state={{ from: location }} 
      />
    );
  }

  if (roles && !roles.some(role => hasRole(user, role))) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
};

