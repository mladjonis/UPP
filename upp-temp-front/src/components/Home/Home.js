import React from "react";
import { connect } from "react-redux";
import { dummy } from "../../actions";

const Home = (props) => {
  console.log(props);
  return (
    <div className="container">
      {/*prosledi route propsOVE u child komponentu ako bude trebalo */}
      <button onClick={() => props.dummy()}>asa</button>
      {/* <FormFields {...props} /> */}
    </div>
  );
};

const mapDispatchToProps = (dispatch) => {
  return {
    dummy: () => dispatch(dummy()),
  };
};

export default connect(null, mapDispatchToProps)(Home);
