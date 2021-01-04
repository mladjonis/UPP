import React, { useState } from "react";
import FormFields from "../FormFields/FormFields";
import FormFieldsReader from "../FormFields/FormFieldsReader";

const Registration = (props) => {
  const [registrationDecistion, setRegistrationDecistion] = useState("");

  const registration = () => {
    if (registrationDecistion === "reader") {
      //ubaciti novu komponentu za citaoce
      return <FormFieldsReader />;
    } else if (registrationDecistion === "writer") {
      return <FormFields />;
    } else {
      return null;
    }
  };

  return (
    <div className="container">
      <button onClick={() => setRegistrationDecistion("reader")}>
        Reader registration
      </button>
      <button onClick={() => setRegistrationDecistion("writer")}>
        Writer registration
      </button>
      {registration()}
    </div>
  );
};

export default Registration;
