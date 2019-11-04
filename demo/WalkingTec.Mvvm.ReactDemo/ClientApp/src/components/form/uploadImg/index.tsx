import { Icon, message, Modal, Upload } from "antd";
import React from "react";
import RequestFiles from 'utils/RequestFiles';
import Viewer from 'viewerjs';
import lodash from 'lodash'
function beforeUpload(file) {
    console.log(file.type)
    const isJPG = lodash.includes(file.type, 'image/');
    if (!isJPG) {
        message.error('You can only upload image file!');
    }
    // const isLt2M = file.size / 1024 / 1024 < 2;
    // if (!isLt2M) {
    //     message.error('Image must smaller than 2MB!');
    // }
    return isJPG //&& isLt2M;
}
export class WtmUploadImg extends React.Component<any, any> {
    static wtmType = "UploadImg";
    img = new Image();
    viewer: Viewer = new Viewer(this.img);
    state = {
        loading: false,
        previewVisible: false,
        previewImage: '',
        fileList: this.props.value != null && this.props.value != "" ? [
            {
                uid: '-1',
                name: 'xxx.png',
                status: 'done',
                url: RequestFiles.onFileUrl(this.props.value),
            }
        ] : [],
    };
    createViewer(fileId) {
        if (fileId) {
            this.img.src = RequestFiles.onFileUrl(fileId);
        }
    }
    componentDidMount() {
        this.createViewer(this.props.value)
    }
    componentWillUnmount() {
        this.viewer && this.viewer.destroy();
    }
    onChange(data) {
        this.props.onChange(data);
    }
    handleChange = (info) => {
        if (info.file.status === 'uploading') {
            this.setState({ fileList: info.fileList, loading: true });
            //  this.setState({ loading: true });
        }
        if (info.file.status === 'done') {
            const response = info.file.response
            if (typeof response.Id === "string") {
                this.createViewer(response.Id);
                this.onChange(response.Id);
            } else {
                message.error(`${info.file.name} ${response.message}`)
            }
            this.setState({ fileList: info.fileList, loading: false });
        }
    }
    handlePreview = (file) => {
        this.viewer.show()
        // this.setState({
        //     previewImage: file.url || file.thumbUrl,
        //     previewVisible: true,
        // });
    }
    onRemove = (file) => {
        if (this.props.disabled) {
            return
        }
        const response = file.response
        this.setState({ fileList: [], loading: false }, () => {
            this.onChange(undefined);
        });
        const fileId = response && response.Id //|| this.props.value
        if (typeof fileId === "string") {
            setTimeout(() => {
                RequestFiles.onFileDelete(fileId)
            });
        }
    }
    render() {
        const { previewVisible, previewImage, fileList, loading } = this.state;
        const uploadButton = (
            <div>
                <Icon type={loading ? 'loading' : 'plus'} />
                <div className="ant-upload-text">Upload</div>
            </div>
        );
        return (
            <div style={{ minWidth: 105, minHeight: 105 }}>
                <Upload
                    accept='image/*'
                    listType="picture-card"
                    fileList={fileList as any}
                    action={RequestFiles.FileTarget}
                    beforeUpload={beforeUpload}
                    onChange={this.handleChange}
                    onPreview={this.handlePreview}
                    onRemove={this.onRemove}
                >
                    {fileList.length == 0 && uploadButton}
                </Upload>
            </div>
        );
    }
}
export default WtmUploadImg