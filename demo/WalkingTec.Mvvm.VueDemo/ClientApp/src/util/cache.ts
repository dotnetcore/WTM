const defaultCache = {
    setCookie(name, value, days?) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
            expires = "; expires=" + date["toGMTString"]();
        }
        document.cookie = name + "=" + value + expires + "; path=/";
    },
    setCookieJson(name, value, days?) {
        const val = JSON.stringify(value);
        this.setCookie(name, val, days);
    },
    getCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(";");
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === " ") c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) {
                return decodeURIComponent(c.substring(nameEQ.length, c.length));
            }
        }
        return null;
    },
    getCookieJson(name) {
        const val = this.getCookie(name) || "{}";
        return JSON.parse(val);
    },
    setStorage(key, value) {
        if (typeof value === "object") {
            value = JSON.stringify(value);
        }
        localStorage.setItem(key, value);
    },
    getStorage(key, isJson: boolean = false) {
        const value = localStorage.getItem(key);
        if (isJson) {
            return JSON.parse(value || "");
        } else {
            return value;
        }
    },
    clearStorage() {
        localStorage.clear();
    }
};
export default defaultCache;
