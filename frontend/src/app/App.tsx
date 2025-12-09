import { BrowserRouter, Routes, Route } from "react-router-dom";
import { styled, createGlobalStyle } from "styled-components";
import { Login, Registration } from "../features/auth/pages";
import { Header } from "./Header";
import { PrivateRoute } from "../shared/components/PrivateRoute";
import { AuthProvider } from "../features/auth/context/AuthContext";
import { Profile } from "../features/profile/pages/profile";
import { Projects } from "../features/projects/pages/projects";
import { ProjectDetail } from "../features/projects/pages/projectDetail";
import { ProjectProvider } from "../features/projects/context/ProjectContext";
import { colors, fonts } from "../shared/styles/tokens";

const GlobalStyles = createGlobalStyle`
  html, body, #root {
    height: 100%;
  }

  * { box-sizing: border-box }

  body {
    margin: 0;
    padding: 0;
    font-family: ${fonts.family};
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    background: linear-gradient(180deg, ${colors.bg} 0%, ${colors.white} 40%);
    color: ${colors.textPrimary};
  }

  a { color: ${colors.primary}; }

  input::placeholder, textarea::placeholder { color: ${colors.placeholder} }

  button { font-family: inherit }
`;

const AppContainer = styled.div`
  min-height: 100vh;
  display: flex;
  flex-direction: column;
`;

const MainContent = styled.main`
  flex: 1;
  display: flex;
  align-items: stretch;
  background: ${colors.white};
  justify-content: center;
`;

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ProjectProvider>
          <GlobalStyles />
          <AppContainer>
            <Header />
            <MainContent>
              <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/registration" element={<Registration />} />

                <Route element={<PrivateRoute />}>
                  <Route path="/dashboard" element={null} />
                  <Route path="/projects" element={<Projects />} />
                  <Route path="/projects/:id" element={<ProjectDetail />} />
                  <Route path="/profile" element={<Profile />} />
                </Route>
              </Routes>
            </MainContent>
          </AppContainer>
        </ProjectProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
