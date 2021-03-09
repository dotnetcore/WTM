import date from "@/util/date";
// 时间格式
const formatTime = (
  value,
  customFormat: string = "yyyy-MM-dd hh:mm:ss",
  isMsec: boolean = true
) => {
  // customFormat 要展示的时间格式
  // isMsec----传入的value值是否是毫秒
  value = isMsec ? value : value * 1000;
  return date.toFormat(value, customFormat);
};
export default formatTime;
