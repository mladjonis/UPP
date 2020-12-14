import {
  FETCH_FORM_DATA_SUCCESS,
  FETCH_FORM_DATA_ERROR,
  FETCH_FORM_DATA,
} from "../actions/types";

const initState = {
  formData: null,
  error: null,
};

const formReducer = (state = initState, action) => {
  switch (action.type) {
    case FETCH_FORM_DATA:
      return { ...state, formData: action.payload };
    case FETCH_FORM_DATA_ERROR:
      return { ...state, error: action.error };
    default:
      return state;
  }
};

export default formReducer;
