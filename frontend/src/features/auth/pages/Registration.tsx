import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { AuthForm } from "../components/AuthForm";

export const Registration = () => {
  const [username, setUsername] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const { register, fieldErrors, formError } = useAuth();
  const navigate = useNavigate();

  const handleRegistration = async (e: React.FormEvent) => {
    e.preventDefault();
    const result = await register(fullName, username, password);

    if (result) {
      navigate("/dashboard");
    };
  };

  return (
    <AuthForm
      buttonText="Зарегистрироваться"
      onSubmit={handleRegistration}      
      fieldErrors={fieldErrors}
      formError={formError}
      fields={[
        {
          displayName: "Имя пользователя",
          name: "Username",
          type: "text",
          placeholder: "Введите имя пользователя",
          onChange: (e) => setUsername(e.target.value),
        },
        {
          displayName: "ФИО",
          name: "FullName",
          type: "text",
          placeholder: "Введите ФИО",
          onChange: (e) => setFullName(e.target.value),
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
