import axios from "axios";

export const paymentApi = axios.create({
  baseURL: "https://localhost:44343/api/Payment",
});

paymentApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  config.headers = {
    Authorization: `Bearer ${token}`,
  };
  return config;
});
