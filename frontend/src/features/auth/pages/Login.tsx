import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthForm } from "../components/AuthForm";
import { useAuth } from "../hooks/useAuth";

export const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, formError, fieldErrors } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    await login(username, password);

    if (!formError && !fieldErrors[0]) {
      navigate("/dashboard");
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
