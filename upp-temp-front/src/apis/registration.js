import axios from "axios";

export const registrationApi = axios.create({
  baseURL: "https://localhost:44343/api/Registration",
});

registrationApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers = {
      Authorization: `Bearer ${token}`,
    };
  }
  return config;
});
