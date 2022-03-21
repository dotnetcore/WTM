
/*eslint eqeqeq: ["error", "smart"]*/
DONOTUSE_TABLAYID = undefined;
if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (prefix) {
        return this.slice(0, prefix.length) === prefix;
    };
}
if (typeof Array.prototype.removeByID != 'function') {
    Array.prototype.removeByID = function (val) {
        var index = -1;
        for (var i = 0; i < this.length; i++) {
            if (this[i].ID == val.ID) {
                index = i;
                break;
            }
        }
        if (index > -1) {
            this.splice(index, 1);
        }
    };
}

window.ff = {
    DONOTUSE_Text_LoadFailed: "",
    DONOTUSE_Text_SubmitFailed: "",
    DONOTUSE_Text_PleaseSelect: "",
    DONOTUSE_Text_FailedLoadData: "",

    SetCookie: function (name, value, allwindow) {
        try {
            var cookiePrefix = '', windowGuid = '';

            if ("undefined" !== typeof DONOTUSE_COOKIEPRE) {
                cookiePrefix = DONOTUSE_COOKIEPRE;
            }
            if ("undefined" !== typeof DONOTUSE_WINDOWGUID) {
                windowGuid = DONOTUSE_WINDOWGUID;
            }

            if (allwindow) {
                $.cookie(cookiePrefix + name, value);
            }
            else {
                $.cookie(cookiePrefix + windowGuid + name, value);
            }
        }
        catch (e) { }
    },

    GetCookie: function (name, allwindow) {
        try {
            var cookiePrefix = '', windowGuid = '';
            if ("undefined" !== typeof DONOTUSE_COOKIEPRE) {
                cookiePrefix = DONOTUSE_COOKIEPRE;
            }
            if ("undefined" !== typeof DONOTUSE_WINDOWGUID) {
                windowGuid = DONOTUSE_WINDOWGUID;
            }
            if (allwindow) {
                return $.cookie(cookiePrefix + name);
            }
            else {
                return $.cookie(cookiePrefix + windowGuid + name);

            }
        }
        catch (e) { }
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
        var table = layui.table
            , nums = 0 
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
        var table = layui.table
            , nums = 0 // 未选中个数
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

    Alert: function (msg, title) {
        var layer = layui.layer;
        if (title != undefined) {
            layer.alert(msg, { title: title });
        }
        else {
            layer.alert(msg);
        }
    },

    Msg: function (msg, title) {
        var layer = layui.layer;
        if (title != undefined) {
            layer.msg(msg, { title: title });
        }
        else {
            layer.msg(msg);
        }
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
                                $('#LAY_app_body', child.document).html(data);
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
                        layer.alert(ff.DONOTUSE_Text_LoadFailed);
                    }
                }
            });
        }
        else {
            layer.close(index);
            location.hash = url;
        }
    },


    LoadPage1: function (url, where) {
        url = url.toLowerCase();
        if (url.indexOf("http://") === 0 || url.indexOf("https://") === 0) {
            $('#' + where).html("<iframe frameborder='no' border='0' height='100%' src='" + url + "'></iframe>");
            $('#' + where).css("overflow-y", "auto");
        }
        else {
            var layer = layui.layer, index = layer.load(2);
            $.ajax({
                url: decodeURIComponent(url),
                type: 'GET',
                success: function (data) {
                    $('#' + where).html(data);
                    $('#' + where).css("overflow-y", "scroll");
                    layer.close(index);
                },
                error: function (xhr, status, error) {
                    layer.close(index);
                    layer.alert(ff.DONOTUSE_Text_LoadFailed);
                },
                complete: function () {
                    ff.SetCookie("windowids", null);
                }
            });
        }
    },

    GetPostData: function (formId) {
        var richtextbox = $("#" + formId + " textarea");
        for (var i = 0; i < richtextbox.length; i++) {
            var ra = richtextbox[i].attributes['layeditindex'];
            if (ra !== undefined && ra != null) {
                var rindex = ra.value;
                layui.layedit.sync(rindex);
            }
        }
        var combobox = $('#' + formId + ' :checkbox');

        var datastr = $('#' + formId).serialize();
        var checkboxes = $('#' + formId + ' :checkbox');
        for (i = 0; i < checkboxes.length; i++) {
            var ck = checkboxes[i];
            if (ck.checked === false && (ck.value === 'true' || ck.value === 'false')) {
                datastr += "&" + ck.name + "=false";
            }
        }
        return datastr;
    },

    RenderForm: function (formId) {
        var comboxs = $(".layui-form[lay-filter=" + formId + "] div[wtm-ctype='combo']");
            layui.use(['form'], function () {
                var form = layui.form.render(null, formId);
            });
    },

    PostForm: function (url, formId, divid, searchervm) {
        var layer = layui.layer;
        var index = layer.load(2);
        if (url === undefined || url === "") {
            url = $("#" + formId).attr("action");
        }
        var d = null;
        if (searchervm !== undefined && searchervm !== null && searchervm !== "") {
            d = ff.GetSearchFormData(formId, searchervm);
        }
        else {
            d = ff.GetFormData(formId)
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: url,
            data: d,
            async: true,
            error: function (request) {
                layer.close(index);
                alert(ff.DONOTUSE_Text_SubmitFailed);
            },
            success: function (data, textStatus, request) {
                if (request.getResponseHeader('IsScript') === 'true') {
                    eval(data);
                }
                else {
                    var inlayer = $("#" + formId).parents(".layui-layer-content");
                    if (inlayer !== undefined && inlayer.length > 0) {
                        data = "<div id='" + $.cookie("divid") + "' class='donotuse_pdiv'>" + data + "</div>";
                    } else {
                        data = "<div id='" + $.cookie("divid") + "' class='layui-card-body donotuse_pdiv'>" + data + "</div>";
                    }
                    $("#" + divid).parent().html(data);
                }
                layer.close(index);
            }
        });
    },

    BgRequest: function (url, para, divid) {
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
                if (request.responseText !== undefined && request.responseText !== "") {
                    layer.alert(request.responseText);
                }
                else {
                    layer.alert(ff.DONOTUSE_Text_LoadFailed);
                }
            },
            success: function (str, textStatus, request) {
                layer.close(index);
                if (request.getResponseHeader('IsScript') === 'true') {
                    eval(str);
                }
                else {
                    data = "<div id='" + $.cookie("divid") + "' class='layui-card-body donotuse_pdiv'>" + str + "</div>";
                    var p = $("#" + divid).parent();
                    p.html(data);
                }
            }
        });

    },

    OpenDialog: function (url, windowid, title, width, height, para, maxed) {
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
        if ("undefined" !== typeof DONOTUSE_WINDOWGUID) {
            this.SetCookie("windowguid", DONOTUSE_WINDOWGUID, true);
        }
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
            error: function (xhr) {
                layer.close(index);
                let location = xhr.getResponseHeader("Location");
                if (location) {
                    window.location = location;
                    return false;
                }
                ff.SetCookie("windowids", owid);
                if (xhr.responseText !== undefined && xhr.responseText !== "") {
                    layer.alert(xhr.responseText);
                }
                else {
                    layer.alert(ff.DONOTUSE_Text_LoadFailed);
                }
            },
            success: function (str, textStatus, request) {
                layer.close(index);
                max = true;
                if (request.getResponseHeader('IsScript') === 'true') {
                    ff.SetCookie("windowids", owid);
                    eval(str);
                }
                else {
                    str = "<div  id='" + $.cookie("divid") + "' class='donotuse_pdiv'>" + str + "</div>";
                    var area = 'auto';
                    if (width > document.body.clientWidth) {
                        max = false;
                        maxed = true;
                    }
                    if (width !== undefined && width !== null && height !== undefined && height !== null) {
                        area = [width + 'px', height + 'px'];
                    }
                    if (width !== undefined && width !== null && (height === undefined || height === null)) {
                        area = width + 'px';
                    }
                    if (title === undefined || title === null || title === '') {
                        title = false;
                        max = false;
                    }
                    var oid = layer.open({
                        type: 1
                        , title: title
                        , area: area
                        , maxmin: max
                        , shade: 0.8
                        , btn: []
                        , id: windowid //设定一个id，防止重复弹出
                        , content: str
                        //, success: function (layero, index) {
                        //    if (height == undefined || height == null || height == '' || max == false){
                        //        document.getElementById('layui-layer' + index).getElementsByClassName('layui-layer-content')[0].style.overflow = 'unset';
                        //    }
                        //}
                        , resizing: function (layero) {
                            ff.triggerResize();
                          $(layero).find("div[ischart = '1']").each(
                                function (index) {
                                    eval($(this).attr('id') + 'Chart.resize();');
                                }
                            );
                        }
                        , full: function (layero) {
                            ff.triggerResize();
                            $(layero).find("div[ischart = '1']").each(
                                function (index) {
                                    eval($(this).attr('id') + 'Chart.resize();');
                                }
                            );
                        }
                        , restore : function (layero) {
                            ff.triggerResize();
                          $(layero).find("div[ischart = '1']").each(
                                function (index) {
                                    eval($(this).attr('id') + 'Chart.resize();');
                                }
                            );
                        }
                        , end: function () {
                            if (ff.GetCookie("windowids") === wid) {
                                ff.SetCookie("windowids", owid);
                            }
                        }
                    });
                    if (maxed === true) {
                        layer.full(oid);
                        ff.triggerResize();
                  }
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
                if (request.responseText !== undefined && request.responseText !== "") {
                    layer.alert(request.responseText);
                }
                else {
                    layer.alert(ff.DONOTUSE_Text_LoadFailed);
                }
            },
            success: function (str) {
                var regGridId = /<\s{0,}table\s+.*\s+id\s{0,}=\s{0,}"(.*)"\s+lay-filter="\1"\s{0,}.*?>\s{0,}<\s{0,}\/\s{0,}table\s{0,}>/im;
                var regGridVar = /wtVar_(.*)\s{0,}=\s{0,}table.render\([a-zA-Z0-9_]{1,}option\)/im;
                if ($(tempId).length > 0 && regGridId.test(str) && regGridVar.test(str)) {
                    // 获取gridId
                    var gridId = regGridId.exec(str)[1];
                    var gridVar = 'wtVar_' + regGridVar.exec(str)[1];
                    var template = $(tempId)[0].innerHTML;
                    template = template.replace(/[$]{2}script[$]{2}/img, "<script>").replace(/[$]{2}#script[$]{2}/img, "</script>");
                    //get old gridid
                    try {
                        var oldgridid = /table[.]reload\('(.*)',\s{0,}{/img.exec(template)[1];
                        //替换gridId
                        template = template.replace(new RegExp(oldgridid, "gim"), gridId);
                    }
                    catch (e) { }
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
                var max = true;
                if (title === undefined || title === null || title === '') {
                    title = false;
                    max = false;
                }
                if (width > document.body.clientWidth) {
                    max = false;
                }
                var oid = layer.open({
                    type: 1
                    , title: title
                    , area: area
                    , maxmin: max
                    , btn: []
                    , shade: 0.8
                    , id: windowid //设定一个id，防止重复弹出
                    , content: str
                    , end: function () {
                        ff.SetCookie("windowids", owid);
                    }
                });
                if (width > document.body.clientWidth) {
                    layer.full(oid);
                }

            }
        });
    },

    CloseDialog: function () {
        var layer = layui.layer;
        var wid = this.GetCookie("windowids");
        if (wid !== null && wid !== '') {
            var wids = wid.split(",");
            var windowid = wids.pop();
            var index = $('#' + windowid).parent('.layui-layer').attr("times");
            layer.close(index);
            this.SetCookie("windowids", wids.join());
        }
        else {
            if (layui.setter == undefined || layui.setter.pageTabs == undefined) {
                window.close();
            }
            else if (layui.setter.pageTabs === false || $('.layadmin-tabsbody-item').length === 0) {
                $('#LAY_app_body').html('');
            }
            else {
                layui.admin.closeThisTabs();
            }
        }
    },

    ResizeChart: function (id) {
        if (layui == undefined ) {
            return;
        }
        if (id === undefined || id === null || id === '') {
            layui.use(['admin'], function () {
                layui.admin.resize(function () {
                    {
                       $("div[ischart = '1']").each(
                            function (index) {
                                eval($(this).attr('id') + 'Chart.resize();');
                            }
                        );
                    }
                });
            }
            );
        }
        else {
            layui.use(['admin'], function () {
                layui.admin.resize(function () {
                    {
                        $("#"+id).find("div[ischart = '1']").each(
                            function (index) {
                               eval($(this).attr('id') + 'Chart.resize();');
                            }
                        );
                    }
                });
            }
            );
        }
    },

    ChainChange: function (url, self) {
        var form = layui.form;
        var linkto = self.attributes["wtm-linkto"];
        if (linkto == undefined) {
            return;
        }
        var formid = self.closest("form").id
        var target = $('#' + formid).find('#' + linkto.value);
        if (target.length == 0) {
            return;
        }
        var controltype = target.attr("wtm-ctype");
        var targetfilter = target.attr("lay-filter");
        var targetname = target.attr("wtm-name");
        var ismulticombo = target.attr("wtm-combo") != undefined;
        var targetid = target.attr("id");
        var comboid = targetid;

        if (controltype == undefined) {
            controltype = "";
        }
        if (targetfilter == undefined) {
            targetfilter = "";
        }
        targetfilter += "div";
        //clear
        switch (controltype) {
            case "combo":
                window[comboid].update({ data: [] });
                break;
            case "checkbox":
                target.html('');
                form.render('checkbox', targetfilter);
                break;
            case "radio":
                target.html('');
                form.render('radio', targetfilter);
                break;
            case "tree":
                window[comboid].update({ data: [] });
                break;
            case "transfer":
                layui.transfer.reload(targetid, {
                    data: []
                });
            default:
        }
        if (url != "") {
            $.get(url, {}, function (data, status) {
                if (status === "success") {
                    var i = 0;
                    var item = null;

                    if (controltype === "tree") {
                        window[comboid].update({ data: ff.getTreeItems(data.Data) });
                    }
                    if (controltype === "transfer") {
                        layui.transfer.reload(targetid, {
                            data: ff.getTransferItems(data.Data)
                        });
                    }

                    if (controltype === "combo") {
                        window[comboid].update({ data: ff.getComboItems(data.Data) });
                    }
                    if (controltype === "checkbox") {
                        for (i = 0; i < data.Data.length; i++) {
                            item = data.Data[i];
                            if (item.Selected === true) {
                                target.append("<input type='checkbox'  name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "' checked />");
                            }
                            else {
                                target.append("<input type='checkbox' name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "'  />");
                            }
                        }
                        form.render('checkbox', targetfilter);
                    }
                    if (controltype === "radio") {
                        for (i = 0; i < data.Data.length; i++) {
                            item = data.Data[i];
                            if (item.Selected === true) {
                                target.append("<input type='radio'  name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "' checked />");
                            }
                            else {
                                target.append("<input type='radio' name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "'  />");
                            }
                        }
                        form.render('radio', targetfilter);
                    }

                }
                else {
                    layer.alert(ff.DONOTUSE_Text_FailedLoadData);
                }
            });
        }

        ff.ChainChange("", target[0], "");
    },

    LoadComboItems: function (controltype,url, controlid, targetname,svals, cb) {
        var target = $("#" + controlid);
        var targetfilter = target.attr("lay-filter");
        var ismulticombo = target.attr("wtm-combo") != undefined;
        if (svals == undefined || svals == null) {
            svals = [];
        }
       $.get(url, {}, function (data, status) {
           if (status === "success") {
               var i = 0;
               var item = null;
               if (controltype === "tree") {
                   var da = ff.getTreeItems(data.Data, svals);
                   window[controlid].update({ data: da });
                   if (cb !== undefined && cb != null) {
                       cb();
                   }
               }
               if (controltype == "transfer") {
                   layui.transfer.reload(controlid, {
                       data: ff.getTransferItems(data.Data, svals)
                   });
               }
               if (controltype === "combo") {
                   var da = ff.getComboItems(data.Data, svals);
                    window[controlid].update({ data: da });
               }
               if (controltype === "checkbox") {
                   for (i = 0; i < data.Data.length; i++) {
                       item = data.Data[i];
                       if (item.Selected === true || svals.indexOf(item.Value) > -1) {
                           target.append("<input type='checkbox'  name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "' checked />");
                       }
                       else {
                           target.append("<input type='checkbox' name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "'  />");
                       }
                   }
                   layui.form.render('checkbox', targetfilter + "div");
               }
               if (controltype === "radio") {
                   for (i = 0; i < data.Data.length; i++) {
                       item = data.Data[i];
                       if (item.Selected === true || svals.indexOf(item.Value) > -1) {
                           target.append("<input type='radio'  name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "' checked />");
                       }
                       else {
                           target.append("<input type='radio' name = '" + targetname + "' value = '" + item.Value + "' title = '" + item.Text + "'  />");
                       }
                   }
                   layui.form.render('radio', targetfilter + "div");
               }

           }

            else {
                layer.alert(ff.DONOTUSE_Text_FailedLoadData);
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

    GetFormData: function (formId) {
        var richtextbox = $("#" + formId + " textarea");
        for (var i = 0; i < richtextbox.length; i++) {
            var ra = richtextbox[i].attributes['layeditindex'];
            if (ra !== undefined && ra != null) {
                var rindex = ra.value;
                layui.layedit.sync(rindex);
            }
        }
        var searchForm = $('#' + formId), filter = {}, filterback = {}, fieldElem = searchForm.find('input,select,textarea');

        var tables = $('#' + formId + ' table[id]');
        for (var i = 0; i < tables.length; i++) {
            var tableid = tables[i].id;
            var loaddata = layui.table.cache[tableid];
            if (loaddata == undefined || loaddata.length == 0) {
                var subpro = tables[i].attributes["subpro"].value;
                if (subpro != undefined && subpro != "") {
                    filter[subpro + ".length"] = "0";
                }
            }
        }

        var xselect = searchForm.find("div[wtm-ctype='tree'],div[wtm-ctype='combo']");
        layui.each(xselect, function (_, item) {
            var val = window[item.id].getValue('value');
            if (val.length > 1) {
                fieldElem = fieldElem.filter(function (index) {
                    return this.name != item.attributes["wtm-name"].value;
                })
                $.each(val, function (i, v) {
                    fieldElem.push({ name: item.attributes["wtm-name"].value, value: v });
                });
            }
        });

        var check = {};
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox$/.test(item.type) && !item.checked) {
                if (item.value === "true") {
                    filter[item.name] = false;
                }
                return;
            }
            if (/^radio$/.test(item.type) && !item.checked) {
                return;
            }
            var itemname = item.name;
            if (/_DONOTUSE_(.*?)\[(\d?)\]\.(.*?)$/.test(itemname)) {
                var name1 = RegExp.$1;
                var number = RegExp.$2;
                var name2 = RegExp.$3;
                if (filterback.hasOwnProperty(name1) == false && filter.hasOwnProperty(name1 + "[" + number + "]." + name2) == false) {
                    filterback[name1] = 1;
                }
                return;
            }
            if (/_DONOTUSE_(.*?)$/.test(itemname)) {
                var name1 = RegExp.$1;
              if (filterback.hasOwnProperty(name1) == false && filter.hasOwnProperty(name1) == false) {
                    filterback[name1] = 1;
                }
                return;
            }
            var issub = false;
            if (/(.*?)\[(\-?\d?)\]\.(.*?)$/.test(itemname)) {
                var name1 = RegExp.$1;
                var number = RegExp.$2;
                var name2 = RegExp.$3;
                if (number == "-1") {
                    var checkname = itemname;
                    if (check.hasOwnProperty(checkname) == false) {
                        check[checkname] = 0;
                    }
                    if (filterback.hasOwnProperty(name1) == true) {
                        filterback[name1] = undefined;
                    }
                    if (filterback.hasOwnProperty(itemname) == true) {
                        filterback[itemname] = undefined;
                    }
                    var newname = itemname;
                    newname = name1 + "[" + check[checkname] + "]." + name2;
                    filter[newname] = item.value;
                    check[checkname] = check[checkname] + 1;
                    issub = true;
                }
            }
            if (issub == false) {
                if (filter.hasOwnProperty(itemname)) {
                    var temp = filter[itemname];
                    if (!(temp instanceof Array))
                        temp = [temp];
                    temp.push(item.value);
                    filter[itemname] = temp;
                }
                else {
                    filter[itemname] = item.value;
                    if (filterback.hasOwnProperty(itemname) == true && item.value != "") {
                        filterback[itemname] = undefined;
                    }
                }
            }
        });

        for (item in filterback) {
            if (filterback[item] !== undefined) {
                filter[item] = undefined;
                filter[item + ".length"] = "0";
            }
        }
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
        if (defaultcondition == null) {
            defaultcondition = {};
        }
        var tempwhere = {};
        $.extend(tempwhere, defaultcondition);

        $.extend(tempwhere, formData);
        var form = $('<form method="POST" action="' + url + '">');
        for (var attr in tempwhere) {
            if (tempwhere[attr] != null) {
                if (Array.isArray(tempwhere[attr])) {
                    for (var i = 0; i < tempwhere[attr].length; i++) {
                        form.append($('<input type="hidden" name="' + attr + '[' + i + ']" value="' + tempwhere[attr][i] + '">'));
                    }
                }
                else {
                    form.append($('<input type="hidden" name="' + attr + '" value="' + tempwhere[attr] + '">'));
                }
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

    Download: function (url, ids) {
        var form = $('<form method="POST" action="' + url + '">');
        if (ids !== undefined && ids !== null) {
            for (var i = 0; i < ids.length; i++) {
                form.append($('<input type="hidden" name="Ids" value="' + ids[i] + '">'));
            }
        }
        $('body').append(form);
        form.submit();
        form.remove();
    },

    RefreshChart: function (chartid) {
        var postdata = '';

        var searcher = $('form[chartlink*="' + chartid + '"]');
        if (searcher !== undefined && searcher.length > 0) {
            postdata = ff.GetFormData(searcher[0].id);
        }
            $.ajax({
                cache: false,
                type: 'POST',
                url: eval(chartid+"ChartUrl"),
                data: postdata,
                async: true,
                success: function (data, textStatus, request) {
                    if (data.series != undefined) {
                        data.series = data.series.replace(/"type":"charttype"/g, eval(chartid + 'ChartType'));
                    }
                    eval(chartid + 'Chart.setOption({dataset: JSON.parse(data.dataset),series: JSONfns.parse(data.series)},{replaceMerge:\'series\'});');
                    if (eval(chartid + 'ChartLegend') == 'true') {
                        eval(chartid + 'Chart.setOption({legend:JSON.parse(data.legend)});');
                    }
                }
            });
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
        var searchBtns = $('#' + dialogid + tab + ' form a[IsSearchButton]');
        if (searchBtns.length > index) {
            var sb = $('#' + searchBtns[index].id);
            var form = sb.parents("form");
            if (form.attr("oldpost") == 'True') {
                sb.trigger("click");
            }
            else {
                sb.trigger("myclick", true);
            }
        }
        else {
            if (tables.length > index) {
                layui.table.reload(tables[index].id);
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
        var re = /(<input .*?)\s*\/>(.*?)/ig;
        var re2 = /(<select .*?)\s*>(.*?<\/select>)/ig;
        var re3 = /(.*?)<input hidden name='(.*?)\.id' .*?\/>(.*?)/ig;
        for (val in data) {
            if (typeof (data[val]) == 'string') {
                data[val] = data[val].replace(/\[\d?\]/ig, "[" + loaddata.length + "]");
                data[val] = data[val].replace(/_\d?_/ig, "_" + loaddata.length + "_");
                data[val] = data[val].replace(re, "$1 onchange=\"ff.gridcellchange(this,'" + gridid + "'," + loaddata.length + ",'" + val + "',0)\" />$2");
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

    SetGridCellDate: function (id) {
        layui.use('laydate', function () {
            var laydate = layui.laydate;
            laydate.render({
                elem: '#' + id
                , show: true
                , closeStop: '#' + id
                , done: function (value, date, endDate) {
                    document.getElementById(id).onchange();
                }
            });
        });
    },

    LoadLocalData: function (gridid, option, datas, isnormaltable) {
        var re = /(<input .*?)\s*\/>/ig;
        var re2 = /(<select .*?)\s*>(.*?<\/select>)/ig;
        for (var i = 0; i < datas.length; i++) {
            var data = datas[i];
            for (val in data) {
                if (typeof (data[val]) == 'string') {
                    data[val] = data[val].replace(/[$]{2}script[$]{2}/img, "<script>").replace(/[$]{2}#script[$]{2}/img, "</script>");
                    if (isnormaltable === false) {
                        data[val] = data[val].replace(re, "$1 onchange=\"ff.gridcellchange(this,'" + gridid + "'," + i + ",'" + val + "',0)\" />");
                        data[val] = data[val].replace(re2, "$1 onchange=\"ff.gridcellchange(this,'" + gridid + "'," + i + ",'" + val + "',1)\" >$2");
                    }
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
        }, 100);
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

    getTreeItems: function (data,svals) {
        var rv = [];
        if (svals == undefined || svals == null) {
            svals = [];
        }

        for (var i = 0; i < data.length; i++) {
            var item = {};
            item.value = data[i].Value;
            item.name = data[i].Text;
            item.disabled = data[i].Disabled;
            item.selected = data[i].Selected || svals.indexOf(data[i].Value) > -1;
            item.icon = data[i].Icon;

            if (data[i].Children != null && data[i].Children.length > 0) {
                item.children = this.getTreeItems(data[i].Children, svals);
            }
            rv.push(item);
        }
        return rv;
    },

    getComboItems: function (data, svals) {
        var rv = [];
        if (svals == undefined || svals == null) {
            svals = [];
        }
        for (var i = 0; i < data.length; i++) {
            var item = {};
            item.value = data[i].Value;
            item.name = data[i].Text;
            item.disabled = data[i].Disabled;
            item.selected = data[i].Selected || svals.indexOf(data[i].Value) > -1;
            item.icon = data[i].Icon;
            if (data[i].Children != null && data[i].Children.length > 0) {
                item.children = this.getTreeItems(data[i].Children, svals);
            }
            rv.push(item);
        }
        return rv;
    },

    getTransferItems: function (data, svals) {
        var rv = [];
        if (svals == undefined || svals == null) {
            svals = [];
        }

        for (var i = 0; i < data.length; i++) {
            var item = {};
            item.value = data[i].Value;
            item.title = data[i].Text;
            item.disabled = data[i].Disabled;
            //item.checked = data[i].Selected || svals.indexOf(data[i].Value) > -1;
            rv.push(item);
        }
        return rv;
    },


    changeComboIcon: function (data) {
        for (var i = 0; i < data.elem.length; i++) {
            if (data.elem[i].value === data.value) {
                var value = data.value
                    , iconFont = $(data.elem[i]).attr('icon')
                    , comboTitle = data.othis.children('.layui-select-title')
                    , icon = comboTitle.children('._wtm-combo-icon')
                    , comboInput = comboTitle.children('input');
                if (icon.length !== 0) {
                    icon.remove();
                    $(comboInput).removeAttr("style");
                }
                if (iconFont !== undefined && iconFont !== null) {
                    icon = $('<i class="_wtm-combo-icon ' + iconFont + '"></i>');

                    comboTitle.prepend(icon);
                    $(comboInput).css({ "padding-left": "30px" });
                }
                break;
            }
        }
    },

    resetForm: function (formId) {

        $("#" + formId).find('input[type=text],select').each(function () {
            $(this).val('');
        });

        var hidAreas = [' input[wtm-tag=wtmselector]'];
        // 多选下拉框
        var multiCombos = $('#' + formId + ' select[wtm-combo=MULTI_COMBO]');
        if (multiCombos && multiCombos.length > 0) {
            for (i = 0; i < multiCombos.length; i++) {
                var name = multiCombos.attr('lay-filter');
                hidAreas.push(" input[name='" + name + "']");
            }
        }
        for (var i = 0; i < hidAreas.length; i++) {
            var hiddenAreas = $('#' + formId + hidAreas[i]);
            if (hiddenAreas && hiddenAreas.length > 0) {
                for (j = 0; j < hiddenAreas.length; j++) {
                    hiddenAreas[j].remove();
                }
            }
        }
    },

    refreshcombobox: function (select, arr) {
        layui.formSelects.on({
            layFilter: select.layFilter, left: '', right: '', separator: ',', arr: arr,
            url: select.url, self: select.self, targetname: select.targetname, linkto: select.linkto, cf: select.cf,
            selectFunc: select.selectFunc
        });
    }
};

$.ajax({
    url: '/_framework/GetScriptLanguage',
    type: 'GET',
    success: function (data) {
        for (val in data) {
            ff[val] = data[val];
        }
    }
});
