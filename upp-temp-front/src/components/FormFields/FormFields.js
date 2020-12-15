import React from "react";
import { connect } from "react-redux";
import { compose } from "redux";
import { fetchFormData, submitWriterForm } from "../../actions";

// const formData = {
//   processDefinitionId: "Process_Probe_12:2:c1caa886-3cc5-11eb-92f4-e4f89c5bfdff",
//   processInstanceId: //TO NAM TREBA IZVUCI TO IZ APIJA
//   processDefinitionKey: "Process_Probe_12",
//   taskId: "registration_task",
//   taskName: "Registration",
//   formKey: "form_key",
//   camundaFormFields: [
//     {
//       formId: "username",
//       label: "Username",
//       type: "string",
//       validators: [
//         {
//           validatorName: "required",
//           validatorConfig: "none",
//         },
//         {
//           validatorName: "minlength",
//           validatorConfig: "6",
//         },
//       ],
//     },
//   ],
// };

class FormFields extends React.Component {
  componentDidMount() {
    this.props.fetchFormData();
  }

  populateFormSendingList = (formListData) => {
    let list = [];
    formListData.forEach((value, key) =>
      list.push({ FieldId: key, FieldValue: value })
    );
    return list;
  };

  onFormSubmit = (event) => {
    event.preventDefault();
    const data = new FormData(event.target);
    //format je recimo input-value pa onda input-name
    this.props.submitWriterForm(
      this.populateFormSendingList(data),
      this.props.formData.taskId,
      this.props.formData.processInstanceId,
      this.props.formData.processDefinitionId
    );
  };

  render() {
    const { formData } = this.props;
    return (
      <form onSubmit={this.onFormSubmit}>
        {formData &&
          formData.camundaFormFields.map((field) => {
            return (
              <React.Fragment>
                {field.type === "long" ? (
                  <React.Fragment>
                    <label htmlFor={field.formId}> {field.label}</label>
                    <input
                      id={field.formId}
                      name={field.formId}
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
                  </React.Fragment>
                ) : null}
              </React.Fragment>
            );
          })}
        <button type="submit">Submit</button>
      </form>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    formData: state.form.formData,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFormData: () => dispatch(fetchFormData()),
    submitWriterForm: (formListData, taskId, procInstanceId) =>
      dispatch(submitWriterForm(formListData, taskId, procInstanceId)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(FormFields);
