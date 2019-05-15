import { Icon, message, Modal, Upload, Button } from "antd";
import React from "react";
import RequestFiles from 'utils/RequestFiles';
import lodash from 'lodash'
export class WtmUpload extends React.Component<any, any> {
    static wtmType = "Upload";
    img = new Image();
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
    onRemove = (file) => {
        if (this.props.disabled) {
            return
        }
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
        return (
            <Upload
                action={RequestFiles.FileTarget}
                onChange={this.handleChange}
                onRemove={this.onRemove}
            >
                <Button>
                    <Icon type="upload" /> 选择文件
    </Button>
            </Upload>
        );
    }
}
export default WtmUpload