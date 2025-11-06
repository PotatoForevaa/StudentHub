import { useState } from "react";
import { useAuth } from "../hooks/useAuth";
import { useNavigate } from "react-router-dom";
import { styled } from "styled-components";
import { AuthForm } from "./authForm";

export const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, loading } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    const result = await login(username, password);

    if (result.success) {
      navigate("/dashboard");
    } else {
      alert(result.error);
    }
  };

  return (
    <AuthForm
      buttonText="Войти"
      onSubmit={handleLogin}
      fields={[
        {
          name: "Логин",
          type: "text",
          placeholder: "Введите логин",
          onChange: (e) => setUsername(e.target.value),
        },
        {
          name: "Пароль",
          type: "password",
          placeholder: "Введите пароль",
          onChange: (e) => setPassword(e.target.value),
        },
      ]}
    />
  );
};
