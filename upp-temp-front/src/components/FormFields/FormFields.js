import React from "react";
import { connect } from "react-redux";
import {
  fetchFormData,
  submitWriterForm,
  startWriterProcess,
} from "../../actions";
import { history } from "../../history";

const formData = {
  processDefinitionId:
    "Process_Probe_12:2:c1caa886-3cc5-11eb-92f4-e4f89c5bfdff",
  // processInstanceId: //TO NAM TREBA IZVUCI TO IZ APIJA
  processDefinitionKey: "Process_Probe_12",
  taskId: "registration_task",
  taskName: "Registration",
  formKey: "form_key",
  camundaFormFields: [
    {
      formId: "username",
      label: "Username",
      type: "string",
      defaultValue: "Akcija,Komedija,Biografija",
      values: [
        {
          id: "odluka1",
          name: "prodji",
        },
        {
          id: "odluka2",
          name: "jos materijala",
        },
      ],
      validators: [
        {
          validatorName: "required",
          validatorConfig: "none",
        },
        // {
        //   validatorName: "minlength",
        //   validatorConfig: "6",
        // },
      ],
    },
  ],
};

class FormFields extends React.Component {
  state = {
    zanrovi: formData,
    genreRequirement: false,
  };

  async componentDidMount() {
    console.log(this.props);
    await this.props.startWriterProcess();
    await this.props.fetchFormData(this.props.processInstanceId);
  }

  // populateFormSendingList = (formListData) => {
  //   let list = [];
  //   let genresList = [];
  //   formListData.forEach((value, key) => {
  //     if (key == "genres") {
  //       genresList.push(value);
  //     } else {
  //       list.push({ FieldId: key, FieldValue: value });
  //     }
  //   });
  //   //spoji sve vrednosti pod kljucem genres iz niza genresList da bude string zbog modela u camundi
  //   if (genresList.length > 0) {
  //     let l2 = [{ FieldId: "genres", FieldValue: genresList.join(",") }];
  //     this.setState({
  //       ...this.state,
  //       checkboxCondition: true,
  //     });
  //     return [...list, ...l2];
  //   } else {
  //     this.setState({ ...this.state, checkboxCondition: false });
  //     return list;
  //   }
  // };

  populateFormSendingList = async (formListData) => {
    let sendingList = [];
    let genresList = [];
    let cometeeList = [];
    formListData.forEach((value, key) => {
      if (key === "Genres") {
        genresList.push(value);
      } else if (key === "Odluka") {
        cometeeList.push(value);
      } else {
        sendingList.push({ FieldId: key, FieldValue: value });
      }
    });
    console.log(genresList);
    console.log(cometeeList);
    if (genresList.length > 0) {
      let l2 = [{ FieldId: "genres_", FieldValue: genresList.join(",") }];
      await this.setStateAsync({ genreRequirement: true });
      console.log(this.state);
      return [...sendingList, ...l2];
    } else if (cometeeList.length > 0) {
      let l3 = [{ FieldId: "odluka", FieldValue: genresList }];
    } else {
      await this.setStateAsync({ genreRequirement: false });
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

    if (this.state.genreRequirement === false) return;
    console.log(listData);
    await this.props.submitWriterForm(
      listData,
      this.props.formData.taskId,
      this.props.formData.processInstanceId,
      this.props.formData.processDefinitionId
    );
    history.push("/email-confirmation");
  };

  render() {
    const { formData, registrationResponse } = this.props;
    console.log(this.props);
    return (
      <div style={{ margin: "30px 400px", backgroundColor: "khaki" }}>
        <h3 style={{ margin: "3px 40px" }}>Registracija pisca</h3>
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
    registrationResponse: state.form.registrationResponse,
    processInstanceId: state.form.processInstanceId,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFormData: (processInstanceId) =>
      dispatch(fetchFormData(processInstanceId)),
    submitWriterForm: (formListData, taskId, procInstanceId) =>
      dispatch(submitWriterForm(formListData, taskId, procInstanceId)),
    startWriterProcess: () => dispatch(startWriterProcess()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(FormFields);
