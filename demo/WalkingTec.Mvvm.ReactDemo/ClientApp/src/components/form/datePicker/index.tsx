/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-26 00:21:18
 * @modify date 2019-02-26 00:21:18
 * @desc [description]
 */
import { DatePicker } from 'antd';
import { DatePickerProps } from 'antd/lib/date-picker/interface';
import lodash from 'lodash';
import moment from 'moment';
import * as React from 'react';
const { MonthPicker, RangePicker, WeekPicker } = DatePicker;


interface IWtmDatePickerProps extends DatePickerProps {
  display?: boolean;
  [key: string]: any;
}
export class WtmDatePicker extends React.Component<IWtmDatePickerProps, any> {
  static wtmType = "DatePicker";
  onChange(data, dataString) {
    const onChange: any = this.props.onChange;
    // onChange && onChange(moment(dataString, this.getFormat()).valueOf(), data)
    onChange && onChange(dataString, data)
  }
  getFormat() {
    const { showTime, format } = this.props;
    if (format) {
      return format
    }
    return showTime ? "YYYY-MM-DD HH:mm:ss" : "YYYY-MM-DD";
  }
  render() {
    let props: any = {
      format: this.getFormat(),
      ...this.props
    }
    if (this.props.display) {
      return <span>{moment(props.value, props.format).format(props.format)}</span>
    }
    if (props.value) {
      // 转换 默认值
      if (lodash.isString(props.value)) {
        props.defaultValue = moment(props.value, props.format)
      }
    }
    // console.log("TCL: WtmRangePicker -> render -> props", this.props)
    delete props.value;
    return (<DatePicker  {...props} onChange={this.onChange.bind(this)} />);
  }
}

interface IWtmRangePickerProps extends DatePickerProps {
  display?: boolean;
  [key: string]: any;
}
export class WtmRangePicker extends React.Component<IWtmRangePickerProps, any> {
  static wtmType = "RangePicker";
  onChange(data, dataString) {
    const onChange: any = this.props.onChange;
    onChange && onChange(dataString, data)
  }
  getFormat() {
    const { showTime, format } = this.props;
    if (format) {
      return format
    }
    return showTime ? "YYYY-MM-DD HH:mm:ss" : "YYYY-MM-DD";
  }
  render() {
    let props: any = {
      format: this.getFormat(),
      ...this.props
    }
    if (this.props.display) {
      return <span>{moment(props.value, props.format).format(props.format)}</span>
    }
    if (props.value) {
      // 转换 默认值
      if (lodash.isString(props.value)) {
        props.defaultValue = moment(props.value, props.format)
      }
    }
    delete props.value;
    return (<RangePicker  {...props} onChange={this.onChange.bind(this)} />);
  }
}
export default WtmDatePicker
