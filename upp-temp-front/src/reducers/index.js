import { combineReducers } from "redux";
import formReducer from "./formReducer";
import genresReducer from "./genresReducer";

export default combineReducers({
  form: formReducer,
  genre: genresReducer,
});
