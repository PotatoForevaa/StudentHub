import { useState, useContext } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { AuthForm } from "../components/AuthForm";
import { useAuthForm } from "../hooks/useAuthForm";
import { AuthContext } from "../context/AuthContext";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const { login } = useContext(AuthContext);
  const { handleError, fieldErrors, formError, resetErrors, setLoading } = useAuthForm();

  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    resetErrors();
    setLoading(true);

    try {
      const success = await login(username, password);
      if (success) {
        navigate(from);
      } 
    } catch (err) {
      handleError(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthForm
      buttonText="Войти"
      onSubmit={handleLogin}
      fieldErrors={fieldErrors}
      formError={formError}
      fields={[
        {
          displayName: "Имя пользователя",
          name: "username",
          type: "text",
          placeholder: "Введите логин",
          onChange: (e) => setUsername(e.target.value),
        },
        {
          displayName: "Пароль",
          name: "password",
          type: "password",
          placeholder: "Введите пароль",
          onChange: (e) => setPassword(e.target.value),
        },
      ]}
    />
  );
};

export default Login;
