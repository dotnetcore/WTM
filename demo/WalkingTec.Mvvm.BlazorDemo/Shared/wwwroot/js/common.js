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

    $.extend({
        _showToast: function () {
            var $toast = $('.row .toast').toast('show');
            $toast.find('.toast-progress').css({ "width": "100%" });
        },
        highlight: function (el) {
            var $el = $(el);
            $el.find('[data-toggle="tooltip"]').tooltip();
            var code = $el.find('code')[0];
            if (code) {
                hljs.highlightBlock(code);
            }
        },
        copyText: function (ele) {
            if (navigator.clipboard) {
                navigator.clipboard.writeText(ele);
            }
            else {
                if (typeof ele !== "string") return false;
                var input = document.createElement('input');
                input.setAttribute('type', 'text');
                input.setAttribute('value', ele);
                document.body.appendChild(input);
                input.select();
                document.execCommand('copy');
                document.body.removeChild(input);
            }
        },
        _initChart: function (el, obj, method) {
            var showToast = false;
            var handler = null;
            $(document).on('chart.afterInit', '.chart', function () {
                showToast = $(this).height() < 200;
                if (handler != null) window.clearTimeout(handler);
                if (showToast) {
                    handler = window.setTimeout(function () {
                        if (showToast) {
                            obj.invokeMethodAsync(method);
                        }
                    }, 1000);
                }
            });
        },
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
        indexTyper: function (el) {
            var $this = $(el);
            var $cursor = $this.next();

            var typeChar = function (original, reverse) {
                var plant = original.concat();
                return new Promise(function (resovle, reject) {
                    $cursor.addClass('active');
                    var eventHandler = window.setInterval(function () {
                        if (plant.length > 0) {
                            if (!reverse) {
                                var t1 = $this.text() + plant.shift();
                                $this.text(t1);
                            }
                            else {
                                var t1 = plant.pop();
                                $this.text(plant.join(''));
                            }
                        }
                        else {
                            window.clearInterval(eventHandler);
                            $cursor.removeClass('active');

                            var handler = window.setTimeout(function () {
                                window.clearTimeout(handler);
                                if (reverse) {
                                    return resovle();
                                }
                                else {
                                    typeChar(original, true).then(function () {
                                        return resovle();
                                    });
                                }
                            }, 1000);
                        }
                    }, 200);
                });
            };

            var text1 = ['最', '好', '用', '的'];
            var text2 = ['最', '好', '看', '的'];
            var text3 = ['最', '简', '单', '实', '用', '的'];

            var loop = function () {
                var handler = window.setTimeout(function () {
                    window.clearTimeout(handler);
                    typeChar(text1, false).then(function () {
                        typeChar(text2, false).then(function () {
                            typeChar(text3).then(function () {
                                loop();
                            });
                        });
                    });
                }, 200);
            };

            loop();
            $.carouselHome();
        },
        carouselHome: function () {
            var $ele = $('#carouselExampleCaptions');
            var leaveHandler = null;
            $ele.hover(function () {
                if (leaveHandler != null) {
                    window.clearTimeout(leaveHandler);
                }

                var $this = $(this);
                var $bar = $this.find('[data-slide]');
                $bar.removeClass('d-none');
                var hoverHandler = window.setTimeout(function () {
                    window.clearTimeout(hoverHandler);
                    $this.addClass('hover');
                }, 10);
            }, function () {
                var $this = $(this);
                var $bar = $this.find('[data-slide]');
                $this.removeClass('hover');
                leaveHandler = window.setTimeout(function () {
                    window.clearTimeout(leaveHandler);
                    $bar.addClass('d-none');
                }, 300);
            });

            $('.welcome-footer [data-toggle="tooltip"]').tooltip();
        },
        table_wrap: function () {
            var handler = window.setInterval(function () {
                var spans = $('body').find('.table-wrap-header-demo th .table-cell span');
                if (spans.length === 0) {
                    return;
                }

                window.clearInterval(handler);
                spans.each(function () {
                    $(this).tooltip({
                        title: $(this).text()
                    });
                });
            }, 500);
        },
        tooltip: function () {
            $('[data-toggle="tooltip"]').tooltip();
        },
        table_test: function (el, obj, method) {
            var $el = $(el);
            $el.on('click', 'tbody tr', function () {
                $el.find('.active').removeClass('active');
                var index = $(this).addClass('active').data('index');

                obj.invokeMethodAsync(method, index);
            });
        },
        initTheme: function (el) {
            var $el = $(el);

            $el.on('click', '.btn-theme, .theme-close, .theme-item', function (e) {
                var $theme = $el.find('.theme-list');
                $theme.toggleClass('is-open').slideToggle('fade');
            });
        },
        setTheme: function (css, cssList) {
            var $link = $('link').filter(function (index, link) {
                var href = $(link).attr('href');
                return href === '_content/BootstrapBlazor/css/bootstrap.blazor.bundle.min.css';
            });
            var $targetLink = $link.next();
            if ($link.length == 1) {
                if (css === '') {
                    // remove
                    var $theme = cssList.filter(function (theme) {
                        return $targetLink.attr('href') === theme;
                    });
                    if ($theme.length === 1) {
                        $targetLink.remove();
                    }
                }
                else {
                    // append
                    $link.after('<link rel="stylesheet" href="' + css + '">')
                }
            }
        }
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
