import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AuthForm } from "./AuthForm";
import { useAuth } from "../../hooks/useAuth";

export const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, error } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    const result = await login(username, password);

    if (result) {
      navigate("/dashboard");
    } else {
      alert(error);
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
          onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
            setUsername(e.target.value),
        },
        {
          name: "Пароль",
          type: "password",
          placeholder: "Введите пароль",
          onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
            setPassword(e.target.value),
        },
      ]}
    />
  );
};
