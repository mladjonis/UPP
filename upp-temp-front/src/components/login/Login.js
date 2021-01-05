import React from "react";
import { Redirect } from "react-router-dom";
import { connect } from "react-redux";
import { login } from "../../actions";

//napraviti novi proces da se isproba ovo sa komisijom
//namestiti claimovanje taskova komisije
//napraviti za njih poseban endpoint na front u i backu da vraca podatke koje treba sta znam koje...

class Login extends React.Component {
  state = {
    password: "",
    email: "",
  };
  handleChange = (e) => {
    this.setState({
      [e.target.id]: e.target.value,
    });
  };
  onSubmit = (e) => {
    e.preventDefault();
    this.props.login(this.state);
  };
  render() {
    if (this.props.auth) {
      <Redirect to="/" />;
    }
    return (
      <div className="container text-center">
        <form className="form-signin" onSubmit={this.onSubmit}>
          <h1 className="h3 mb-3 font-weight-normal">Please sign in</h1>
          <label htmlFor="email" className="sr-only">
            Email address
          </label>
          <input
            type="email"
            id="email"
            className="form-control"
            placeholder="Email address"
            onChange={this.handleChange}
            required
            autoFocus
          />
          <label htmlFor="password" className="sr-only">
            Password
          </label>
          <input
            type="password"
            id="password"
            className="form-control"
            placeholder="Password"
            onChange={this.handleChange}
            required
          />
          <div className="checkbox mb-3">
            <label>
              <input type="checkbox" value="remember-me" /> Remember me
            </label>
          </div>
          <button className="btn btn-lg btn-primary btn-block" type="submit">
            Sign in
          </button>
          <p className="mt-5 mb-3 text-muted">© ASA 2020 KRAJ JE BLIZU</p>
        </form>
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    auth: state.form.auth,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    login: (credentials) => dispatch(login(credentials)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Login);
