import React from "react";
import { connect } from "react-redux";
import { fetchGenericFormData, submitPayment } from "../../actions";
import { history } from "../../history";

class Payment extends React.Component {
  async componentDidMount() {
    await this.props.fetchGenericFormData(
      this.props.processInstanceId,
      "Payment"
    );
  }

  populateFormSendingList = async (formListData) => {
    let sendingList = [];
    formListData.forEach((value, key) => {
      sendingList.push({ FieldId: key, FieldValue: value });
    });
    return [...sendingList];
  };

  setStateAsync = (state) => {
    return new Promise((resolve) => {
      this.setState(state, resolve);
    });
  };

  onFormSubmit = async (event) => {
    event.preventDefault();
    const data = new FormData(event.target);
    const listData = await this.populateFormSendingList(data);
    await this.props.submitPayment(
      listData,
      this.props.formData.taskId,
      this.props.formData.processInstanceId,
      this.props.formData.processDefinitionId
    );
    history.push("/");
  };

  render() {
    const { formData, paymentStatus } = this.props;
    console.log(this.props);
    return (
      <div style={{ margin: "30px 400px", backgroundColor: "khaki" }}>
        <h3 style={{ margin: "3px 40px" }}>Placanje</h3>
        <form onSubmit={this.onFormSubmit}>
          {formData &&
            formData.camundaFormFields.map((field) => {
              return (
                <React.Fragment>
                  {field.type === "long" ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <input
                        className="form-control"
                        id={field.formId}
                        name={field.formId}
                        type="numeric"
                        min={
                          field.validators.find(
                            (z) => z.validatorName === "min"
                          )
                            ? field.validators[
                                field.validators.findIndex(
                                  (z) => z.validatorName === "min"
                                )
                              ].validatorConfig
                            : null
                        }
                        max={
                          field.validators.find(
                            (z) => z.validatorName === "max"
                          )
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
                    </div>
                  ) : null}
                  {field.type === "string" && field.defaultValue ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <select
                        className="form-control"
                        multiple={true}
                        id={field.formId}
                        name={field.label}
                        required={
                          field.validators.find(
                            (z) => z.validatorName === "required"
                          )
                            ? true
                            : false
                        }
                      >
                        {field.defaultValue.split(",").map((val) => {
                          return (
                            <option key={val} value={val}>
                              {val}
                            </option>
                          );
                        })}
                      </select>
                    </div>
                  ) : null}
                  {field.type === "string" && field.formId === "password" ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <input
                        className="form-control"
                        id={field.formId}
                        name={field.formId}
                        type="password"
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
                    </div>
                  ) : null}
                  {field.type === "string" &&
                  field.formId !== "password" &&
                  field.formId !== "email" &&
                  !field.defaultValue ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <input
                        className="form-control"
                        id={field.formId}
                        name={field.formId}
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
                    </div>
                  ) : null}
                  {field.type === "string" &&
                  field.formId !== "password" &&
                  field.formId === "email" ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <input
                        className="form-control"
                        id={field.formId}
                        name={field.formId}
                        type="email"
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
                    </div>
                  ) : null}
                  {field.type === "enum" ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <select
                        className="form-control"
                        id={field.formId}
                        name={field.label}
                        required={
                          field.validators.find(
                            (z) => z.validatorName === "required"
                          )
                            ? true
                            : false
                        }
                      >
                        {field.values.map((val) => {
                          return (
                            <option key={val.id} value={val.id}>
                              {val.name}
                            </option>
                          );
                        })}
                      </select>
                    </div>
                  ) : null}
                  {/*proveri da li ima greske  */}
                </React.Fragment>
              );
            })}
          <button type="submit" className="btn btn-primary">
            Submit
          </button>
        </form>
      </div>
    );
  }
}
const mapStateToProps = (state) => {
  console.log(state);
  return {
    formData: state.form.formData,
    paymentStatus: state.form.paymentStatus,
    processInstanceId: state.form.processInstanceId,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchGenericFormData: (processInstanceId, taskNameOrId) =>
      dispatch(fetchGenericFormData(processInstanceId, taskNameOrId)),
    submitPayment: (formListData, taskId, procInstanceId) =>
      dispatch(submitPayment(formListData, taskId, procInstanceId)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Payment);
