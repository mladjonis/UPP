import React from "react";
import { connect } from "react-redux";
import {
  fetchFormData,
  submitReaderForm,
  startReaderProcess,
  fetchBetaReadersFormData
} from "../../actions";
import { history } from "../../history";

class FormFieldsReader extends React.Component {
  state = {
    opened: false
  }
  async componentDidMount() {
    await this.props.startReaderProcess();
    await this.props.fetchFormData(this.props.processInstanceId);
  }
  populateFormSendingList = async (formListData) => {
    let sendingList = [];
    let genresList = [];
    let cometeeList = [];
    let betaGenresList = [];
    formListData.forEach((value, key) => {
      if (key === "Genres") {
        genresList.push(value);
      } else if (key === "Odluka") {
        cometeeList.push(value);
      } 
      else if(key === "Beta reader genres"){
        betaGenresList.push(value);
      }
      else {
        sendingList.push({ FieldId: key, FieldValue: value });
      }
    });
    console.log(genresList);
    console.log(cometeeList);
    if (genresList.length > 0) {
      let l2 = [{ FieldId: "genres", FieldValue: genresList.join(",") }];
      await this.setStateAsync({ genreRequirement: true });
      console.log(this.state);
      return [...sendingList, ...l2];
    } else if (cometeeList.length > 0) {
      let l3 = [{ FieldId: "odluka", FieldValue: genresList }];
    } else if (betaGenresList.length > 0){
      let l4 = [{ FieldId: "beta_genres_reader", FieldValue: betaGenresList}];
      return [...sendingList, ...l4];
    }
    else {
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
    await this.props.submitReaderForm(
      listData,
      this.props.formData.taskId,
      this.props.formData.processInstanceId,
      this.props.formData.processDefinitionId
    );
    history.push("/email-confirmation");
  };

  // betaReader = async () => {
  //   await this.props.fetchBetaReadersFormData(this.props.processInstanceId, "Beta reader / genres");
  // }
  handleGenres = () => {
    this.setState({ opened: !this.state.opened });
  }

  render() {
    const { formData, registrationResponse, beta } = this.props;
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
                  {field.type === "string" && field.formId === "genres" ? (
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
                  {field.type === "string" && field.formId === "beta_reader_genres" ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <select
                        className="form-control"
                        multiple={true}
                        id={field.formId}
                        name={field.label}
                        hidden={this.state.opened}
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
                  field.formId !== "genres" &&
                  field.formId !== "password" &&
                  field.formId !== "email" ? (
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
                  field.formId !== "genres" &&
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
                            <option key={val.id} value={val.name}>
                              {val.name}
                            </option>
                          );
                        })}
                      </select>
                    </div>
                  ) : null}
                  {field.type === "boolean" ? (
                    <div className="form-group">
                      <label htmlFor={field.formId}> {field.label}</label>
                      <input
                        className="form-control"
                        id={field.formId}
                        name={field.formId}
                        type="boolean"
                        onClick={this.handleGenres}
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
                  {/*proveri da li ima greske  */}
                </React.Fragment>
              );
            })}
            {/* {
              beta && beta.camundaFormFields.map(field=>{
                return (
                  <React.Fragment>
                    {field.type === "boolean" ? (
                      <div className="form-group" onClick={this.betaReader}>
                        <label htmlFor={field.formId}> {field.label}</label>
                        <input
                          className="form-control"
                          id={field.formId}
                          name={field.formId}
                          type="boolean"
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
                  </React.Fragment>)
              })
            } */}
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
    beta: state.form.beta
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFormData: (processInstanceId) =>
      dispatch(fetchFormData(processInstanceId)),
    submitReaderForm: (formListData, taskId, procInstanceId) =>
      dispatch(submitReaderForm(formListData, taskId, procInstanceId)),
    startReaderProcess: () => dispatch(startReaderProcess()),
    fetchBetaReadersFormData: (procInstanceId, taskName) => dispatch(fetchBetaReadersFormData(procInstanceId,taskName))
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(FormFieldsReader);
