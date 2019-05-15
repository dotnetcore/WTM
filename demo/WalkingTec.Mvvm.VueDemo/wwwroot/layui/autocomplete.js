layui.define(['jquery', 'laytpl', 'layer'], function (e) {
    "use strict";
    layui.link('/layui/css/autocomplete.css');
    var hint = layui.hint(),
        $ = layui.jquery,
        laytpl = layui.laytpl,
        layer = layui.layer,
        module = 'autocomplete',
        filter = 'layui-autocomplete',
        container = 'layui-form-autocomplete',
        container_focus = 'layui-form-autocomplete-focus',
        system = {
            config: {
                template: ['<div class="layui-form-autocomplete">', '<dl class="layui-anim layui-anim-upbit">', '</dl>', '</div>'].join(''),
                layout: ['<dd data-index="{{d.index}}">{{d.text}}</dd>'].join(''),
                template_txt: '{{d.text}}',
                template_val: '{{d.value}}',
                cache: false
            },
            index: layui.autocomplete ? layui.autocomplete.index + 1e4: 0,
            data: {},
        },
        callback = function() {
            var _self = this,
                _config = _self.config,
                _id = _config.id;
            return _id && (callback.config[_id] = _config), {
                config: _self.config
            }
        },
        job = function(e) {
            var _self = this;
            _self.index = ++system.index,
            _self.config = $.extend({}, _self.config, system.config, e),
            _self.render()
        };
    job.prototype.config = {
        text: {
            none: "无数据",
            loading: "加载中"
        },
        response: {
            code: 'Code',
            data: 'Data'
        },
        ajax: [],
        data: [],
        filter: ''
    },
    job.prototype.render = function() {
        var _self = this, _config = _self.config;
        if (_config.elem = $(_config.elem), _config.where = _config.where || {}, !_config.elem[0]) return _self;
        var _elem = _config.elem,
            _container = _elem.next('.' + container),
            _html = _self.elem = $(laytpl(_config.template).render({}));
        _config.id = _self.id, _container[0] && _container.remove(), _elem.attr('autocomplete', 'off'), _elem.after(_html);
        _self.events()
    },
    job.prototype.pullData = function () {
        var _self = this,
            _config = _self.config,
            _elem = _config.elem,
            _container = _elem.next('.' + container),
            _dom = _container.find('dl');
        if (!_config.filter) return _self.renderData([]);
        if (_config.cache && _config.data[_self.index]) return _self.renderData(_config.data[_self.index]);
        if (_config.cache && _config.ajax[_self.index] != undefined) return;
        (!_config.cache && _config.ajax[_self.index] != undefined) && _config.ajax[_self.index].abort(), _config.ajax[_self.index] = $.ajax({
            type: _config.method || "get",
            url: _config.url,
            data: {keywords: _config.filter},
            dataType: "json",
            beforeSend: function () {
                _container.addClass(container_focus), _dom.html(['<dd style="text-align: center" autocomplete-load>', _config.text.loading, '</dd>'].join(''))
            },
            success: function (resp) {
                return 200 != eval('resp.' + _config.response.code) ? layer.msg(eval('resp.' + _config.response.data)) : _config.data[_self.index] = eval('resp.' + _config.response.data), _self.renderData(_config.data[_self.index])
            },
            error: function (a,b,c) {
                //hint.error("请求失败")
            },
            complete: function () {
                delete _config.ajax[_self.index]
            }
        })
    },
    job.prototype.renderData = function (resp) {
        var _self = this,
            _config = _self.config,
            _elem = _config.elem,
            _container = _elem.next('.' + container),
            _dom = _container.find('dl'),
            _list = [];

        layui.each(resp, function (i, e) {
            (typeof e === "object" || typeof e === "array") ?
                $.each(e, function (_i, _e) {
                    try {
                        if (_e.indexOf(_config.filter) > -1) {
                            _list.push(laytpl(_config.layout).render({ index: i, text: laytpl(_config.template_txt).render(e) }));
                            return false;
                        }
                    }
                    catch(err){ }
                }) : (e.indexOf(_config.filter) > -1 && _list.push(laytpl(_config.layout).render({index: i, text: laytpl(_config.template_txt).render(e)})))
        });
        _dom.html(_list.join('')), _list.length > 0 ? _container.addClass(container_focus) : _container.removeClass(container_focus)
    },
    job.prototype.events = function () {
        var _self = this,
            _config = _self.config,
            _elem = _config.elem,
            _container = _elem.next('.' + container),
            _dom = _container.find('dl');
        _elem.on('focus', function () {
            _config.filter = this.value, _config.cache ? _self.pullData() : _self.renderData(_config.data[_self.index])
        }).on('input propertychange', function () {
            _config.filter = this.value, _self.pullData()
        }),
        $(document).on('click', function (e) {
            var _target = e.target, _item = _dom.find(_target), _e = _item.length > 0 ? _item.closest('dd') : undefined;
            if (_target === _elem[0]) return false;
            if (_e !== undefined) {
                if (_e.attr('autocomplete-load') !== undefined) return false;
                _elem.val(laytpl(_config.template_val).render(_config.data[_self.index][_e.index()])), _config.onselect == undefined || _config.onselect(_config.data[_self.index][_e.index()])
            }
            _container.removeClass(container_focus);
        })
    };
    callback.config = {},
    callback.job = {},
    system.init = function (e, c) {
        var c = c || {}, _self = this, _elems = $(e ? 'input[lay-filter="' + e + '"]': 'input[' + filter + ']');
        _elems.each(function (_i, _e) {
            var _elem = $(_e),
                _lay_data = _elem.attr('lay-data');
            try {
                _lay_data = new Function("return " + _lay_data)()
            } catch (ex) {
                return hint.error("autocomplete元素属性lay-data配置项存在语法错误：" + _lay_data)
            }
            var _config = $.extend({elem: this}, system.config, c, _lay_data);
            _config.url == undefined && (_config.data == undefined || _config.length === 0) && hint.error("autocomplete配置有误，缺少获取数据方式");
            system.render(_config);
        })
    },
    system.render = function (e) {
        var j = new job(e);
        return callback.call(j)
    }
    system.init(), e(module, system);
})