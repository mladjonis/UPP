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
  let user = localStorage.getItem("user");
  if (user) {
    user = JSON.parse(user);
    const userRole = user["userRoles"].find((x) => x.role.name === role);
    return userRole;
  }
  return null;
};

export const getToken = () => {
  const token = localStorage.getItem("token");
  if (token && !isExpired(token)) {
    return token;
  }
  return null;
};

export const getUser = () => {
  const user = localStorage.getItem("user");
  if (user) {
    return JSON.parse(user);
  }
  return null;
};
