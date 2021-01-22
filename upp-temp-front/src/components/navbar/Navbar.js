import React from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { logout } from "../../actions";
import { getRole, getToken, getUser } from "../../token";

const Navbar = (props) => {
  const loggedIn = (auth) => {
    if (!auth && !getToken()) {
      return (
        <React.Fragment>
          <NavLink className="nav-item nav-link" to="/registration">
            Register
          </NavLink>
          <NavLink className="nav-item nav-link" to="/login">
            Login
          </NavLink>
        </React.Fragment>
      );
    } else {
      if (getUser() && getRole("Writer")) {
        return (
          <React.Fragment>
            <NavLink className="nav-item nav-link" to="/">
              Welocome {getUser().userName}
            </NavLink>
            <NavLink className="nav-item nav-link" to="/payment">
              Go to payment
            </NavLink>
            <NavLink className="nav-item nav-link" to="/upload">
              Upload documents
            </NavLink>
            <NavLink
              className="nav-item nav-link"
              to="/"
              onClick={props.logout}
            >
              Logout
            </NavLink>
          </React.Fragment>
        );
      } else if (getUser() && getRole("Cometee")) {
        return (
          <React.Fragment>
            <NavLink className="nav-item nav-link" to="/">
              Welocome {getUser().userName}
            </NavLink>
            <NavLink className="nav-item nav-link" to="/cometee">
              Go to cometee tab
            </NavLink>
            <NavLink
              className="nav-item nav-link"
              to="/"
              onClick={props.logout}
            >
              Logout
            </NavLink>
          </React.Fragment>
        );
      }
    }
  };
  return (
    <nav
      className="navbar navbar-expand-lg navbar-light bg-light"
      style={{ backgroundColor: "#e3f2fd" }}
    >
      <NavLink className="navbar-brand" to="/">
        Literarno udruzenje UPP
      </NavLink>
      <div className="nav navbar-nav ml-auto">
        <NavLink className="nav-item nav-link" to="/">
          Home
        </NavLink>
        {loggedIn(props.auth)}
      </div>
    </nav>
  );
};

const mapStateToProps = (state) => {
  return {
    auth: state.form.auth,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    logout: () => dispatch(logout()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Navbar);
