import { useEffect, useState, useContext, useMemo } from 'react';
import { AuthContext } from '../../auth/context/AuthContext';
import userService from '../../../shared/services/userService';
import type { User } from '../../../shared/types';

export interface UseProfileReturn {
  user: User | null;
  picture: string | null;
  loading: boolean;
  error: string | null;
  pictureLoading: boolean;
}

export function useProfile(username?: string): UseProfileReturn {
  const { user: currentUser, picture: authPicture } = useContext(AuthContext);

  const [targetUser, setTargetUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [targetPicture, setTargetPicture] = useState<string | null>(null);
  const [pictureLoading, setPictureLoading] = useState(false);

  // ✅ FIX: стабильное вычисление через username, а не объект user
  const isOwnProfile = useMemo(() => {
    if (!username || !currentUser?.username) return false;
    return currentUser.username === username;
  }, [username, currentUser?.username]);

  // ---------------- USER LOAD ----------------
  useEffect(() => {
    setLoading(true);
    setError(null);

    if (!username) {
      setTargetUser(currentUser);
      setLoading(false);
      return;
    }

    if (currentUser?.username === username) {
      setTargetUser(currentUser);
      setLoading(false);
      return;
    }

    userService.getByUsername(username)
      .then(res => {
        if (res?.isSuccess && res.data) {
          setTargetUser(res.data);
        } else {
          setError(res?.errors?.[0]?.message || 'Пользователь не найден');
          setTargetUser(null);
        }
      })
      .catch(err => {
        console.error('Failed to fetch user data:', err);
        setError('Ошибка загрузки профиля');
        setTargetUser(null);
      })
      .finally(() => {
        setLoading(false);
      });

  }, [username, currentUser]);

  const user = targetUser;

  // ---------------- PICTURE LOAD ----------------
  useEffect(() => {
    if (!user) return;

    // own profile → берём из AuthContext
    if (isOwnProfile) {
      setTargetPicture(null);
      setPictureLoading(false);
      return;
    }

    setPictureLoading(true);

    userService.getProfilePicture(user.username)
      .then(response => {
        if (!response.ok) return null;
        return response.blob();
      })
      .then(blob => {
        if (!blob) {
          setTargetPicture(null);
          return;
        }

        const url = URL.createObjectURL(blob);
        setTargetPicture(url);
      })
      .catch(err => {
        console.error('Failed to fetch profile picture', err);
        setTargetPicture(null);
      })
      .finally(() => {
        setPictureLoading(false);
      });

  }, [user, isOwnProfile]);

  // ---------------- RETURN ----------------
  return {
    user,

    // ✅ FIX: всегда есть fallback логика
    picture: isOwnProfile
      ? (authPicture || null)
      : (targetPicture || null),

    loading,
    error,
    pictureLoading
  };
}