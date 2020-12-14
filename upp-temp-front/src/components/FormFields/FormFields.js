import React from "react";
import { connect } from "react-redux";
import { compose } from "redux";
import { fetchFormData } from "../../actions";

class FormFields extends React.Component {
  componentDidMount() {
    this.props.fetchFormData();
  }
  render() {
    const { formData } = this.props;
    return (
      <form>
        {formData &&
          formData.camundaFormFields.map((field) => {
            return (
              <React.Fragment>
                {field.type === "long" ? (
                  <React.Fragment>
                    <label htmlFor={field.formId}> {field.label}</label>
                    <input
                      id={field.formId}
                      type="numeric"
                      min={
                        field.validators.find((z) => z.validatorName === "min")
                          ? field.validators[
                              field.validators.findIndex(
                                (z) => z.validatorName === "min"
                              )
                            ].validatorConfig
                          : null
                      }
                      max={
                        field.validators.find((z) => z.validatorName === "max")
                          ? field.validators[
                              field.validators.findIndex(
                                (z) => z.validatorName === "max"
                              )
                            ].validatorConfig
                          : null
                      }
                      readOnly={
                        field.validators.find(
                          (z) => z.validatorName === "readonly"
                        )
                          ? true
                          : false
                      }
                      required={
                        field.validators.find(
                          (z) => z.validatorName === "required"
                        )
                          ? true
                          : false
                      }
                    />
                  </React.Fragment>
                ) : null}
                {field.type === "string" ? (
                  <React.Fragment>
                    <label htmlFor={field.formId}> {field.label}</label>
                    <input
                      id={field.formId}
                      type="text"
                      minLength={
                        field.validators.find(
                          (z) => z.validatorName === "minlength"
                        )
                          ? field.validators[
                              field.validators.findIndex(
                                (z) => z.validatorName === "minlength"
                              )
                            ].validatorConfig
                          : null
                      }
                      maxLength={
                        field.validators.find(
                          (z) => z.validatorName === "maxlength"
                        )
                          ? field.validators[
                              field.validators.findIndex(
                                (z) => z.validatorName === "maxlength"
                              )
                            ].validatorConfig
                          : null
                      }
                      readOnly={
                        field.validators.find(
                          (z) => z.validatorName === "readonly"
                        )
                          ? true
                          : false
                      }
                      required={
                        field.validators.find(
                          (z) => z.validatorName === "required"
                        )
                          ? true
                          : false
                      }
                    />
                  </React.Fragment>
                ) : null}
              </React.Fragment>
            );
          })}
      </form>
    );
  }
}

const mapStateToProps = (state) => {
  console.log("state", state);
  return {
    formData: state.form.formData,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFormData: () => dispatch(fetchFormData()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(FormFields);
