import React, { useState, useEffect } from "react";
import { connect } from "react-redux";
import { uploadDocuments, fetchFormData } from "../../actions";

const FileUpload = (props) => {
  const [files, setFiles] = useState();
  const [docCount, setDocCount] = useState(true);

  useEffect(() => {
    props.fetchFormData(props.processInstanceId);
  }, []);

  const saveFiles = (e) => {
    setFiles(e.target.files);
  };

  const onClick = () => {
    if (files.length < props.formData.formKey) {
      setDocCount(false);
      return;
    }
    setDocCount(true);
    props.uploadDocuments(files, props.processInstanceId);
  };

  return (
    <React.Fragment>
      <div>
        Upload atleast {props.formData.formKey} document to be reviewed by
        cometee
      </div>
      <input type="file" onChange={saveFiles} multiple />
      <input type="button" value="Upload" onClick={onClick} />
      {!docCount ? (
        <div>Atleast {props.formData.formKey} document is required</div>
      ) : null}
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  console.log(state);
  return {
    processInstanceId: state.form.processInstanceId,
    formData: state.form.formData,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    uploadDocuments: (files, procInstId) =>
      dispatch(uploadDocuments(files, procInstId)),
    fetchFormData: (procInstId) => dispatch(fetchFormData(procInstId)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(FileUpload);
