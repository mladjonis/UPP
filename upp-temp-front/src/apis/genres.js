import axios from "axios";

export const genresApi = axios.create({
  baseURL: "https://localhost:44343/api/Genres",
});
