import { Login, Registration } from "../features/auth/pages";
import { Header } from "./Header";
import { BrowserRouter, Routes, Route } from "react-router-dom";
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
    background: #0c0032;
  `;

  return (
    <BrowserRouter>
      <GlobalStyles />
      <AppContainer>
        <Header />
        <MainContent>
          <Routes>
            <Route path="/" element={<Login />} />
            <Route path="/login" element={<Login />} />
            <Route path="/registration" element={<Registration />} />
          </Routes>
        </MainContent>
      </AppContainer>
    </BrowserRouter>
  );
}

export default App;
