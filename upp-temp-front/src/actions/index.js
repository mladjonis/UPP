import {
  SIGN_UP_ERROR,
  SIGN_UP_SUCCESS,
  FETCH_FORM_DATA_SUCCESS,
  FETCH_FORM_DATA_ERROR,
  FETCH_FORM_DATA,
} from "./types";
import { registrationApi } from "../apis/registration";

//fetch form data
export const fetchFormData = () => async (dispatch) => {
  const response = await registrationApi.get("/GetFormData");
  console.log(response);
  dispatch({ type: FETCH_FORM_DATA, payload: response.data });
};
