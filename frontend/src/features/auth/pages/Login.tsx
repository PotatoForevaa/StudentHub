import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthForm } from "../components/AuthForm";
import { useAuth } from "../hooks/useAuth";

export const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, logout, loading, formError, fieldErrors} = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    const result = await login(username, password);

    if (result) {
      navigate("/dashboard");
    } else {
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
          name: "Username",
          type: "text",
          placeholder: "Введите логин",
          onChange: (e) => setUsername(e.target.value),
        },
        {
          name: "Password",
          type: "password",
          placeholder: "Введите пароль",
          onChange: (e) => setPassword(e.target.value),
        },
      ]}
    />

  );
};
