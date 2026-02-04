import { useState, useContext } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useAuthForm } from "../hooks/useAuthForm";
import { AuthForm } from "../components/AuthForm";
import { AuthContext } from "../context/AuthContext";

const Registration = () => {
  const [username, setUsername] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");

  const { register } = useContext(AuthContext);
  const { handleError, fieldErrors, formError, resetErrors, setLoading } = useAuthForm();

  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";
  
  const handleRegistration = async (e: React.FormEvent) => {
    e.preventDefault();
    resetErrors();
    setLoading(true);

    try {
      const success = await register(fullName, username, password);
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
      buttonText="Зарегистрироваться"
      onSubmit={handleRegistration}
      fieldErrors={fieldErrors}
      formError={formError}
      fields={[
        {
          displayName: "Имя пользователя",
          name: "username",
          type: "text",
          placeholder: "Введите имя пользователя",
          onChange: (e) => setUsername(e.target.value),
        },
        {
          displayName: "ФИО",
          name: "fullName",
          type: "text",
          placeholder: "Введите ФИО",
          onChange: (e) => setFullName(e.target.value),
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

export default Registration
