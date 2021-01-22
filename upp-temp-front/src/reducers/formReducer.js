import {
  FETCH_FORM_DATA_SUCCESS,
  FETCH_FORM_DATA_ERROR,
  FETCH_FORM_DATA,
  SUBMIT_FORM_DATA,
  START_PROCESS,
  EMAIL_SUBMIT,
  LOGIN,
  DOC_UPLOAD,
  SUBMIT_COMETEE_FORM_DATA,
  GET_COMETEE_USERS,
  FETCH_BETA_FORM_DATA,
} from "../actions/types";

const initState = {
  formData: null,
  error: null,
  registrationResponse: null,
  processInstanceId: null,
  emailConfirmation: null,
  cometeeRespone: null,
  auth: null,
  cometeeUsers: null,
  beta: null,
};

const formReducer = (state = initState, action) => {
  switch (action.type) {
    case FETCH_FORM_DATA:
      return { ...state, formData: action.payload };
    case FETCH_FORM_DATA_ERROR:
      return { ...state, error: action.error };
    case SUBMIT_FORM_DATA:
      return { ...state, registrationResponse: action.payload };
    case SUBMIT_COMETEE_FORM_DATA:
      return { ...state, cometeeRespone: action.payload };
    case START_PROCESS:
      return { ...state, processInstanceId: action.payload };
    case EMAIL_SUBMIT:
      return { ...state, emailConfirmation: action.payload };
    case GET_COMETEE_USERS:
      return { ...state, cometeeUsers: action.payload };
    case LOGIN:
      return { ...state, auth: action.payload };
    case FETCH_BETA_FORM_DATA:
      return { ...state, beta: action.payload };
    case DOC_UPLOAD:
      return state;
    default:
      return state;
  }
};

export default formReducer;
