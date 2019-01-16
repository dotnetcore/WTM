import { Input, Switch, Icon, Select, Upload, message, Modal } from 'antd';
import * as React from 'react';
import Store from '../store';
function beforeUpload(file) {
    const isJPG = file.type === 'image/jpeg';
    if (!isJPG) {
        message.error('You can only upload JPG file!');
    }
    const isLt2M = file.size / 1024 / 1024 < 2;
    if (!isLt2M) {
        message.error('Image must smaller than 2MB!');
    }
    return isJPG && isLt2M;
}
export default {
    /** ITCode */
    ITCode: <Input placeholder="请输入 ITCode" />,
    /** Password */
    Password: <Input placeholder="请输入 Password" />,
    /** Email */
    Email: <Input placeholder="请输入 Email" />,
    /** Name */
    Name: <Input placeholder="请输入 Name" />,
    /** IsValid */
    IsValid: <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} />,
    /** 照片 */
    PhotoId: class extends React.Component<any, any> {
        state = {
            loading: false,
            previewVisible: false,
            previewImage: '',
            fileList: this.props.initialValue != null && this.props.initialValue != "" ? [
                {
                    uid: '-1',
                    name: 'xxx.png',
                    status: 'done',
                    url: this.props.initialValue,
                }
            ] : [],
        };
        onChange(data) {
            this.props.onChange(data);
        }
        handleChange = (info) => {
            console.log(info.file.status);
            if (info.file.status === 'uploading') {
                this.setState({ fileList: info.fileList, loading: true });
                //  this.setState({ loading: true });
            }
            if (info.file.status === 'done') {
                const response = info.file.response
                if (typeof response.id === "string") {
                    this.onChange(response.id);
                } else {
                    message.error(`${info.file.name} ${response.message}`)
                }
                this.setState({ fileList: info.fileList, loading: false });
            }
        }
        onRemove = (file) => {
            console.log(file);
            const response = file.response
            if (typeof response.id === "string") {
                Store.onFileDelete(response.id)
            }
            this.setState({ fileList: [], loading: false });
        }
        render() {
            console.log(this.props);
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
                        // name="avatar"
                        accept='image/jpeg'
                        listType="picture-card"
                        // className="avatar-uploader"
                        // showUploadList={false}
                        fileList={fileList as any}
                        action={Store.Request.address + Store.Urls.fileUpload.src}
                        beforeUpload={beforeUpload}
                        onChange={this.handleChange}
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
    },
    /** 性别 */
    Sex: <Select placeholder="性别" >
        <Select.Option value="0">男</Select.Option>
        <Select.Option value="1">女</Select.Option>
    </Select>
}
