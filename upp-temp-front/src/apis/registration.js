import axios from "axios";

export const registrationApi = axios.create({
  baseURL: "https://localhost:44343/api/Registration",
});
