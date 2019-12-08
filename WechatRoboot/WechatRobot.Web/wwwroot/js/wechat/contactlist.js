$(function () {
    InitMainTable();
});

function InitMainTable() {
    //记录页面bootstrap-table全局变量$table，方便应用
    var queryUrl = '/Wechat/GetContactList';
    $('#table').bootstrapTable({
        url: queryUrl,                      //请求后台的URL（*）
        method: 'POST',                     //请求方式（*）
        //toolbar: '#toolbar',              //工具按钮用哪个容器
        striped: true,                      //是否显示行间隔色
        cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
        pagination: true,                   //是否显示分页（*）
        sortable: true,                     //是否启用排序
        sortOrder: "asc",                   //排序方式
        sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
        pageNumber: 1,                      //初始化加载第一页，默认第一页,并记录
        pageSize: 10,                       //每页的记录行数（*）
        pageList: [10, 25, 50, 100],        //可供选择的每页的行数（*）
        search: false,                      //是否显示表格搜索
        strictSearch: true,
        showColumns: true,                  //是否显示所有的列（选择显示的列）
        showRefresh: true,                  //是否显示刷新按钮
        minimumCountColumns: 2,             //最少允许的列数
        clickToSelect: true,                //是否启用点击选中行
        //height: 500,                      //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
        uniqueId: "UserName",               //每一行的唯一标识，一般为主键列
        showToggle: true,                   //是否显示详细视图和列表视图的切换按钮
        cardView: false,                    //是否显示详细视图
        detailView: false,                  //是否显示父子表
        contentType: "application/x-www-form-urlencoded",
        columns: [{
            checkbox: true,
            visible: true                  //是否显示复选框
        }, {
            field: 'UserName',
            title: '用户ID'
        }, {
            field: 'NickName',
            title: '昵称'
        }, {
            field: 'RemarkName',
            title: '备注名'
        }, {
            field: 'SexCN',
            title: '性别'
        }, {
            field: 'VerifyFlagCN',
            title: '类型'
        }, {
            field: 'Province',
            title: '省份'
        }, {
            field: 'City',
            title: '城市'
        }, {
            field: 'Signature',
            title: '签名'
        }, {
            field: 'UserName',
            title: '操作',
            width: 120,
            align: 'center',
            valign: 'middle',
            formatter: ActionFormatter
        }],
        //得到查询的参数
        queryParams: function (params) {
            var temp = {
                Limit: params.limit,
                Offset: params.offset
            };
            return temp;
        },
        responseHandler: function (res) {
            if (res.Success) {
                return {
                    "rows": res.Data,
                    "total": res.Total
                };
            }
            else {
                layer.alert(res.Desc, { icon: 2 });
            }
        },
        onLoadSuccess: function (data) {
        },
        onLoadError: function () {
            showTips("数据加载失败！");
        }
    });
}

function ActionFormatter(value, row, index) {
    var id = value;
    var result = "";
    result += "<a href='javascript:;' class='btn btn-xs green' onclick=\"ViewById('" + id + "')\" title='查看'><span class='glyphicon glyphicon-search'></span></a>";   
    result += "<a href='javascript:;' class='btn btn-xs blue' onclick=\"EditById('" + id + "')\" title='编辑'><span class='glyphicon glyphicon-pencil'></span></a>";    
    result += "<a href='javascript:;' class='btn btn-xs red' onclick=\"DeleteById('" + id + "')\" title='删除'><span class='glyphicon glyphicon-remove'></span></a>";
    return result;
}