import React, { useState } from "react";
import { connect } from "react-redux";
import { uploadDocuments } from "../../actions";

const FileUploadMore = (props) => {
  const [files, setFiles] = useState();
  const [docCount, setDocCount] = useState(true);

  const saveFiles = (e) => {
    setFiles(e.target.files);
  };

  const onClick = () => {
    if (files.length < 1) {
      setDocCount(false);
      return;
    }
    setDocCount(true);
    props.uploadDocuments(files, props.processInstanceId);
  };

  return (
    <React.Fragment>
      <div>Upload atleast 1 document to be re-reviewed by cometee</div>
      <input type="file" onChange={saveFiles} multiple />
      <input type="button" value="Upload" onClick={onClick} />
      {!docCount ? <div>Atleast 1 document is required</div> : null}
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  console.log(state);
  return {
    processInstanceId: state.form.processInstanceId,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    uploadDocuments: (files, procInstId) =>
      dispatch(uploadDocuments(files, procInstId)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(FileUploadMore);
