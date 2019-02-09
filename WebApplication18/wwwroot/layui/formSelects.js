(function (layui) {
  /* 为多选下拉框添加必填验证 */
  layui.form.config.verify = $.extend(layui.form.config.verify, {
    selectRequired: function (value, item) {
      var name = item.attributes['wtm-name'].value;
      var parentForm = $(item).parents('.layui-form');
      if (parentForm.find('input[name="' + name + '"]').length == 0) {
        return '必填项不能为空';
      }
    }
  })
  return window.formSelects = formSelects = {
    selects: {},
    on: function (options) {//开启

      if (!options || !options.layFilter) {
        alert('请传入lay-filter');
        return;
      }
      var selFilter = options.layFilter;
      // 保存多选下拉的配置
      formSelects.selects[selFilter] = $.extend({}, {
        layFilter: '',
        left: '【',
        right: '】',
        separator: '',
        $dom: null,
        arr: []
      }, options);

      layui.use(['form', 'jquery'], function () {
        var form = layui.form, $ = layui.jquery, select = formSelects.selects[selFilter];
        select.$dom = $('select[lay-filter="' + select.layFilter + '"]').next();
        select.$dom.find('dl').css('display', 'none');
        formSelects.refresh(select);
        formSelects.show(select);

        form.on('select(' + select.layFilter + ')', function (data) {
          var $choose = formSelects.exchange(data);
          //如果所选有值, 放到数组中
          if ($choose) {
            var include = false;
            for (var i in select.arr) {
              if (select.arr[i] && select.arr[i].val == $choose.val) {
                select.arr.splice(i, 1);
                include = true;
              }
            }
            if (!include) {
              select.arr.push($choose);
            }
          }
          formSelects.refresh(select);
          //调整渲染的Select显示
          formSelects.show(select);
          //取消收缩效果
          select.$dom.find('dl').css('display', 'block');
          //这行代码是用于展示数据结果的
          if (options.selectFunc) {
            options.selectFunc(select.arr);
          }
        });

        $(document).on('click', 'select[lay-filter="' + select.layFilter + '"] + div input', function() {
          formSelects.show(select);
        });
        $(document).on('click', 'body:not(select[lay-filter="' + select.layFilter + '"] + div)', function (e) {
          var showFlag = $(e.target).parents('.layui-form-select').prev().attr('lay-filter') == select.layFilter;
          var thisFlag = select.$dom.find('dl').css('display') == 'block';
          if (showFlag) {//点击的input框
            select.$dom.find('dl').css('display', thisFlag ? 'none' : 'block');
          } else {
            if (thisFlag) {
              select.$dom.find('dl').css('display', 'none');
            }
          }
        });
      });
    },
    show: function (select) {
      select.$dom.find('.layui-this').removeClass('layui-this');
      var input_val = '';
      for (var i in select.arr) {
        var obj = select.arr[i];
        if (obj) {
          input_val += select.separator + select.left + obj.name + select.right;
          select.$dom.find('dd[lay-value="' + obj.val + '"]').addClass('layui-this');
        }
      }
      if (select.separator && select.separator.length > 0 && input_val.startsWith(select.separator)) {
        input_val = input_val.substr(select.separator.length);
      }
      select.$dom.find('.layui-select-title input').val(input_val);
    },
    refresh: function (select) {
      // 找到要提交的表单
      var temp = $('#' + select.layFilter.replace(/[.]{1}/img, '_'));
      if (temp.length > 0) temp[0].name = "";

      // 找到 formSelects 生成的标签
      var subTag = $('input[name="' + select.layFilter + '"]');
      for (var i = 0; i < subTag.length; i++) {
        subTag[i].remove();
      }
      for (var i = 0; i < select.arr.length; i++) {
        temp.parent().append('<input name="' + select.layFilter + '" hidden value="' + select.arr[i].val + '"></input>')
      }
    },
    exchange: function (data) {
      if (data.value) {
        return {
          name: $(data.elem).find('option[value=' + data.value + ']').text(),
          val: data.value
        }
      }
    }
  };
})(layui);
