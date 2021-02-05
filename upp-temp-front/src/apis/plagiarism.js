import axios from "axios";

export const plagiarismApi = axios.create({
  baseURL: "https://localhost:44343/api/Plagiarism",
});

plagiarismApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  config.headers = {
    Authorization: `Bearer ${token}`,
  };
  return config;
});
