class WalkingTecUI {
    SetCookie(name, value, allwindow) {
        if (allwindow) {
            $.cookie(DONOTUSE_COOKIEPRE + name, value);
        }
        else {
            $.cookie(DONOTUSE_COOKIEPRE + DONOTUSE_WINDOWGUID + name, value);
        }
    }

    GetCookie(name, allwindow) {
        if (allwindow) {
            return $.cookie(DONOTUSE_COOKIEPRE + name);
        }
        else {
            return $.cookie(DONOTUSE_COOKIEPRE + DONOTUSE_WINDOWGUID + name);

        }
    }

    /**
     * 获取所有选中的 Id
     * @param {string} gridId
     * @returns {Array<string>}
     */
    GetSelections(gridId) {
        var checkStatus = layui.table.checkStatus(gridId);
        var data = checkStatus.data;
        var ids = [];
        if (data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                ids.push(data[i].ID);
            }
        }
        return ids;
    }

    /**
     * 是否全部选中
     * @param {string} gridId
     * @returns {boolean}
     */
    GetIsSelectAll(gridId) {
        return layui.table.checkStatus(gridId).isAll;
    }

    Alert(msg) {
        let layer = layui.layer;
        layer.alert(msg);
    }

    LoadPage(url) {
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
    }

    GetPostData(formid) {
        let datastr = $('#' + formid).serialize();
        let checkboxes = $('#' + formid + ' :checkbox');
        for (var i = 0; i < checkboxes.length; i++) {
            let ck = checkboxes[i];
            if (ck.checked === false && (ck.value == true || ck.value == false)) {
                datastr += "&" + ck.name + "=false";
            }
        }
        return datastr;
    }

    PostForm(url, formid, divid) {
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
    }

    OpenDialog(url, windowid, title, width, height, para) {
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
                var x = layer.open({
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
                ff.SetCookie("toplayerindex", x);
            }
        });
    }

    CloseDialog() {
        let layer = layui.layer;
        var wid = this.GetCookie("windowids");
        if (wid !== null && wid !== '') {
            layer.close(ff.GetCookie("toplayerindex"));
        }
        else {
            $('#DONOTUSE_MAINPANEL').html('');
        }
    }

    CloseAllDialog() {
        let layer = layui.layer;
        layer.closeAll();
    }

    LinkedChange(url, target) {
        $.get(url, {}, function (data, status) {
            if (status === "success") {
                $('#target').html('<option value = "">请选择</option>');
                for (let item of data) {
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
            layer.close(index);
        });

    }

    /**
     * 获取表单数据数组
     * @param {string} formId
     * @returns {Array<object>}
     */
    GetFormArray(formId) {
        var searchForm = $('#' + formId), filter = [], fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            if (item.value !== null && item.value !== "")
                filter.push({ name: item.name, value: item.value });
        });
        return filter;
    }

    /**
     * 获取表单数据
     * @param {string} formId
     * @returns {object}
     */
    GetFormDataWithoutNull(formId) {
        var searchForm = $('#' + formId), filter = {}, fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            if (item.value !== null && item.value !== "")
                filter[item.name] = item.value;
        });
        return filter;
    }

    /**
     * 获取表单数据
     * @param {string} formId
     * @returns {object}
     */
    GetFormData(formId) {
        var searchForm = $('#' + formId), filter = {}, fieldElem = searchForm.find('input,select,textarea');
        layui.each(fieldElem, function (_, item) {
            if (!item.name) return;
            if (/^checkbox|radio$/.test(item.type) && !item.checked) return;
            filter[item.name] = item.value;
        });
        return filter;
    }

    /**
     * 下载 Excel 或者 Pdf
     * @param {string} url
     * @param {string} formId
     */
    DownloadExcelOrPdf(url, formId) {
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
    }

    /**
     * RefreshGrid
     * @param {string} dialogid
     */
    RefreshGrid(dialogid) {
        var searchBtns = $('#' + dialogid + ' form a[class*=layui-btn]');
        if (searchBtns.length > 0) {
            searchBtns[0].click();
        } else {
            table.reload($('#' + dialogid + ' table')[0].id);
        }
    }
}

window.ff = new WalkingTecUI();
window.onbeforeunload = function () {
    ff.SetCookie("windowids", null);
    ff.SetCookie("toplayerindex", null);
}