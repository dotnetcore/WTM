/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:42
 * @modify date 2018-09-12 18:53:42
 * @desc [description]
*/
import { message } from 'antd';
import BraftEditor, { EditorState } from 'braft-editor';
import * as React from 'react';
/**
 * https://github.com/margox/braft-editor
 * 富文本编辑
 */
export default class extends React.Component<any, any> {
  state = {
    responseList: [],
  }
  receiveHtml(object) {
    const onChange = this.props.onChange;
    onChange(object == '<p></p>' ? null : object);
  }
  shouldComponentUpdate() {
    return false;
  }
  onChange = (content) => {
    this.receiveHtml(content.toHTML());
    console.log('onChange', content.toHTML())
  }

  onRawChange = (rawContent) => {
    console.log('onRawChange', rawContent)
  }

  render() {
    const editorProps = {
      height: 500,
      contentFormat: 'html',
      // initialContent: this.props.value,
      defaultValue: EditorState.createFrom(this.props.value),
      onChange: this.onChange,
      media: {
        allowPasteImage: true, // 是否允许直接粘贴剪贴板图片（例如QQ截图等）到编辑器
        image: true, // 开启图片插入功能
        video: false, // 开启视频插入功能
        audio: false, // 开启音频插入功能
        validateFn: null, // 指定本地校验函数，说明见下文
        uploadFn: (param) => {
          const serverURL = '/admin/upload'
          const xhr = new XMLHttpRequest
          const fd = new FormData()
          // libraryId可用于通过mediaLibrary示例来操作对应的媒体内容
          // console.log(param.libraryId)

          const successFn = (response) => {
            console.log(xhr.response);
            response = JSON.parse(xhr.response);
            // 假设服务端直接返回文件上传后的地址
            // 上传成功后调用param.success并传入上传后的文件地址
            if (response.code == 200) {
              param.success({
                url: response.data.fullUrl,
                meta: {
                  id: 'xxx',
                  title: 'xxx',
                  alt: 'xxx',
                  loop: true, // 指定音视频是否循环播放
                  autoPlay: true, // 指定音视频是否自动播放
                  controls: true, // 指定音视频是否显示控制栏
                  poster: 'http://xxx/xx.png', // 指定视频播放器的封面
                }
              })
            } else {
              message.error(`图片上传失败`);
              param.error({
                msg: 'unable to upload.'
              })
            }
          }

          const progressFn = (event) => {
            // 上传进度发生变化时调用param.progress
            param.progress(event.loaded / event.total * 100)
          }

          const errorFn = (response) => {
            // 上传发生错误时调用param.error
            param.error({
              msg: 'unable to upload.'
            })
            message.error(`图片上传失败`);
          }

          xhr.upload.addEventListener("progress", progressFn, false)
          xhr.addEventListener("load", successFn, false)
          xhr.addEventListener("error", errorFn, false)
          xhr.addEventListener("abort", errorFn, false)

          fd.append('file', param.file)
          xhr.open('POST', serverURL, true)
          xhr.send(fd)

        }, // 指定上传函数，说明见下文
        removeConfirmFn: null, // 指定删除前的确认函数，说明见下文
        onRemove: null, // 指定媒体库文件被删除时的回调，参数为被删除的媒体文件列表(数组)
        onChange: null, // 指定媒体库文件列表发生变化时的回调，参数为媒体库文件列表(数组)
        onInsert: null, // 指定从媒体库插入文件到编辑器时的回调，参数为被插入的媒体文件列表(数组)
      }
    }

    return (<BraftEditor {...editorProps} />)
  }
}

