import axios from "axios";

export const booksApi = axios.create({
  baseURL: "https://localhost:44343/api/Books",
});

fileApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  config.headers = {
    Authorization: `Bearer ${token}`,
  };
  return config;
});
