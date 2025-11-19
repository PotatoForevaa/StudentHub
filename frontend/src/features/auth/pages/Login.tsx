import { useContext, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { AuthForm } from "../components/AuthForm";
import { useAuth } from "../hooks/useAuth";
import { AuthContext } from "../../../shared/context/AuthContext";

export const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, formError, fieldErrors } = useAuth();
  const { setAuth } = useContext(AuthContext);
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    const success = await login(username, password);
    if (success) {
      setAuth(true);
      navigate(from);
    };
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
          name: "Username",
          type: "text",
          placeholder: "Введите логин",
          onChange: (e) => setUsername(e.target.value),
        },
        {
          displayName: "Пароль",
          name: "Password",
          type: "password",
          placeholder: "Введите пароль",
          onChange: (e) => setPassword(e.target.value),
        },
      ]}
    />
  );
};
