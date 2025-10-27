import { Login } from './Login.jsx'
import { Header } from './Header.jsx'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Registration } from './Registration.jsx'

function App() {
  
  return (
      <>
          <Header />
          <Routes>
              <Route path="/" element={<Login />} />
              <Route path="login" element={<Login />} />
              <Route path="registration" element={<Registration />} />
          </Routes>
      </>
  )
}

export default App
