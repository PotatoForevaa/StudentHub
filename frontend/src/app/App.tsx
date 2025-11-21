import { BrowserRouter, Routes, Route } from "react-router-dom";
import { styled, createGlobalStyle } from "styled-components";
import { Login, Registration } from "../features/auth/pages";
import { Header } from "./Header";
import { PrivateRoute } from "../shared/components/PrivateRoute";
import { AuthProvider } from "../shared/context/AuthContext";

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
  return (
    <BrowserRouter>
      <AuthProvider>
        <GlobalStyles />
        <AppContainer>
          <Header />

          <MainContent>
            <Routes>
              <Route path="/login" element={<Login />} />
              <Route path="/registration" element={<Registration />} />

              <Route element={<PrivateRoute />}>
                <Route path="/dashboard" element={null} />
                <Route path="/projects" element={null} />
                <Route path="/profile" element={null} />
              </Route>
            </Routes>
          </MainContent>
        </AppContainer>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
