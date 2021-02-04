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
  FETCH_BETA_FORM_DATA,
  FETCH_PAYMENT,
} from "./types";
import { registrationApi } from "../apis/registration";
import { genresApi } from "../apis/genres";
import { fileApi } from "../apis/file";
import { cometeeApi } from "../apis/cometee";
import { paymentApi } from "../apis/payment";
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

export const submitReaderForm = (
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

export const submitReaderForm2 = (
  formListData,
  taskId,
  procInstanceId
) => async (dispatch) => {
  const response = await registrationApi.post("/RegisterReader", {
    SubmitFields: formListData,
    TaskId: taskId,
    ProcessInstanceId: procInstanceId,
  });
  dispatch({ type: SUBMIT_FORM_DATA, payload: response.data });
};

export const submitBetaReaderForm = (
  formListData,
  taskId,
  procInstanceId
) => async (dispatch) => {
  const response = await registrationApi.post("/RegisterBetaReader", {
    SubmitFields: formListData,
    TaskId: taskId,
    ProcessInstanceId: procInstanceId,
  });
  dispatch({ type: SUBMIT_FORM_DATA, payload: response.data });
};

export const getGenres = () => async (dispatch) => {
  const response = await genresApi.get("/GetAll");

  dispatch({ type: GET_ALL_GENRES, payload: response.data });
};

export const startWriterProcess = () => async (dispatch) => {
  const response = await registrationApi.get("/StartWriterProcess");
  dispatch({ type: START_PROCESS, payload: response.data });
};

export const startReaderProcess = () => async (dispatch) => {
  const response = await registrationApi.get("/StartReaderProcess");
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
  if (response.data.user.userRoles.find((x) => x.role.name === "Writer")) {
    history.push("/upload");
  } else {
    history.push("/");
  }
};

export const logout = () => async (dispatch) => {
  console.log("usao u logout");
  localStorage.removeItem("token");
  localStorage.removeItem("user");
  history.push("/");
  await registrationApi.post("/Logout");
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

export const fetchFormDataCometee = (procId, taskNameOrId) => async (
  dispatch
) => {
  const response = await cometeeApi.get("/GetFormData", {
    params: {
      ProcessInstanceId: procId,
      TaskNameOrId: taskNameOrId,
    },
  });
  dispatch({ type: FETCH_BETA_FORM_DATA, payload: response.data });
};

export const fetchBetaReadersFormData = (procId, taskNameOrId) => async (
  dispatch
) => {
  const response = await registrationApi.get("/GetGenresFormData", {
    params: {
      ProcessInstanceId: procId,
      TaskNameOrId: taskNameOrId,
    },
  });
  dispatch({ type: FETCH_BETA_FORM_DATA, payload: response.data });
};

export const fetchGenericFormData = (procId, taskNameOrId) => async (
  dispatch
) => {
  const response = await registrationApi.get("/GetGenericFormData", {
    params: {
      ProcessInstanceId: procId,
      TaskNameOrId: taskNameOrId,
    },
  });
  dispatch({ type: FETCH_FORM_DATA, payload: response.data });
};

export const submitPayment = (formListData, procId, taskNameOrId) => async (
  dispatch
) => {
  const response = await paymentApi.get("/PaymentRequest", {
    params: {
      SubmitFields: formListData,
      ProcessInstanceId: procId,
      TaskNameOrId: taskNameOrId,
    },
  });
  dispatch({ type: FETCH_PAYMENT, payload: response.data });
};

export const dummy = () => async (dispatch) => {
  const response = await registrationApi.get("/dummy");
  console.log(response);
};
