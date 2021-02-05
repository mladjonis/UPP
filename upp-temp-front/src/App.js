import React from "react";
import Home from "./components/Home/Home";
import EmailConfirmation from "./components/email-confirmation/EmailConfirmation";
import Registration from "./components/registration/Registration";
import Login from "./components/login/Login";
import Navbar from "./components/navbar/Navbar";
import { Route, Switch } from "react-router-dom";
import FileUpload from "./components/file-upload/FileUpload";
import DecisionsDetails from "./components/cometee/DecisionsDetails";
import Payment from "./components/payment/Payment";
import FileUploadMore from "./components/file-upload/FileUploadMore";
import ChooseEditors from "./components/choose-editors/ChooseEditors";
import PlagiarismProposal from "./components/plagiarism-proposal/PlagiarismProposal";
import PlagiarismMessage from "./components/plagiarism-proposal/PlagiarismMessage";
import DecisionPlagiarism from "./components/cometee/DecisionPlagiarism";

function App() {
  return (
    <React.Fragment>
      <Navbar />
      <Switch>
        <Route path="/" exact component={Home} />
        <Route path="/login" component={Login} />
        <Route path="/registration" component={Registration} />
        <Route path="/email-confirmation" component={EmailConfirmation} />
        <Route path="/upload" component={FileUpload} />
        <Route path="/upload-more" component={FileUploadMore} />
        <Route path="/cometee" component={DecisionsDetails} />
        <Route path="/payment" component={Payment} />
        <Route path="/proposal" component={PlagiarismProposal} />
        <Route path="/proposal-message" component={PlagiarismMessage} />
        <Route path="/choose-editors" component={ChooseEditors} />
        <Route path="/decision-plagiarism" component={DecisionPlagiarism} />
      </Switch>
    </React.Fragment>
  );
}

export default App;
