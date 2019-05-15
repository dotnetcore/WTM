// 补0
const fillZero = (num, len) => {
    return (Array(len).join('0') + num).slice(-len);
};
const getYMDW = date => {
    const year = date.getFullYear();
    // fillZero(date.getDate(), 2);
    // fillZero(date.getMonth() + 1, 2);
    const month = date.getMonth() + 1;
    const day = date.getDate();
    const week = date.getDay();
    const hours = date.getHours();
    const minutes = date.getMinutes();
    const seconds = date.getSeconds();
    let weekVal = '';
    switch (week) {
        case 0:
            weekVal = '星期日';
            break;
        case 1:
            weekVal = '星期一';
            break;
        case 2:
            weekVal = '星期二';
            break;
        case 3:
            weekVal = '星期三';
            break;
        case 4:
            weekVal = '星期四';
            break;
        case 5:
            weekVal = '星期五';
            break;
        case 6:
            weekVal = '星期六';
            break;
    }
    return { year, month, day, weekVal, hours, minutes, seconds };
};
// 当前时间 在时间段内判断
const timeRange = function(beginTime, endTime) {
    var strb = beginTime.split(':');
    if (strb.length != 2) {
        return false;
    }
    var stre = endTime.split(':');
    if (stre.length != 2) {
        return false;
    }
    var b = new Date();
    var e = new Date();
    var n = new Date();
    b.setHours(strb[0]);
    b.setMinutes(strb[1]);
    e.setHours(stre[0]);
    e.setMinutes(stre[1]);
    if (n.getTime() - b.getTime() > 0 && n.getTime() - e.getTime() < 0) {
        return true;
    } else {
        // console.log("当前时间是：" + n.getHours() + ":" + n.getMinutes() + "，不在该时间范围内！");
        return false;
    }
};
/**
 * 日期格式化
 * @param phone
 */
const toFormat = (dateStr, fmt = 'yyyy-MM-dd hh:mm:ss') => {
    const date = new Date(dateStr);
    //author: meizz
    const o = {
        'M+': date.getMonth() + 1, //月份
        'd+': date.getDate(), //日
        'h+': date.getHours(), //小时
        'm+': date.getMinutes(), //分
        's+': date.getSeconds(), //秒
        'q+': Math.floor((date.getMonth() + 3) / 3), //季度
        S: date.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(
            RegExp.$1,
            (date.getFullYear() + '').substr(4 - RegExp.$1.length)
        );
    }
    for (const k in o) {
        if (new RegExp('(' + k + ')').test(fmt)) {
            fmt = fmt.replace(
                RegExp.$1,
                RegExp.$1.length == 1
                    ? o[k]
                    : ('00' + o[k]).substr(('' + o[k]).length)
            );
        }
    }
    return fmt;
};
export default { getYMDW, fillZero, timeRange, toFormat };
