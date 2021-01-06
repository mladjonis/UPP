import axios from "axios";

export const cometeeApi = axios.create({
  baseURL: "https://localhost:44343/api/Cometee",
});

cometeeApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  config.headers = {
    Authorization: `Bearer ${token}`,
  };
  return config;
});
