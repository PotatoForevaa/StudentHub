import { useEffect, useState } from "react";
import { Login, Registration } from "../features/auth/pages";
import { Header } from "./Header";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { styled } from "styled-components";
import { createGlobalStyle } from "styled-components";
import { AuthContext } from "../shared/context/AuthContext";
import { PrivateRoute } from "./PrivateRoute";
import authService from "../services/api/authService";

const GlobalStyles = createGlobalStyle`
    body {
      margin: 0;
      padding: 0;
    }
    
    * {
      box-sizing: border-box;
      font-family: calibri;
    }
  `;

const AppContainer = styled.div`
  min-height: 100vh;
  display: flex;
  flex-direction: column;
`;

const MainContent = styled.main`
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #0c0032;
`;

function App() {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setAuth] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const res = await authService.getCurrentUser();
        setUser(res.data);
        setAuth(true);
      } catch {
        setUser(null);
        setAuth(false);
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, []);

  return (
    <BrowserRouter>
      <AuthContext.Provider value={{ isAuthenticated, setAuth }}>
        <GlobalStyles />
        <AppContainer>
          {loading ? (
            <div>Loading...</div>
          ) : (
            <>
              <Header />
              <MainContent>
                <Routes>
                  <Route path="/login" element={<Login />} />
                  <Route path="/registration" element={<Registration />} />
                  <Route element={<PrivateRoute />}>
                    <Route path="/dashboard" element={ null } />
                    <Route path="/projects" element={ null } />
                    <Route path="/profile" element={ null} />
                  </Route>
                </Routes>
              </MainContent>
            </>
          )}
        </AppContainer>
      </AuthContext.Provider>
    </BrowserRouter>
  );
}

export default App;
