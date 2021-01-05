import {
  FETCH_FORM_DATA_SUCCESS,
  FETCH_FORM_DATA_ERROR,
  FETCH_FORM_DATA,
  SUBMIT_FORM_DATA,
  START_PROCESS,
  EMAIL_SUBMIT,
  LOGIN,
  DOC_UPLOAD,
} from "../actions/types";

const initState = {
  formData: null,
  error: null,
  registrationResponse: null,
  processInstanceId: null,
  emailConfirmation: null,
  auth: null,
};

const formReducer = (state = initState, action) => {
  switch (action.type) {
    case FETCH_FORM_DATA:
      return { ...state, formData: action.payload };
    case FETCH_FORM_DATA_ERROR:
      return { ...state, error: action.error };
    case SUBMIT_FORM_DATA:
      return { ...state, registrationResponse: action.payload };
    case START_PROCESS:
      console.log(action.payload);
      console.log(state);
      return { ...state, processInstanceId: action.payload };
    case EMAIL_SUBMIT:
      return { ...state, emailConfirmation: action.payload };
    case LOGIN:
      return { ...state, auth: action.payload };
    case DOC_UPLOAD:
      return state;
    default:
      return state;
  }
};

export default formReducer;
