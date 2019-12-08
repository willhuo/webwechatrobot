function GetLoginFlag() {
    $.ajax({
        type: "GET",
        dataType: "JSON",
        url: "/Wechat/GetLoginFlag",
        success: function (data) {            
            if (data.Success) {
                console.log(data);
                if (data.Data) {
                    location.reload();
                }
            } else {
                console.log(data.Desc);
            }
        },
        error: function (data) {
            layer.alert(data.Desc, { icon: 2 });
        }
    });
}