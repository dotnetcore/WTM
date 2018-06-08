window.ff = {
    SetCookie: function (name, value, allwindow) {
        var cookiePrefix = '', windowGuid = '';

        if ("undefined" !== typeof DONOTUSE_COOKIEPRE) {
            cookiePrefix = DONOTUSE_COOKIEPRE
        }
        if ("undefined" !== typeof DONOTUSE_WINDOWGUID) {
            windowGuid = DONOTUSE_WINDOWGUID
        }

        if (allwindow) {
            $.cookie(cookiePrefix + name, value);
        }
        else {
            $.cookie(cookiePrefix + windowGuid + name, value);
        }
    },

    GetCookie: function(name, allwindow) {
        if (allwindow) {
            return $.cookie(DONOTUSE_COOKIEPRE + name);
        }
        else {
            return $.cookie(DONOTUSE_COOKIEPRE + DONOTUSE_WINDOWGUID + name);

        }
    },

    /**
     * 获取所有选中的 Id
     * @param {string} gridId
     * @returns {Array<string>}
     */
    GetSelections: function(gridId) {
        var checkStatus = layui.table.checkStatus(gridId);
        var data = checkStatus.data;
        var ids = [];
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                ids.push(data[i].ID);
            }
        }
        return ids;
    },

    /**
     * 获取所有未选中的 Id
     * @param {string} gridId
     * @returns {Array<string>}
     */
    GetNonSelections: function(gridId) {
        var nums = 0 // 未选中个数
            , invalidNum = 0
            , ids = [] // 未选中id
            , data = table.cache[gridId] || [];
        //计算未选中个数
        layui.each(data, function (i, item) {
            if (item.constructor === Array) {
                invalidNum++; //无效数据，或已删除的
                return;
            }
            if (!item[table.config.checkName]) {
                nums++;
                ids.push(item.ID);
            }
        });
        return ids;
    },

    /**
     * 获取所有选中的数据
     * @param {string} gridId
     * @returns {Array<object>}
     */
    GetSelectionData: function(gridId) {
        return layui.table.checkStatus(gridId).data;
    },

    /**
     * 获取所有未选中的数据
     * @param {string} gridId
     * @returns {Array<object>}
     */
    GetNonSelectionData: function(gridId) {
        var nums = 0 // 未选中个数
            , invalidNum = 0
            , arr = [] // 未选中数据
            , data = table.cache[gridId] || [];
        //计算未选中个数
        layui.each(data, function (i, item) {
            if (item.constructor === Array) {
                invalidNum++; //无效数据，或已删除的
                return;
            }
            if (!item[table.config.checkName]) {
                nums++;
                arr.push(table.clearCacheKey(item));
            }
        });
        return arr;
    },

    /**
     * 是否全部选中
     * @param {string} gridId
     * @returns {boolean}
     */
    GetIsSelectAll: function(gridId) {
        return layui.table.checkStatus(gridId).isAll;
    },

    /**
     * 提示消息
     * @param {string} msg
     */
    Alert: function(msg) {
        let layer = layui.layer;
        layer.alert(msg);
    },

    /**
     * 提示消息
     * @param {string} msg
     */
    Msg: function(msg) {
        let layer = layui.layer;
        layer.msg(msg);
    },

    LoadPage: function(url) {
        this.SetCookie("windowids", null);
        let layer = layui.layer;
        var index = layer.load(2);
        url = decodeURIComponent(url);
        DONOTUSE_IGNOREHASH = true;

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.location.hash = '#' + url;
                $('#DONOTUSE_MAINPANEL').html(data);
                $('#DONOTUSE_MAINPANEL').scrollTop(0);
                layer.close(index);
            },
            error: function (data) {
                layer.close(index);
                layer.alert('加载失败');
            }
        });
    },


    LoadPage1: function(url, where) {
        let layer = layui.layer, index = layer.load(2);
        $.ajax({
            url: decodeURIComponent(url),
            type: 'GET',
            success: function (data) {
                $('#' + where).html(data);
                layer.close(index);
            },
            error: function (xhr, status, error) {
                layer.close(index);
               layer.alert('加载失败');
            },
            complete: function () {
                ff.SetCookie("windowids", null);
            }
        });
    },

    GetPostData: function(formid) {
        let datastr = $('#' + formid).serialize();
        let checkboxes = $('#' + formid + ' :checkbox');
        for (var i = 0; i < checkboxes.length; i++) {
            let ck = checkboxes[i];
            if (ck.checked === false && (ck.value == true || ck.value == false)) {
                datastr += "&" + ck.name + "=false";
            }
        }
        return datastr;
    },

    PostForm: function(url, formid, divid) {
        let layer = layui.layer;
        let index = layer.load(2);
        $.ajax({
            cache: false,
            type: "POST",
            url: url,
            data: ff.GetPostData(formid),
            async: true,
            error: function (request) {
                layer.close(index);
                alert("提交失败");
            },
            success: function (data, textStatus, request) {
                var wid = ff.GetCookie("windowids");
                if (wid == null || wid == "") {
                    DONOTUSE_IGNOREHASH = true;
                    window.location.hash = '#' + url;
                }
                layer.close(index);
                if (request.getResponseHeader('IsScript') === 'true') {
                    eval(data);
                }
                else {
                    $("#" + divid).parent().html(data);
                }
            }
        });
    },

    OpenDialog: function (url, windowid, title, width, height, para) {
        let layer = layui.layer;
        var index = layer.load(2);
        var wid = this.GetCookie("windowids");
        var owid = wid;
        if (wid === null || wid === '') {
            wid = windowid;
        }
        else {
            wid += "," + windowid;
        }
        this.SetCookie("windowids", wid);
        this.SetCookie("windowguid", DONOTUSE_WINDOWGUID, true);
        let getpost = "GET";
        if (para != undefined) {
            getpost = "Post";
        }

        $.ajax({
            cache: false,
            type: getpost,
            url: url,
            data: para,
            async: true,
            error: function (request) {
                layer.close(index);
                alert("加载失败");
            },
            success: function (str, textStatus, request) {
                layer.close(index);
                if (request.getResponseHeader('IsScript') === 'true') {
                    eval(str);
                }
                else {
                    let area = 'auto';
                    if (width !== undefined && width !== null && height !== undefined && height !== null) {
                        area = [width + 'px', height + 'px'];
                    }
                    if (width !== undefined && width !== null && (height === undefined || height === null)) {
                        area = width + 'px';
                    }
                    if (title === undefined || title === null || title === '') {
                        title = false;
                    }
                    layer.open({
                        type: 1
                        , title: title //不显示标题栏
                        , area: area
                        , shade: 0.8
                        , id: windowid //设定一个id，防止重复弹出
                        , content: str
                        , end: function () {
                            ff.SetCookie("windowids", owid);
                        }
                    });
                }
            }
        });
    },

    OpenDialog2: function(url, windowid, title, width, height, tempId, para) {
        let layer = layui.layer;
        var index = layer.load(2);
        var wid = this.GetCookie("windowids");
        var owid = wid;
        if (wid === null || wid === '') {
            wid = windowid;
        }
        else {
            wid += "," + windowid;
        }
        this.SetCookie("windowids", wid);
        this.SetCookie("windowguid", DONOTUSE_WINDOWGUID, true);
        let getpost = "GET";
        if (para != undefined) {
            getpost = "Post";
        }

        $.ajax({
            cache: false,
            type: getpost,
            url: url,
            data: para,
            async: true,
            error: function (request) {
                layer.close(index);
                alert("加载失败");
            },
            success: function (str) {
                var regGridId = /<\s{0,}table\s+id\s{0,}=\s{0,}"(.*)"\s+lay-filter/im;
                var regGridVar = /wtVar_(.*)\s{0,}=\s{0,}table.render\({/im;
                if ($(tempId).length > 0 && regGridId.test(str) && regGridVar.test(str)) {
                    // 获取gridId
                    var gridId = regGridId.exec(str)[1];
                    var gridVar = 'wtVar_' + regGridVar.exec(str)[1];
                    var template = $(tempId)[0].innerHTML;
                    template = template.replace(/[$]{2}script[$]{2}/img, "<script>").replace(/[$]{2}#script[$]{2}/img, "</script>");
                    //替换gridId
                    template = template.replace(/table[.]reload\('(.*)',\s{0,}{/img, 'table.reload(\'' + gridId + '\',{');
                    //替换grid参数变量
                    template = template.replace(/\$.extend\((.*)[.]config[.]where,/img, '$.extend(' + gridVar + '.config.where,');
                    str = str.replace('$$SearchPanel$$', template);
                }
                layer.close(index);
                let area = 'auto';
                if (width !== undefined && width !== null && height !== undefined && height !== null) {
                    area = [width + 'px', height + 'px'];
                }
                if (width !== undefined && width !== null && (height === undefined || height === null)) {
                    area = width + 'px';
                }
                if (title === undefined || title === null || title === '') {
                    title = false;
                }
                layer.open({
                    type: 1
                    , title: title //不显示标题栏
                    , area: area
                    , shade: 0.8
                    , id: windowid //设定一个id，防止重复弹出
                    , content: str
                    , end: function () {
                        ff.SetCookie("windowids", owid);
                    }
                });
            }
        });
    },

    CloseDialog: function() {
        let layer = layui.layer;
        var wid = this.GetCookie("windowids");
        if (wid !== null && wid !== '') {
            let windowid = wid.split(",").pop();
            let index = $('#' + windowid).parent('.layui-layer').attr("times");
            layer.close(index);
            this.SetCookie("windowids", wid);
        }
        else {
            $('#DONOTUSE_MAINPANEL').html('');
        }
    },

    CloseAllDialog: function() {
        let layer = layui.layer;
        layer.closeAll();
    },

    LinkedChange: function(url, target) {
        $.get(url, {}, function (data, status) {
            if (status === "success") {
                $('#' + target).html('<option value = "">请选择</option>');
                //for (let item of data.data) {
                for (var i = 0; i < data.data.length; i++) {
                    var item = data.data[i];
                    if (item.selected == true) {
                        $('#' + target).append('<option value = "' + item.value + '" selected>' + item.text + '</option>');
                    }
                    else {
                        $('#' + target).append('<option value = "' + item.value + '" >' + item.text + '</option>');
                    }
                }
                var form = layui.form;
                form.render('select');
            }
            else {
                layer.alert('获取数据失败');
            }
        });

    },

    /**
     * 获取表单数据数组
     * @param {string} formId
     * @returns {Array<object>}
     */
    GetFormArray: function(formId) {
        var searchForm = $('#' + formId), filter = [], fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            if (item.value !== null && item.value !== "")
                filter.push({ name: item.name, value: item.value });
        });
        return filter;
    },

    /**
     * 获取表单数据
     * @param {string} formId
     * @returns {object}
     */
    GetFormDataWithoutNull: function(formId) {
        var searchForm = $('#' + formId), filter = {}, fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            if (item.value !== null && item.value !== "")
                filter[item.name] = item.value;
        });
        return filter;
    },

    /**
     * 获取表单数据
     * @param {string} formId
     * @returns {object}
     */
    GetFormData: function(formId) {
        var searchForm = $('#' + formId), filter = {}, fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            filter[item.name] = item.value;
        });
        return filter;
    },

    /**
     * 获取搜索表单数据
     * @param {string} formId
     * @param {string} listvm
     * @returns {object}
     */
    GetSearchFormData: function(formId, listvm) {
        let data = ff.GetFormData(formId);
        for (var attr in data) {
            if (attr.startsWith(listvm + ".")) {
                data[attr.replace(listvm + ".", "")] = data[attr];
                delete data[attr];
            }
        }
        return data;
    },

    /**
     * 下载 Excel 或者 Pdf
     * @param {string} url
     * @param {string} formId
     */
    DownloadExcelOrPdf: function(url, formId) {
        var formData = ff.GetFormArray(formId);
        for (var i = 0; i < formData.length; i++) {
            url = url + "&" + formData[i].name + "=" + formData[i].value;
        }
        $.cookie("DONOTUSEDOWNLOADING", "1", { path: '/' });
        var aTag = $('<a>');
        aTag.attr("href", url);
        $("body").append(aTag);
        aTag[0].click();
        aTag.remove();
    },

    /**
     * RefreshGrid
     * @param {string} dialogid
     */
    RefreshGrid: function(dialogid) {
        const tables = $('#' + dialogid + ' table');
        if (tables.length > 0) {
            table.reload(tables[0].id);
        }
        else {
            var searchBtns = $('#' + dialogid + ' form a[class*=layui-btn]');
            if (searchBtns.length > 0) {
                searchBtns[0].click();
            }
        }
    }
}

//window.ff = new WalkingTecUI();
//window.onbeforeunload = function () {
//    ff.SetCookie("windowids", null);
//    ff.SetCookie("toplayerindex", null);
//}