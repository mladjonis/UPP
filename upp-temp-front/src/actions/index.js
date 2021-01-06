import {
  SIGN_UP_ERROR,
  SIGN_UP_SUCCESS,
  FETCH_FORM_DATA_SUCCESS,
  FETCH_FORM_DATA_ERROR,
  FETCH_FORM_DATA,
  SUBMIT_FORM_DATA,
  GET_ALL_GENRES,
  START_PROCESS,
  LOGIN,
  EMAIL_SUBMIT,
  DOC_UPLOAD,
  GET_COMETEE_USERS,
} from "./types";
import { registrationApi } from "../apis/registration";
import { genresApi } from "../apis/genres";
import { fileApi } from "../apis/file";
import { cometeeApi } from "../apis/cometee";
import jwtDecode from "jwt-decode";
import { history } from "../history";

//fetch form data
export const fetchFormData = (processInstanceId) => async (dispatch) => {
  const response = await registrationApi.get("/GetFormData", {
    params: {
      ProcessId: processInstanceId,
    },
  });
  console.log(response);
  dispatch({ type: FETCH_FORM_DATA, payload: response.data });
};

export const submitWriterForm = (
  formListData,
  taskId,
  procInstanceId
) => async (dispatch) => {
  const response = await registrationApi.post("/RegisterUser", {
    SubmitFields: formListData,
    TaskId: taskId,
    ProcessInstanceId: procInstanceId,
  });
  console.log(response);
  dispatch({ type: SUBMIT_FORM_DATA, payload: response.data });
  history.push("/");
};

export const getGenres = () => async (dispatch) => {
  const response = await genresApi.get("/GetAll");

  dispatch({ type: GET_ALL_GENRES, payload: response.data });
};

export const startWriterProcess = () => async (dispatch) => {
  const response = await registrationApi.get("/StartWriterProcess");
  dispatch({ type: START_PROCESS, payload: response.data });
};

export const emailSubmit = (userId, token, processInstanceId) => async (
  dispatch
) => {
  console.log(userId, token, processInstanceId);
  const response = await registrationApi.get("/EmailConfirmation", {
    params: {
      userId: userId,
      token: token,
      processInstanceId: processInstanceId,
    },
  });
  console.log(response);
  dispatch({ type: EMAIL_SUBMIT, payload: response.data });
};

export const login = (loginDto) => async (dispatch) => {
  const response = await registrationApi.post("/Login", loginDto);

  console.log(response.data);
  console.log(response.data.token);

  if (response.data) {
    localStorage.setItem("token", response.data.token);
    localStorage.setItem("user", JSON.stringify(response.data.user));
  }
  dispatch({ type: LOGIN, payload: response.data });
};

export const logout = () => async (dispatch) => {
  console.log("usao u logout");
  await registrationApi.get("/Logout");
  localStorage.removeItem("token");
  localStorage.removeItem("user");
};

export const uploadDocuments = (documents, procId) => async (dispatch) => {
  let formData = new FormData();
  for (let i = 0; i < documents.length; i++) {
    const file = documents[i];
    formData.append("FormFiles", file);
  }
  formData.append("ProcessInstanceId", procId);
  await fileApi.post("/UploadDocuments", formData);
  dispatch({ type: DOC_UPLOAD });
  history.push("/");
};

export const submitCometeeForm = (
  formListData,
  taskId,
  procInstanceId
) => async (dispatch) => {
  const response = await cometeeApi.post("/SubmitCometeeForm", {
    SubmitFields: formListData,
    TaskId: taskId,
    ProcessInstanceId: procInstanceId,
  });
  dispatch({ type: SUBMIT_FORM_DATA, payload: response.data });
};

export const getCometeeUsers = () => async (dispatch) => {
  const response = await cometeeApi.get("/GetUsersToApprove");
  dispatch({ type: GET_COMETEE_USERS, payload: response.data });
};

export const dummy = () => async (dispatch) => {
  const response = await registrationApi.get("/dummy");
  console.log(response);
};
