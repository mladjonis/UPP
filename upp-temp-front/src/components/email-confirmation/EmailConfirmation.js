import React, { useEffect } from "react";
import { connect } from "react-redux";
import { emailSubmit } from "../../actions";

const EmailConfirmation = (props) => {
  const {
    emailSubmit,
    history,
    emailConfirmation,
    location,
    processInstanceId,
  } = props;
  useEffect(() => {
    let urlSearchParams = new URLSearchParams(location.search);
    let userId = urlSearchParams.get("userId");
    let token = urlSearchParams.get("token");
    emailSubmit(userId, token, processInstanceId);
  }, [emailSubmit, location, processInstanceId]);
  return (
    <div>
      <div>Validacija u toku</div>
      {!emailConfirmation ? (
        <div>
          Validacija uspesna bicete preusmereni na login
          {setTimeout(() => {
            history.push("/login");
          }, 1000)}
        </div>
      ) : (
        <div>Desila se greska prilikom validacije pokusajte ponovo</div>
      )}
    </div>
  );
};

const mapStateToProps = (state) => {
  return {
    emailConfirmation: state.form.emailConfirmation,
    processInstanceId: state.form.processInstanceId,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    emailSubmit: (userId, token) => dispatch(emailSubmit(userId, token)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(EmailConfirmation);
