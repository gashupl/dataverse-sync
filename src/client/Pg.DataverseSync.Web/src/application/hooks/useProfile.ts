import { useState, useEffect } from 'react';
import { AuthService } from '../services/authService';
import type { User } from '../../domain/entities/auth';

const authService = new AuthService();

export function useProfile() {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const currentUser = await authService.getCurrentUser();
        setUser(currentUser);
      } catch {
        setError('Failed to load profile data.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchProfile();
  }, []);

  return { user, isLoading, error };
}
