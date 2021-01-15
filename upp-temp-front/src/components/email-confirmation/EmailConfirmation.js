import React from "react";
import { connect } from "react-redux";
import { emailSubmit } from "../../actions";

class EmailConfirmation extends React.Component {
  state = {
    isRedirected: true,
  };

  componentDidMount() {
    let urlSearchParams = new URLSearchParams(this.props.location.search);
    if (urlSearchParams) {
      let userId = urlSearchParams.get("userId");
      let token = urlSearchParams.get("token");
      let procInstId = urlSearchParams.get("processInstanceId");
      this.props.emailSubmit(userId, token, procInstId);
    } else {
      this.setState({ isRedirected: false });
    }
  }

  render() {
    return (
      <div>
        {!this.state.isRedirected ? (
          <div>Proverite email za verifikaciju naloga</div>
        ) : (
          <>
            <div>Validacija u toku</div>
            <div>
              Bicete preusmereni na login, ako ste uneli dobar link bicete
              verifikovani
              {setTimeout(() => {
                this.props.history.push("/login");
              }, 1000)}
            </div>
          </>
        )}
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  console.log(state);
  return {
    emailConfirmation: state.form.emailConfirmation,
    processInstanceId: state.form.processInstanceId,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    emailSubmit: (userId, token, processInstanceId) =>
      dispatch(emailSubmit(userId, token, processInstanceId)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(EmailConfirmation);
