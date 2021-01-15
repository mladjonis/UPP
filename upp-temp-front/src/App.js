import React from "react";
import Home from "./components/Home/Home";
import EmailConfirmation from "./components/email-confirmation/EmailConfirmation";
import Registration from "./components/registration/Registration";
import Login from "./components/login/Login";
import Navbar from "./components/navbar/Navbar";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import FileUpload from "./components/file-upload/FileUpload";
import DecisionsDetails from "./components/cometee/DecisionsDetails";
import Payment from "./components/payment/Payment";

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
        <Route path="/cometee" component={DecisionsDetails} />
        <Route path="/payment" component={Payment} />
      </Switch>
    </React.Fragment>
  );
}

export default App;
