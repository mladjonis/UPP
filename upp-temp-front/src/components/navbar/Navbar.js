import React from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { logout } from "../../actions";

const Navbar = (props) => {
  const loggedIn = (auth) => {
    if (!auth) {
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
      return (
        <NavLink className="nav-item nav-link" to="/" onClick={props.logout}>
          Logout
        </NavLink>
      );
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
