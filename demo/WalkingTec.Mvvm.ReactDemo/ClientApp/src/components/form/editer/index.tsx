/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:42
 * @modify date 2018-09-12 18:53:42
 * @desc [description]
*/
import { Card } from 'antd';
import BraftEditor, { BraftEditorProps, EditorState } from 'braft-editor';
import 'braft-editor/dist/index.css';
import lodash from 'lodash';
import React from 'react';
import './style.less';
import globalConfig from 'global.config';
import RequestFiles from 'utils/RequestFiles';



interface IAppProps extends BraftEditorProps {
  display?: boolean;
  disabled?: boolean;
  onChange?: any;
  [key: string]: any;
}
/**
 * https://github.com/margox/braft-editor
 * 富文本编辑
 */
export class WtmEditor extends React.Component<IAppProps, any> {
  static wtmType = "Editor";
  console = globalConfig.development
  default: BraftEditorProps = {
    media: {
      uploadFn: (params) => {
        console.log("TCL: WtmEditor -> params", params)
        RequestFiles.customRequest({
          action: RequestFiles.FileTarget,
          filename: "file",
          file: params.file,
          onProgress: (event) => {
            params.progress(event.percent)
          },
          onSuccess: (res) => {
            const url = RequestFiles.onFileDownload(res.Id);
            params.success({
              url: url,
              meta: {
                id: res.Id,
                title: res.Name,
                alt: res.Name,
                loop: true, // 指定音视频是否循环播放
                autoPlay: true, // 指定音视频是否自动播放
                controls: true, // 指定音视频是否显示控制栏
                poster: url, // 指定视频播放器的封面
              }
            })
          },
          onError: (err) => {
            params.error({
              msg: err.message
            })
          }
        });
      }
    }
  }
  componentDidMount() {
  }
  state = {
    editorState: BraftEditor.createEditorState(lodash.unescape(this.props.value)), // 设置编辑器初始内容
  }
  onChange(editorState: EditorState) {
    if (lodash.isEqual(this.state.editorState.toRAW(), editorState.toRAW())) {
      return this.setState({ editorState });
    }
    this.setState({ editorState });
    const value = editorState.toHTML()
    this.console && console.log(value)
    this.props.onChange && this.props.onChange(lodash.escape(lodash.eq(value, '<p></p>') ? '' : value), editorState)
  }
  render() {
    const { editorState } = this.state;
    const props = { ...this.default, ...this.props }
    delete props.value;
    delete props.onChange;
    // 禁用
    if (this.props.display) {
      props.controls = [];
      props.disabled = true;
    }
    return <Card className="app-editor-card">
      <BraftEditor
        value={editorState}
        onChange={this.onChange.bind(this)}
        {...props}
      />
    </Card>
  }
}
export default WtmEditor

