import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Suspense, lazy } from 'react';
import { styled, createGlobalStyle } from "styled-components";
import { Header } from "./Header";
import { PrivateRoute } from "../shared/components/PrivateRoute";
import { AppProvider } from "../shared/components/AppProvider";
import { ErrorBoundary } from "../shared/components/ErrorBoundary";
import { LoadingSpinner } from "../shared/components/LoadingSpinner";
import { colors, fonts } from "../shared/styles/tokens";

const Login = lazy(() => import("../features/auth/pages/Login"));
const Registration = lazy(() => import("../features/auth/pages/Registration"));
const Profile = lazy(() => import("../features/profile/pages/profile"));
const Projects = lazy(() => import("../features/projects/pages/projects"));
const ProjectDetail = lazy(() => import("../features/projects/pages/projectDetail"));
const UserList = lazy(() => import("../features/users/pages/userlist"));

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
    <ErrorBoundary>
      <BrowserRouter>
        <AppProvider>
          <GlobalStyles />
          <AppContainer>
            <Header />
            <MainContent>
              <Suspense fallback={<LoadingSpinner text="Загрузка страницы..." size="lg" />}>
                <Routes>
                  <Route path="/login" element={<Login />} />
                  <Route path="/registration" element={<Registration />} />

                  <Route element={<PrivateRoute />}>
                    <Route path="/dashboard" element={null} />
                    <Route path="/" element={<Profile />} />
                    <Route path="/projects" element={<Projects />} />
                    <Route path="/projects/:id" element={<ProjectDetail />} />
                    <Route path="/users" element={<UserList />} />
                    <Route path="/:username" element={<Profile />} />
                  </Route>
                </Routes>
              </Suspense>
            </MainContent>
          </AppContainer>
        </AppProvider>
      </BrowserRouter>
    </ErrorBoundary>
  );
}

export default App;
