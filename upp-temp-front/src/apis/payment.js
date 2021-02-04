import axios from "axios";

export const fileApi = axios.create({
  baseURL: "https://localhost:44343/api/Payment",
});

fileApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  config.headers = {
    Authorization: `Bearer ${token}`,
  };
  return config;
});
