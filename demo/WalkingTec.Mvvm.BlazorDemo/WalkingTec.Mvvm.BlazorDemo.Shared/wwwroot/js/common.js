(function ($) {
    $.extend({
        loading: function () {
            var $loader = $("#loading");
            if ($loader.length > 0) {
                $loader.addClass("is-done");
                var handler = window.setTimeout(function () {
                    window.clearTimeout(handler);
                    $loader.remove();
                    $('body').removeClass('overflow-hidden');
                }, 600);
            }
        },
    });
})(jQuery);

window.localStorageFuncs = {
    set: function (key, value) {
        localStorage.setItem(key, value);
    },
    get: function (key, start) {
        if (start === undefined) {
            start = 0;
        }
        var t = localStorage.getItem(key);
        if (t == null) {
            return null;
        }
        var rv;
        if (t.length - start > 20000) {
            rv = t.substring(start, start+20000);
        }
        else {
            rv = t.substring(start);
        }
        return rv;
    },
    remove: function (key) {
        localStorage.removeItem(key);
    }
};

window.urlFuncs = {
    getCurrentUrl: function () {
        return window.location.href;
    },
    redirect: function (url) {
        window.location.href = url;
    },
    refresh: function () {
        window.location.reload();
    },
    download: function (url, data, method = "POST") {
        var xhr = new XMLHttpRequest();
        xhr.open(method, url, true);    // 也可以使用POST方式，根据接口
        xhr.responseType = "blob";  // 返回类型blob
        xhr.setRequestHeader('content-type', 'application/json');
        xhr.setRequestHeader('Authorization', 'Bearer ' + localStorageFuncs.get("wtmtoken"));
        xhr.onload = function () {
            if (this.status === 200) {
                var fname = this.getResponseHeader("content-disposition");
                var filename = "";
                if (/filename\*=UTF-8''(.*?)($|;|\s)/.test(fname)) {
                    filename = RegExp.$1;
                }
                else if (/filename=(.*?)($|;|\s)/.test(fname))
                {
                    filename = RegExp.$1;
                }
                filename = decodeURI(filename);
                var blob = this.response;
                var reader = new FileReader();
                reader.readAsDataURL(blob);  
                reader.onload = function (e) {
                    var a = document.createElement('a');
                    a.download = filename;
                    a.href = e.target.result;
                    $("body").append(a);
                    a.click();
                    $(a).remove();
                }
            }
        };
        // 发送ajax请求
        xhr.send(data)
    }
}
