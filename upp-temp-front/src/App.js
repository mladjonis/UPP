import React from "react";
import "./App.css";
import Home from "./components/Home/Home";

function App() {
  const forma = {
    processInstanceId:
      "Process_Probe_12:2:c1caa886-3cc5-11eb-92f4-e4f89c5bfdff",
    processDefinitionKey: "Process_Probe_12",
    taskId: "registration_task",
    taskName: "Registration",
    formKey: "form_key",
    camundaFormFields: [
      {
        formId: "username",
        label: "Username",
        type: "string",
        validators: [
          {
            validatorName: "required",
            validatorConfig: "none",
          },
          {
            validatorName: "minlength",
            validatorConfig: "6",
          },
        ],
      },
    ],
  };
  return (
    <div className="App">
      <Home />
    </div>
  );
}

export default App;
