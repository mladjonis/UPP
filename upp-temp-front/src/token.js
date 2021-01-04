import jwtDecode from "jwt-decode";

export const decodedToken = (token) => {
  return jwtDecode(token);
};

export const isExpired = (token) => {
  if (token && jwtDecode(token)) {
    const expiry = jwtDecode(token).exp;
    const now = new Date();
    return now.getTime() > expiry * 1000;
  }
  return false;
};
