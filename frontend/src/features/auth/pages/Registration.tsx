import { useState, type ChangeEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { AuthForm } from "../components/AuthForm";

export const Registration = () => {
  const [username, setUsername] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleRegistration = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const result = await register(username, password, fullName);

    if (result.success) {
      navigate("/dashboard");
    } else {
      alert(result.error);
    }
  };

  return (
    <AuthForm
      buttonText="Зарегистрироваться"
      onSubmit={handleRegistration}
      fields={[
        {
          name: "Логин",
          type: "text",
          placeholder: "Введите логин",
          onChange: (e: ChangeEvent<HTMLInputElement>) => setUsername(e.target.value),
        },
        {
          name: "ФИО",
          type: "text",
          placeholder: "Введите ФИО",
          onChange: (e: ChangeEvent<HTMLInputElement>) => setFullName(e.target.value),
        },
        {
          name: "Пароль",
          type: "password",
          placeholder: "Введите пароль",
          onChange: (e: ChangeEvent<HTMLInputElement>) => setPassword(e.target.value),
        },
      ]}
    />
  );
};
