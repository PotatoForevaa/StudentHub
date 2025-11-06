import { Login } from "./login.jsx";
import { Header } from "./header.jsx";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Registration } from "./registration.jsx";
import { AuthForm } from "./authForm.jsx";
import { styled } from "styled-components";
import { createGlobalStyle } from "styled-components";

function App() {
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
    background: #4ECDC4;
  `;

  return (
    <>
      <GlobalStyles />
      <AppContainer>
        <Header />
        <MainContent>
          <Routes>
            <Route path="/" element={<Login />} />
            <Route path="login" element={<Login />} />
            <Route path="registration" element={<Registration />} />
            <Route path="auth" element={<AuthForm />} />
          </Routes>
        </MainContent>
      </AppContainer>
    </>
  );
}

export default App;
