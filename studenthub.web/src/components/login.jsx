import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import { styled } from 'styled-components';


const Form = styled.form`
     display: flex;
     justify-content: center;
     align-items: center;
`;

export const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const { login, loading } = useAuth();
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        const result = await login(username, password);

        if (result.success) {
            navigate('/dashboard');
        } else {
            alert(result.error)
        }     

    };

    return (
        <Form onSubmit={handleLogin}>
            <label>Логин</label>
            <input
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Логин"
            />
            <label>Пароль</label>
            <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Пароль"
            />
            <button disabled={loading}>
                {loading ? 'Вход...' : 'Войти'}
            </button>
        </Form>
    );
};