import { useState } from 'react';
import { authService } from '../services/api/authService';

export const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await authService.login(username, password);
            console.log('Успешный вход:', response.data);
            window.location.href = '/dashboard';
        } catch (error) {
            console.error('Ошибка входа:', error.response?.data);
            alert('Неверные данные');
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleLogin}>
            <input
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Логин"
            />
            <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Пароль"
            />
            <button disabled={loading}>
                {loading ? 'Вход...' : 'Войти'}
            </button>
        </form>
    );
};