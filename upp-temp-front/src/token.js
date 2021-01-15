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

export const getRole = (role) => {
  const user = JSON.parse(localStorage.getItem("user"));
  console.log();
  console.log(user.userRoles);
  console.log(user["userRoles"]);
  const userRole = user["userRoles"].find((x) => x.role.name === role);
  return userRole;
};

export const getToken = () => {
  const token = JSON.parse(localStorage.getItem("token"));
  if (!isExpired(token)) {
    return token;
  }
  return null;
};

export const getUser = () => {
  return JSON.parse(localStorage.getItem("user"));
};
