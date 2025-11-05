import { useState, useEffect } from 'react'
import { authService } from '../services/api/authService'

export const useAuth = () => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Проверяем авторизацию при загрузке приложения
    useEffect(() => {
        checkAuth();
    }, []);

    // Проверить авторизацию (профиль пользователя)
    const checkAuth = async () => {
        try {
            const response = await authService.getProfile();
            setUser(response.data);
        } catch (error) {
            setUser(null);
        } finally {
            setLoading(false);
        }
    };

    // Вход в систему
    const login = async (username, password) => {
        setLoading(true);
        setError(null);

        try {
            const response = await authService.login(username, password);
            setUser(response.data.user);
            return { success: true };
        } catch (error) {
            const errorMessage = Object.values(error.response.data.errors)[0];
            setError(errorMessage);
            return { success: false, error: errorMessage };
        } finally {
            setLoading(false);
        }
    };

    // Регистрация
    const register = async (username, password, fullName) => {
        setLoading(true);
        setError(null);

        try {
            const response = await authService.register(username, password, fullName);
            return { success: true, data: response.data };
        } catch (error) {
            const errorMessage = Object.values(error.response.data.errors)[0];
            setError(errorMessage);
            return { success: false, error: errorMessage };
        } finally {
            setLoading(false);
        }
    };

    // Выход из системы
    const logout = async () => {
        try {
            await authService.logout();
        } catch (error) {
            console.error('Ошибка при выходе:', error);
        } finally {
            setUser(null);
            setError(null);
        }
    };

    // Очистить ошибку
    const clearError = () => setError(null);

    return {
        // Состояния
        user,
        loading,
        error,
        isAuthenticated: !!user,

        // Методы
        login,
        register,
        logout,
        checkAuth,
        clearError
    };
};