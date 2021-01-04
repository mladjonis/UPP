import React, { useState } from "react";
import { connect } from "react-redux";
import { uploadDocuments } from "../../actions";

const FileUpload = (props) => {
  const [files, setFiles] = useState();
  const [docCount, setDocCount] = useState(true);

  const saveFiles = (e) => {
    setFiles(e.target.files);
  };

  const onClick = () => {
    if (files.length < 2) {
      setDocCount(false);
      return;
    }
    setDocCount(true);
    props.uploadDocuments(files);
  };

  return (
    <React.Fragment>
      <input type="file" onChange={saveFiles} multiple />
      <input type="button" value="Upload" onClick={onClick} />
      {!docCount ? <div>Atleast 2 document is required</div> : null}
    </React.Fragment>
  );
};

const mapDispatchToProps = (dispatch) => {
  return {
    uploadDocuments: (files) => dispatch(uploadDocuments(files)),
  };
};

export default connect(null, mapDispatchToProps)(FileUpload);
