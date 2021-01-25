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
    get: function (key) {
        return localStorage.getItem(key);
    }
};
