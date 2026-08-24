import { User } from '../../types';

export const tokenStorage = {
  getToken: (): string | null => {
    return localStorage.getItem('auth_token');
  },

  setToken: (token: string): void => {
    localStorage.setItem('auth_token', token);
  },

  removeToken: (): void => {
    localStorage.removeItem('auth_token');
  },

  hasToken: (): boolean => {
    return !!localStorage.getItem('auth_token');
  },

  getUser: (): User | null => {
    const userJson = localStorage.getItem('auth_user');
    if (!userJson) return null;
    try {
      return JSON.parse(userJson);
    } catch {
      return null;
    }
  },

  setUser: (user: User): void => {
    localStorage.setItem('auth_user', JSON.stringify(user));
  },

  removeUser: (): void => {
    localStorage.removeItem('auth_user');
  },

  clear: (): void => {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
  },
};
