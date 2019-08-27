
/*eslint eqeqeq: ["error", "smart"]*/
DONOTUSE_TABLAYID = undefined;
if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (prefix) {
        return this.slice(0, prefix.length) === prefix;
    };
}
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

    GetCookie: function (name, allwindow) {
        if (allwindow) {
            return $.cookie(DONOTUSE_COOKIEPRE + name);
        }
        else {
            return $.cookie(DONOTUSE_COOKIEPRE + DONOTUSE_WINDOWGUID + name);

        }
    },

    GetSelections: function (gridId) {
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

    GetNonSelections: function (gridId) {
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

    GetSelectionData: function (gridId) {
        return layui.table.checkStatus(gridId).data;
    },

    GetNonSelectionData: function (gridId) {
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

    GetIsSelectAll: function (gridId) {
        return layui.table.checkStatus(gridId).isAll;
    },

    Alert: function (msg) {
        var layer = layui.layer;
        layer.alert(msg);
    },

    Msg: function (msg) {
        var layer = layui.layer;
        layer.msg(msg);
    },

    LoadPage: function (url, newwindow, title, para) {
        this.SetCookie("windowids", null);
        var layer = layui.layer;
        var index = layer.load(2);
        url = decodeURIComponent(url);
        furl = url;
        var re = /(\/_framework\/outside\?url=)(.*?)$/ig;
        url = url.replace(re, function (match, p1, p2) {
            return p1 + encodeURIComponent(p2);
        });

        if (newwindow === true || para !== undefined) {
            var getpost = "GET";
            if (para !== undefined) {
                getpost = "Post";
            }
            $.ajax({
                type: getpost,
                url: url,
                data: para,
                success: function (data, textStatus, request) {
                    if (request.getResponseHeader('IsScript') === 'true') {
                        eval(data);
                    }
                    else {
                        data = '<div class="layui-card donotuse_pdiv"><div class="layui-card-body donotuse_pdiv" id=\"' + $.cookie("divid") + '\" >' + data + "</div></div>";
                                var child = window.open("/Home/PIndex#/_framework/redirect");
                                child.document.close();
                                $(child.document).ready(function () {
                                    setTimeout(function () {
                                        $('#Lay_app_body_main', child.document).html(data);
                                        $(child.document).attr("title", title);
                                    }, 500);
                                });
                        }
                    layer.close(index);
                },
                error: function (a, b, c) {
                    layer.close(index);
                    if (a.responseText !== undefined && a.responseText !== "") {
                        layer.alert(a.responseText);
                    }
                    else {
                        layer.alert('加载失败');
                    }
                }
            });
        }
        else {
            location.hash = url;
        }
    },


    LoadPage1: function (url, where) {
        url = url.toLowerCase();
        if (url.indexOf("http://") === 0 || url.indexOf("https://") === 0) {
            $('#' + where).html("<iframe frameborder='no' border='0' height='100%' src='" + url + "'></iframe>");
        }
        else {
            var layer = layui.layer, index = layer.load(2);
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
        }
    },

    GetPostData: function (formid) {
        var richtextbox = $("#" + formid + " textarea");
        for (var i = 0; i < richtextbox.length; i++) {
            var ra = richtextbox[i].attributes['layeditindex'];
            if (ra !== undefined && ra != null) {
                var rindex = ra.value;
                layui.layedit.sync(rindex);
            }
        }


        var datastr = $('#' + formid).serialize();
        var checkboxes = $('#' + formid + ' :checkbox');
        for (i = 0; i < checkboxes.length; i++) {
            var ck = checkboxes[i];
            if (ck.checked === false && (ck.value === 'true' || ck.value === 'false')) {
                datastr += "&" + ck.name + "=false";
            }
        }
        return datastr;
    },

    RenderForm: function (formid) {
        var comboxs = $(".layui-form[lay-filter=" + formid + "] select[wtm-combo='MULTI_COMBO']");
        if (comboxs.length === 0) {
            layui.use(['form'], function () {
                var form = layui.form.render(null, formid);
            });
        }
        else {
            layui.use(['form', 'formSelects'], function () {
                var formSelects = layui.formSelects;
                var form = layui.form.render(null, formid);
                /* 启用 ComboBox 多选 */
                for (var i = 0; i < comboxs.length; i++) {
                    var filter = comboxs[i].attributes['lay-filter'].value, arr = [], subTag = $('input[name=""' + filter + '""]');
                    for (var a = 0; a < subTag.length; a++) {
                        arr.push({ name: subTag[a].attributes['text'].value, val: subTag[a].value });
                    }
                    formSelects.on({
                        layFilter: filter, left: '', right: '', separator: ',', arr: arr,
                        selectFunc: null
                    });
                }
            });
        }
    },

    PostForm: function (url, formid, divid) {
        var layer = layui.layer;
        var index = layer.load(2);
        if (url === undefined || url === "") {
            url = $("#" + formid).attr("action");
        }
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
                //if (wid == null || wid === "") {
                //    DONOTUSE_IGNOREHASH = true;
                //    window.location.hash = '#' + url;
                //}
                layer.close(index);
                if (request.getResponseHeader('IsScript') === 'true') {
                    eval(data);
                }
                else {
                    data = "<div id='" + $.cookie("divid") + "' class='layui-card-body donotuse_pdiv'>" + data + "</div>";
                    $("#" + divid).parent().html(data);
                }
            }
        });
    },

    BgRequest: function (url, para) {
        var layer = layui.layer;
        var index = layer.load(2);
        var getpost = "GET";
        if (para !== undefined) {
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
                eval(str);
            }
        });

    },

    OpenDialog: function (url, windowid, title, width, height, para) {
        var layer = layui.layer;
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
        var getpost = "GET";
        if (para !== undefined) {
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
                ff.SetCookie("windowids", owid);
                alert("加载失败");
            },
            success: function (str, textStatus, request) {
                layer.close(index);
                if (request.getResponseHeader('IsScript') === 'true') {
                    ff.SetCookie("windowids", owid);
                    eval(str);
                }
                else {
                    str = "<div id='" + $.cookie("divid") + "' class='donotuse_pdiv'>" + str + "</div>";
                    var area = 'auto';
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

    OpenDialog2: function (url, windowid, title, width, height, tempId, para) {
        var layer = layui.layer;
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
        var getpost = "GET";
        if (para !== undefined) {
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
                ff.SetCookie("windowids", owid);
                alert("加载失败");
            },
            success: function (str) {
                var regGridId = /<\s{0,}table\s+.*\s+id\s{0,}=\s{0,}"(.*)"\s+lay-filter="\1"\s{0,}>\s{0,}<\s{0,}\/\s{0,}table\s{0,}>/im;
                var regGridVar = /wtVar_(.*)\s{0,}=\s{0,}table.render\([a-zA-Z0-9_]{1,}option\)/im;
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
                var area = 'auto';
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

    CloseDialog: function () {
        var layer = layui.layer;
        var wid = this.GetCookie("windowids");
        if (wid !== null && wid !== '') {
            var windowid = wid.split(",").pop();
            var index = $('#' + windowid).parent('.layui-layer').attr("times");
            layer.close(index);
            this.SetCookie("windowids", wid);
        }
        else {
            if (layui.setter.pageTabs === false || $('.layadmin-tabsbody-item').length === 0) {
                $('#LAY_app_body').html('');
            }
            else {
                layui.admin.closeThisTabs();
            }
        }
    },

    LinkedChange: function (url, target, targetname) {
        $.get(url, {}, function (data, status) {
            if (status === "success") {
                var i = 0;
                var item = null;
                var form = layui.form;
                var controltype = "";
                var ele = $('#' + target);
                if (ele.length > 0) {
                    if (ele[0].localName === "select") {
                        controltype = "combo";
                    }
                    if (ele.attr("div-for") === "checkbox") {
                        controltype = "checkbox";
                    }
                    if (ele.attr("div-for") === "radio") {
                        controltype = "radio";
                    }
                }
                else {
                    if ($('#div' + target).length > 0) {
                        controltype = "tree";
                    }
                }

                if (controltype === "tree") {
                    layui.tree.reload('tree' + target, {
                        data: ff.getTreeItems(data.Data)
                    });
                }


                if (controltype === "combo") {
                    $('#' + target).html('<option value = "">请选择</option>');
                    for (i = 0; i < data.Data.length; i++) {
                        item = data.Data[i];
                        if (item.Selected === true) {
                            $('#' + target).append('<option value = "' + item.Value + '" selected>' + item.Text + '</option>');
                        }
                        else {
                            $('#' + target).append('<option value = "' + item.Value + '" >' + item.Text + '</option>');
                        }
                    }
                    form.render('select');
                }
                if (controltype === "checkbox") {
                    $('#' + target).html('');
                    for (i = 0; i < data.Data.length; i++) {
                        item = data.Data[i];
                        if (item.Selected === true) {
                            $('#' + target).append("<input type='checkbox'  name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "' checked />");
                        }
                        else {
                            $('#' + target).append("<input type='checkbox' name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "'  />");
                        }
                    }
                    form.render('checkbox');
                }
                if (controltype === "radio") {
                    $('#' + target).html('');
                    for (i = 0; i < data.Data.length; i++) {
                        item = data.Data[i];
                        if (item.Selected === true) {
                            $('#' + target).append("<input type='radio'  name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "' checked />");
                        }
                        else {
                            $('#' + target).append("<input type='radio' name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "'  />");
                        }
                    }
                    form.render('radio');
                }

            }
            else {
                layer.alert('获取数据失败');
            }
        });

    },

    GetFormArray: function (formId) {
        var searchForm = $('#' + formId), filter = [], fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            if (item.value !== null && item.value !== "")
                filter.push({ name: item.name, value: item.value });
        });
        return filter;
    },

    GetFormDataWithoutNull: function (formId) {
        var searchForm = $('#' + formId), filter = {}, fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            if (item.value !== null && item.value !== "") {
                if (filter.hasOwnProperty(item.name)) {
                    var temp = filter[item.name]
                    if (!(temp instanceof Array))
                        temp = [temp]
                    temp.push(item.value)
                    filter[item.name] = temp;
                }
                else {
                    filter[item.name] = item.value;
                }
            }
        });
        return filter;
    },

    GetFormData: function (formId) {
        var searchForm = $('#' + formId), filter = {}, fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            if (filter.hasOwnProperty(item.name)) {
                var temp = filter[item.name]
                if (!(temp instanceof Array))
                    temp = [temp]
                temp.push(item.value)
                filter[item.name] = temp;
            }
            else {
                filter[item.name] = item.value;
            }
        });
        return filter;
    },

    GetSearchFormData: function (formId, listvm) {
        var data = ff.GetFormData(formId);
        for (var attr in data) {
            if (attr.startsWith(listvm + ".")) {
                data[attr.replace(listvm + ".", "")] = data[attr];
                delete data[attr];
            }
        }
        return data;
    },

    DownloadExcelOrPdf: function (url, formId, defaultcondition, ids) {
        var formData = ff.GetSearchFormData(formId);
        $.extend(defaultcondition, formData);
        $.cookie("DONOTUSEDOWNLOADING", "1", { path: '/' });
        var form = $('<form method="POST" action="' + url + '">');
        for (var attr in defaultcondition) {
            if (defaultcondition[attr] != null) {
                form.append($('<input type="hidden" name="' + attr + '" value="' + defaultcondition[attr] + '">'));
            }
        }
        if (ids !== undefined && ids !== null) {
            for (var i = 0; i < ids.length; i++) {
                form.append($('<input type="hidden" name="Ids" value="' + ids[i] + '">'));
            }
        }
        $('body').append(form);
        form.submit();
        form.remove();
    },

    /**
     * RefreshGrid
     * @param {string} dialogid the dialogid
     * @param {number} index the grid index
     */
    RefreshGrid: function (dialogid, index) {
        if (index === undefined) {
            index = 0;
        }
        var tab = "";
        if (layui.setter.pageTabs === true && dialogid == "LAY_app_body") {
            tab = " .layadmin-tabsbody-item.layui-show";
        }
        var tables = $('#' + dialogid + tab + ' table[id]');
        if (tables.length > index) {
            layui.table.reload(tables[index].id);
        }
        else {
            var searchBtns = $('#' + dialogid + tab + ' form a[class*=layui-btn]');
            if (searchBtns.length > index) {
                searchBtns[index].click();
            }
        }
    },

    AddGridRow: function (gridid, option, data) {
        var loaddata = layui.table.cache[gridid];
        for (val in data) {
            if (val === "ID") {
                data[val] = ff.guid();
            }
        }
        for (val in data) {
            if (typeof (data[val]) == 'string') {
                data[val] = data[val].replace(/\[\d?\]/ig, "[" + loaddata.length + "]");
                data[val] = data[val].replace(/_\d?_/ig, "_" + loaddata.length + "_");
                var re = /(<input .*?)\s*\/>/ig;
                var re2 = /(<select .*?)\s*>(.*?<\/select>)/ig;
                var re3 = /(.*?)<input hidden name='(.*?)\.id' .*?\/>(.*?)/ig;
                data[val] = data[val].replace(re, "$1 onchange=\"ff.gridcellchange(this,'" + gridid + "'," + loaddata.length + ",'" + val + "',0)\" />");
                data[val] = data[val].replace(re2, "$1 onchange=\"ff.gridcellchange(this,'" + gridid + "'," + loaddata.length + ",'" + val + "',1)\" >$2");
                data[val] = data[val].replace(re3, "$1 <input hidden name=\"$2.id\" value='" + data["ID"] + "'/> $3");
            }
        }
        loaddata.push(data);
        option.url = null;
        option.data = loaddata;
        option.limit = 9999;
        layui.table.render(option);
    },

    LoadLocalData: function (gridid, option, datas) {
        for (var i = 0; i < datas.length; i++) {
            var data = datas[i];
            for (val in data) {
                if (typeof (data[val]) == 'string') {
                    data[val] = data[val].replace(/[$]{2}script[$]{2}/img, "<script>").replace(/[$]{2}#script[$]{2}/img, "</script>");
                }
            }
        }
        option.url = null;
        option.data = datas;
        option.limit = 9999;
        layui.table.render(option);
    },

    RemoveGridRow: function (gridid, option, index) {
        var loaddata = layui.table.cache[gridid];
        loaddata.splice(index - 1, 1);
        for (var i = 0; i < loaddata.length; i++) {
            for (val in loaddata[i]) {
                if (typeof (loaddata[i][val]) == 'string') {
                    loaddata[i][val] = loaddata[i][val].replace(/\[\d?\]/ig, "[" + i + "]");
                    loaddata[i][val] = loaddata[i][val].replace(/_\d?_/ig, "_" + i + "_");
                    loaddata[i][val] = loaddata[i][val].replace("/onchange=\".*?\"/", "onchange=\"ff.gridcellchange(this,'" + gridid + "'," + i + ",'" + val + "')\"");
                }
            }
        }
        option.url = null;
        option.data = loaddata;
        option.limit = 9999;
        layui.table.render(option);
    },

    gridcellchange: function (ele, gridid, row, col, celltype) {
        var loaddata = layui.table.cache[gridid];
        if (celltype === 0) {
            loaddata[row][col] = loaddata[row][col].replace(/value\s*=\s*'.*?'/i, "value='" + ele.value + "'");
        }
        if (celltype === 1) {
            loaddata[row][col] = loaddata[row][col].replace(/(<option .*?) selected\s*>/ig, "$1>");
            var re = new RegExp("(<option\\s*value\\s*=\\s*[\"']" + ele.value + "[\"'])\s*>", "ig");
            loaddata[row][col] = loaddata[row][col].replace(re, "$1 selected>");
        }

    },

    clearSelector: function (id) {
        $("#" + id).val("");
        $("#" + id + "_Display").val("");
        var vals = $('#' + "id" + '_Container input[type=hidden]');
        for (var i = 0; i < vals.length; i++) {
            vals[i].remove();
        }
    },

    setSelectorPara: function (id, obj) {
        eval(id + "filter = obj;");
    },

    guid: function () {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    },

    concatWhereStr: function (tempUrl, whereStr, data) {
        if (tempUrl == null) tempUrl = "";
        if (data == null) return tempUrl;
        if (whereStr != null && whereStr.length > 0) {
            for (var i = 0; i < whereStr.length; i++) {
                tempUrl = tempUrl + '&' + whereStr[i] + '=' + data[whereStr[i]];
            }
        }
        return tempUrl;
    },

    triggerResize: function () {
        setTimeout(function () {
            {
                if (typeof (Event) === 'function') {
                    {
                        window.dispatchEvent(new Event('resize'));
                    }
                } else {
                    {
                        var evt = window.document.createEvent('UIEvents');
                        evt.initUIEvent('resize', true, false, window, 0);
                        window.dispatchEvent(evt);
                    }
                }
            }
        }, 10);
    },

    getTreeChecked: function (items) {
        var rv = [];
        for (var i = 0; i < items.length; i++) {
            if (items[i].children == null || items[i].children.length == 0) {
                rv.push(items[i].id);
            }
            else {
                rv = rv.concat(this.getTreeChecked(items[i].children));
            }
        }
        return rv;
    },

    getTreeItems: function (data) {
        var rv = [];
        for (var i = 0; i < data.length; i++) {
            var item = {};
            item.id = data[i].Id;
            item.title = data[i].Text;
            item.href = data[i].Url;
            item.spread = data[i].Expended;
            item.checked = data[i].Checked;

            if (data[i].Children != null && data[i].Children.length > 0) {
                item.children = this.getTreeItems(data[i].Children);
            }
            rv.push(item);
        }
        return rv;

    }
};
