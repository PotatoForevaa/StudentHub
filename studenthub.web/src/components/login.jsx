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
            window.location.href = '/feed';
        } catch (error)
        {
            if (error.response) {
                console.error('Ошибка ответа:', error.response);
                alert('Неверные данные');
            } else if (error.request) {
                console.error('Ошибка запроса', error.request);
            } else {
                console.error('error', error.message)
            }
        } finally {
            setLoading(false);  
        }
    };

    return (
        <form onSubmit={handleLogin}>
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
        </form>
    );
};