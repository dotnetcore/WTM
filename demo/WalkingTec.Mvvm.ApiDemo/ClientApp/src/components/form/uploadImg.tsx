import { message, Icon, Upload, Modal } from "antd";
import RequestFiles from 'utils/RequestFiles';
import React from "react";

function beforeUpload(file) {
    const isJPG = file.type === 'image/jpeg';
    if (!isJPG) {
        message.error('You can only upload JPG file!');
    }
    // const isLt2M = file.size / 1024 / 1024 < 2;
    // if (!isLt2M) {
    //     message.error('Image must smaller than 2MB!');
    // }
    return isJPG //&& isLt2M;
}
export default class UploadImg extends React.Component<any, any> {
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
                this.onChange(response.Id);
            } else {
                message.error(`${info.file.name} ${response.message}`)
            }
            this.setState({ fileList: info.fileList, loading: false });
        }
    }
    handlePreview = (file) => {
        this.setState({
            previewImage: file.url || file.thumbUrl,
            previewVisible: true,
        });
    }
    onRemove = (file) => {
        const response = file.response
        this.setState({ fileList: [], loading: false }, () => {
            this.onChange(null);
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
            <>
                <Upload
                    accept='image/jpeg'
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
                <Modal visible={previewVisible} footer={null} onCancel={() => this.setState({ previewVisible: false })}>
                    <img alt="example" style={{ width: '100%' }} src={previewImage} />
                </Modal>
            </>
        );
    }
}