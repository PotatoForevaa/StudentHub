import { useState } from "react";
import { authService } from "../services/api/authService";

export const Registration = () => {
  const [username, setUsername] = useState("");
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");

  const handleRegistration = async (e) => {
    e.preventDefault();
    await authService.register(username, password, fullName);
  };

  return (
    <form onSubmit={handleRegistration}>
      <label>Имя пользователя</label>
      <input
        value={username}
        onChange={(e) => setUsername(e.target.value)}
      ></input>
      <label>ФИО</label>
      <input
        value={fullName}
        onChange={(e) => setFullName(e.target.value)}
      ></input>
      <label>Введите пароль</label>
      <input
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      ></input>
    </form>
  );
};
