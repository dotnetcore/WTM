// Parse the time to string
export var parseTime = function (time, cFormat) {
    if (time === undefined) {
        return null;
    }
    var format = cFormat || '{y}-{m}-{d} {h}:{i}:{s}';
    var date;
    if (typeof time === 'object') {
        date = time;
    }
    else {
        if (typeof time === 'string' && /^[0-9]+$/.test(time)) {
            time = parseInt(time);
        }
        if (typeof time === 'number' && time.toString().length === 10) {
            time = time * 1000;
        }
        date = new Date(time);
    }
    var formatObj = {
        y: date.getFullYear(),
        m: date.getMonth() + 1,
        d: date.getDate(),
        h: date.getHours(),
        i: date.getMinutes(),
        s: date.getSeconds(),
        a: date.getDay()
    };
    var timeStr = format.replace(/{(y|m|d|h|i|s|a)+}/g, function (result, key) {
        var value = formatObj[key];
        // Note: getDay() returns 0 on Sunday
        if (key === 'a') {
            return ['日', '一', '二', '三', '四', '五', '六'][value];
        }
        if (result.length > 0 && value < 10) {
            return '0' + value;
        }
        return String(value) || '0';
    });
    return timeStr;
};
// Format and filter json data using filterKeys array
export var formatJson = function (filterKeys, jsonData) {
    return jsonData.map(function (data) { return filterKeys.map(function (key) {
        if (key === 'timestamp') {
            return parseTime(data[key]);
        }
        else {
            return data[key];
        }
    }); });
};
// Check if an element has a class
export var hasClass = function (ele, className) {
    return !!ele.className.match(new RegExp('(\\s|^)' + className + '(\\s|$)'));
};
// Add class to element
export var addClass = function (ele, className) {
    if (!hasClass(ele, className))
        ele.className += ' ' + className;
};
// Remove class from element
export var removeClass = function (ele, className) {
    if (hasClass(ele, className)) {
        var reg = new RegExp('(\\s|^)' + className + '(\\s|$)');
        ele.className = ele.className.replace(reg, ' ');
    }
};
// Toggle class for the selected element
export var toggleClass = function (ele, className) {
    if (!ele || !className) {
        return;
    }
    var classString = ele.className;
    var nameIndex = classString.indexOf(className);
    if (nameIndex === -1) {
        classString += '' + className;
    }
    else {
        classString =
            classString.substr(0, nameIndex) +
                classString.substr(nameIndex + className.length);
    }
    ele.className = classString;
};
//# sourceMappingURL=index.js.map