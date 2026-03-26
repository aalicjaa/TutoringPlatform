import axios from "axios";

const baseURL = import.meta.env.VITE_API_URL;

export const tokenStorage = {
  get: () => localStorage.getItem("auth_token"),
  set: (token: string) => localStorage.setItem("auth_token", token),
  clear: () => localStorage.removeItem("auth_token"),
};

export const http = axios.create({
  baseURL,
});

http.interceptors.request.use((config) => {
  const token = tokenStorage.get();
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
