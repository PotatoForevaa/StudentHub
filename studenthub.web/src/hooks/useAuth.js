import { useState, useEffect } from 'react'
import { authService } from '../services/api/authService'

export const useAuth = () => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // ��������� ����������� ��� �������� ����������
    useEffect(() => {
        checkAuth();
    }, []);

    // ��������� ����������� (������� ������������)
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

    // ���� � �������
    const login = async (username, password) => {
        setLoading(true);
        setError(null);

        try {
            const response = await authService.login(username, password);
            setUser(response.data.user);
            return { success: true };
        } catch (error) {
            const errorMessage = error.response?.data?.message || '������ �����';
            setError(errorMessage);
            return { success: false, error: errorMessage };
        } finally {
            setLoading(false);
        }
    };

    // �����������
    const register = async (username, password, fullName) => {
        setLoading(true);
        setError(null);

        try {
            const response = await authService.register(username, password, fullName);
            return { success: true, data: response.data };
        } catch (error) {
            const errorMessage = error.response?.data?.message || '������ �����������';
            setError(errorMessage);
            return { success: false, error: errorMessage };
        } finally {
            setLoading(false);
        }
    };

    // ����� �� �������
    const logout = async () => {
        try {
            await authService.logout();
        } catch (error) {
            console.error('������ ��� ������:', error);
        } finally {
            setUser(null);
            setError(null);
        }
    };

    // �������� ������
    const clearError = () => setError(null);

    return {
        // ���������
        user,
        loading,
        error,
        isAuthenticated: !!user,

        // ������
        login,
        register,
        logout,
        checkAuth,
        clearError
    };
};