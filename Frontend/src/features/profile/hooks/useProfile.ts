import { useEffect, useState, useContext } from 'react';
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
  const { user: currentUser, picture } = useContext(AuthContext);
  const [targetUser, setTargetUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [targetPicture, setTargetPicture] = useState<string | null>(null);
  const [pictureLoading, setPictureLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    setError(null);

    if (username) {
      if (currentUser && currentUser.username === username) {
        setTargetUser(currentUser);
        setLoading(false);
      } else {
        userService.getByUsername(username)
          .then(res => {
            if (res?.isSuccess && res.data) {
              setTargetUser(res.data);
            } else {
              setError(res?.errors?.[0]?.message || 'Пользователь не найден');
              setTargetUser(null);
            }
          })
          .catch(error => {
            console.error('Failed to fetch user data:', error);
            setError('Ошибка загрузки профиля');
            setTargetUser(null);
          })
          .finally(() => {
            setLoading(false);
          });
      }
    } else {
      setTargetUser(currentUser);
      setLoading(false);
    }
  }, [username, currentUser]);

  const user = targetUser;


  useEffect(() => {
    if (user && user !== currentUser) {
      setPictureLoading(true);
      userService.getProfilePicture(user.username)
        .then(response => {
          if (response.ok) {
            response.blob().then(blob => {
              const url = URL.createObjectURL(blob);
              setTargetPicture(url);
            });
          } else {
            setTargetPicture(null);
          }
        })
        .catch(error => {
          console.error('Failed to fetch profile picture', error);
          setTargetPicture(null);
        })
        .finally(() => {
          setPictureLoading(false);
        });
    } else {
      setTargetPicture(null);
      setPictureLoading(false);
    }
  }, [user, currentUser]);

  return {
    user,
    picture: user === currentUser ? (picture || null) : targetPicture,
    loading,
    error,
    pictureLoading
  };
}
