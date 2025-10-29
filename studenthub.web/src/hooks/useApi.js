import { useState, useEffect } from 'react';

export const useApi = (apiCall, immediate = true) => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(immediate);
    const [error, setError] = useState(null);

    const execute = async (...params) => {
        setLoading(true);
        setError(null);

        try {
            const response = await apiCall(...params);
            setData(response.data);
            return response;
        } catch (err) {
            setError(err.response?.data || err.message);
            throw err;
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (immediate) {
            execute();
        }
    }, []);

    return { data, loading, error, execute, setData };
};