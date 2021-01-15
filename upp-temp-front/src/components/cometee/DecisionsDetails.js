import React from "react";
import { connect } from "react-redux";
import { Link } from "react-router-dom";
import {
  fetchFormData,
  fetchFormDataCometee,
  submitCometeeForm,
  getCometeeUsers,
} from "../../actions";

const formData = {
  processInstanceId: "aade0973-4fcd-11eb-9997-e4f89c5bfdff",
  processDefinitionKey: "Process_0abkzqf",
  taskId: "Activity_0yqye3f",
  taskName: "asa",
  formKey: null,
  camundaFormFields: [
    {
      formId: "komentar",
      label: "Komentar",
      type: "string",
      defaultValue: null,
      validators: [
        {
          validatorName: "required",
          validatorConfig: null,
        },
      ],
      values: [],
    },
    {
      formId: "odluka",
      label: "Odluka",
      type: "enum",
      defaultValue: null,
      validators: [],
      values: [
        {
          id: "odluka1",
          label: null,
          name: "111111",
        },
        {
          id: "odluka2",
          label: null,
          name: "222222",
        },
      ],
    },
  ],
};

class DecisionsDetails extends React.Component {
  state = {
    odlukaRequirement: false,
  };

  async componentDidMount() {
    await this.props.getCometeeUsers();
    await this.props.fetchFormDataCometee(
      this.props.processInstanceId.processInstanceId,
      "Commetee meeting"
    );
  }

  async componentDidUpdate() {
    await this.props.getCometeeUsers();
  }

  populateFormSendingList = async (formListData) => {
    let sendingList = [];
    let cometeeList = [];
    formListData.forEach((value, key) => {
      if (key === "Odluka") {
        cometeeList.push(value);
      } else {
        sendingList.push({ FieldId: key, FieldValue: value });
      }
    });

    if (cometeeList.length > 0) {
      let l3 = [{ FieldId: "odluka", FieldValue: cometeeList[0] }];
      await this.setStateAsync({ odlukaRequirement: true });
      return [...sendingList, ...l3];
    } else {
      await this.setStateAsync({ odlukaRequirement: false });
      return [...sendingList];
    }
  };

  setStateAsync = (state) => {
    return new Promise((resolve) => {
      this.setState(state, resolve);
    });
  };

  onFormSubmit = async (event) => {
    event.preventDefault();
    console.log(event.target);
    const data = new FormData(event.target);
    const listData = await this.populateFormSendingList(data);
    //format je recimo input-value pa onda input-name
    console.log(this.state);

    if (this.state.odluka === false) return;
    console.log(listData);
    this.props.submitCometeeForm(
      listData,
      // formData.taskId,
      // formData.processInstanceId,
      // formData.processDefinitionKey
      this.props.formData.taskId,
      this.props.formData.processInstanceId,
      this.props.formData.processDefinitionId
    );
  };

  render() {
    const { formData, cometeeUsers } = this.props;
    console.log(this.props);
    return (
      <div style={{ backgroundColor: "khaki" }}>
        <h3 style={{ margin: "3px 40px" }}>Validacija korisnika</h3>
        {cometeeUsers &&
          cometeeUsers.users.map((user) => {
            return (
              <>
                <div>{user.email}</div>
                {user.files &&
                  user.files.split(",").map((link) => {
                    return <a href={link}>Preview</a>;
                  })}
                <form onSubmit={this.onFormSubmit}>
                  {formData &&
                    formData.camundaFormFields.map((field) => {
                      return (
                        <React.Fragment>
                          {field.type === "long" ? (
                            <div className="form-group">
                              <label htmlFor={field.formId}>
                                {" "}
                                {field.label}
                              </label>
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
                          {field.type === "string" &&
                          field.formId === "genres" ? (
                            <div className="form-group">
                              <label htmlFor={field.formId}>
                                {" "}
                                {field.label}
                              </label>
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
                          {field.type === "string" &&
                          field.formId !== "genres" &&
                          field.formId === "password" ? (
                            <div className="form-group">
                              <label htmlFor={field.formId}>
                                {" "}
                                {field.label}
                              </label>
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
                          field.formId !== "genres" &&
                          field.formId !== "password" &&
                          field.formId !== "email" ? (
                            <div className="form-group">
                              <label htmlFor={field.formId}>
                                {" "}
                                {field.label}
                              </label>
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
                          field.formId !== "genres" &&
                          field.formId !== "password" &&
                          field.formId === "email" ? (
                            <div className="form-group">
                              <label htmlFor={field.formId}>
                                {" "}
                                {field.label}
                              </label>
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
                              <label htmlFor={field.formId}>
                                {" "}
                                {field.label}
                              </label>
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
              </>
            );
          })}
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  console.log(state);
  return {
    formData: state.form.formData,
    processInstanceId: state.form.cometeeUsers,
    cometeeUsers: state.form.cometeeUsers,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFormData: (processInstanceId) =>
      dispatch(fetchFormData(processInstanceId)),
    fetchFormDataCometee: (processInstanceId, taskNameOrId) =>
      dispatch(fetchFormDataCometee(processInstanceId, taskNameOrId)),
    submitCometeeForm: (formListData, taskId, procInstanceId) =>
      dispatch(submitCometeeForm(formListData, taskId, procInstanceId)),
    getCometeeUsers: () => dispatch(getCometeeUsers()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(DecisionsDetails);
