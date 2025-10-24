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
            console.log('�������� ����:', response.data);
            window.location.href = '/dashboard';
        } catch (error) {
            console.error('������ �����:', error.response?.data);
            alert('�������� ������');
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleLogin}>
            <input
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="�����"
            />
            <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="������"
            />
            <button disabled={loading}>
                {loading ? '����...' : '�����'}
            </button>
        </form>
    );
};